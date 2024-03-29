﻿using FluentValidation;
using Logitar.Identity.Contracts.Settings;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using MediatR;
using SkillCraft.Application.Accounts.Validators;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Application.Accounts.Commands;

internal class SignInCommandHandler : IRequestHandler<SignInCommand, SignInResult>
{
  private const string AuthenticationTokenType = "auth+jwt";
  private const string MultiFactorAuthenticationPurpose = "MultiFactorAuthentication";
  private const string MultiFactorAuthenticationTemplate = "MultiFactorAuthentication";
  private const string PasswordlessTemplate = "AccountAuthentication";
  private const string ProfileTokenType = "profile+jwt";

  private readonly IMessageService _messageService;
  private readonly IOneTimePasswordService _oneTimePasswordService;
  private readonly IRealmService _realmService;
  private readonly ISessionService _sessionService;
  private readonly ITokenService _tokenService;
  private readonly IUserService _userService;

  public SignInCommandHandler(IMessageService messageService, IOneTimePasswordService oneTimePasswordService,
    IRealmService realmService, ISessionService sessionService, ITokenService tokenService, IUserService userService)
  {
    _messageService = messageService;
    _oneTimePasswordService = oneTimePasswordService;
    _realmService = realmService;
    _sessionService = sessionService;
    _tokenService = tokenService;
    _userService = userService;
  }

  public async Task<SignInResult> Handle(SignInCommand command, CancellationToken cancellationToken)
  {
    IUserSettings userSettings = await _realmService.GetUserSettingsAsync(cancellationToken);

    SignInPayload payload = command.Payload;
    new SignInValidator(userSettings.Password).ValidateAndThrow(payload);

    if (payload.Credentials != null)
    {
      return await HandleCredentialsAsync(payload.Credentials, payload.Locale, command.SessionAttributes, cancellationToken);
    }
    else if (!string.IsNullOrWhiteSpace(payload.Token))
    {
      return await HandleTokenAsync(payload.Token.Trim(), command.SessionAttributes, cancellationToken);
    }
    else if (payload.OneTimePassword != null)
    {
      return await HandleOneTimePasswordAsync(payload.OneTimePassword, command.SessionAttributes, cancellationToken);
    }
    else if (payload.Profile != null)
    {
      return await CompleteProfileAsync(payload.Profile, command.SessionAttributes, cancellationToken);
    }

    throw new InvalidOperationException($"The {nameof(SignInPayload)} properties are all null.");
  }

  private async Task<SignInResult> HandleCredentialsAsync(CredentialsPayload credentials, string locale, IEnumerable<CustomAttribute> sessionAttributes, CancellationToken cancellationToken)
  {
    User? user = await _userService.FindAsync(credentials.EmailAddress, cancellationToken);
    if (user == null || !user.HasPassword)
    {
      Email email = user?.Email ?? new(credentials.EmailAddress);
      CreatedToken createdToken = await _tokenService.CreateAsync(user?.GetSubject(), email, AuthenticationTokenType, cancellationToken);
      Dictionary<string, string> variables = new()
      {
        ["Token"] = createdToken.Token
      };
      SentMessages sentMessages = user == null
        ? await _messageService.SendAsync(PasswordlessTemplate, email, locale, variables, cancellationToken)
        : await _messageService.SendAsync(PasswordlessTemplate, user, locale, variables, cancellationToken);
      SentMessage sentMessage = sentMessages.ToSentMessage(email);
      return SignInResult.AuthenticationLinkSent(sentMessage);
    }
    else if (credentials.Password == null)
    {
      return SignInResult.RequirePassword();
    }

    MultiFactorAuthenticationMode? mfaMode = user.GetMultiFactorAuthenticationMode();
    if (mfaMode == MultiFactorAuthenticationMode.None && user.IsProfileCompleted())
    {
      Session session = await _sessionService.SignInAsync(user, credentials.Password, sessionAttributes, cancellationToken);
      return SignInResult.Succeed(session);
    }
    else
    {
      user = await _userService.AuthenticateAsync(user, credentials.Password, cancellationToken);
    }

    return mfaMode switch
    {
      MultiFactorAuthenticationMode.Email => await SendMultiFactorAuthenticationEmailMessageAsync(user, locale, cancellationToken),
      MultiFactorAuthenticationMode.Phone => throw new NotSupportedException(), // ISSUE #5: Text message (SMS) multi-factor authentication
      _ => await EnsureProfileIsCompleted(user, sessionAttributes, cancellationToken),
    };
  }
  private async Task<SignInResult> SendMultiFactorAuthenticationEmailMessageAsync(User user, string locale, CancellationToken cancellationToken)
  {
    if (user.Email == null)
    {
      throw new ArgumentException($"The user 'Id={user.Id}' has no email.", nameof(user));
    }

    OneTimePassword oneTimePassword = await _oneTimePasswordService.CreateAsync(user, MultiFactorAuthenticationPurpose, cancellationToken);
    if (oneTimePassword.Password == null)
    {
      throw new InvalidOperationException($"The One-Time Password (OTP) 'Id={oneTimePassword.Id}' has no password.");
    }
    Dictionary<string, string> variables = new()
    {
      ["OneTimePassword"] = oneTimePassword.Password
    };
    SentMessages sentMessages = await _messageService.SendAsync(MultiFactorAuthenticationTemplate, user, locale, variables, cancellationToken);
    SentMessage sentMessage = sentMessages.ToSentMessage(user.Email);
    return SignInResult.RequireOneTimePasswordValidation(oneTimePassword, sentMessage);
  }

  private async Task<SignInResult> HandleTokenAsync(string token, IEnumerable<CustomAttribute> sessionAttributes, CancellationToken cancellationToken)
  {
    ValidatedToken validatedToken = await _tokenService.ValidateAsync(token, AuthenticationTokenType, cancellationToken);
    User user;
    if (validatedToken.Subject == null)
    {
      Email email = validatedToken.Email ?? throw new InvalidOperationException($"The '{nameof(validatedToken.Email)}' claims are required.");
      email.IsVerified = true;
      user = await _userService.CreateAsync(email, cancellationToken);
    }
    else
    {
      Guid userId = Guid.Parse(validatedToken.Subject);
      user = await _userService.FindAsync(userId, cancellationToken) ?? throw new InvalidOperationException($"The user 'Id={userId}' could not be found.");
      if (validatedToken.Email != null)
      {
        Email email = new(validatedToken.Email.Address)
        {
          IsVerified = true
        };
        if (user.Email == null || user.Email.Address != email.Address || user.Email.IsVerified != email.IsVerified)
        {
          user = await _userService.UpdateEmailAsync(user, email, cancellationToken);
        }
      }
    }

    return await EnsureProfileIsCompleted(user, sessionAttributes, cancellationToken);
  }

  private async Task<SignInResult> HandleOneTimePasswordAsync(OneTimePasswordPayload oneTimePasswordPayload, IEnumerable<CustomAttribute> sessionAttributes, CancellationToken cancellationToken)
  {
    OneTimePassword oneTimePassword = await _oneTimePasswordService.ValidateAsync(oneTimePasswordPayload, cancellationToken);
    oneTimePassword.EnsurePurpose(MultiFactorAuthenticationPurpose);
    Guid userId = oneTimePassword.GetUserId();
    User user = await _userService.FindAsync(userId, cancellationToken) ?? throw new InvalidOperationException($"The user 'Id={userId}' could not be found.");

    return await EnsureProfileIsCompleted(user, sessionAttributes, cancellationToken);
  }

  private async Task<SignInResult> CompleteProfileAsync(CompleteProfilePayload profile, IEnumerable<CustomAttribute> sessionAttributes, CancellationToken cancellationToken)
  {
    ValidatedToken validatedToken = await _tokenService.ValidateAsync(profile.Token, ProfileTokenType, cancellationToken);
    if (validatedToken.Subject == null)
    {
      throw new InvalidOperationException($"The claim '{validatedToken.Subject}' is required.");
    }
    Guid userId = Guid.Parse(validatedToken.Subject);
    User user = await _userService.FindAsync(userId, cancellationToken) ?? throw new InvalidOperationException($"The user 'Id={userId}' could not be found.");
    user = await _userService.SaveProfileAsync(user, profile, cancellationToken);

    return await EnsureProfileIsCompleted(user, sessionAttributes, cancellationToken);
  }

  private async Task<SignInResult> EnsureProfileIsCompleted(User user, IEnumerable<CustomAttribute> sessionAttributes, CancellationToken cancellationToken)
  {
    if (!user.IsProfileCompleted())
    {
      CreatedToken createdToken = await _tokenService.CreateAsync(user.GetSubject(), ProfileTokenType, cancellationToken);
      return SignInResult.RequireProfileCompletion(createdToken);
    }

    Session session = await _sessionService.CreateAsync(user, sessionAttributes, cancellationToken);
    return SignInResult.Succeed(session);
  }
}
