using FluentValidation;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using MediatR;
using SkillCraft.Application.Accounts.Constants;
using SkillCraft.Application.Accounts.Events;
using SkillCraft.Application.Accounts.Validators;
using SkillCraft.Contracts.Accounts;
using SkillCraft.Domain;

namespace SkillCraft.Application.Accounts.Commands;

internal class SignInAccountCommandHandler : IRequestHandler<SignInAccountCommand, SignInAccountResult>
{
  private readonly IMessageService _messageService;
  private readonly IOneTimePasswordService _oneTimePasswordService;
  private readonly IPublisher _publisher; // TODO(fpion): handler
  private readonly ISessionService _sessionService;
  private readonly ITokenService _tokenService;
  private readonly IUserService _userService;

  public SignInAccountCommandHandler(IMessageService messageService,
    IOneTimePasswordService oneTimePasswordService,
    IPublisher publisher,
    ISessionService sessionService,
    ITokenService tokenService,
    IUserService userService)
  {
    _messageService = messageService;
    _oneTimePasswordService = oneTimePasswordService;
    _publisher = publisher;
    _sessionService = sessionService;
    _tokenService = tokenService;
    _userService = userService;
  }

  public async Task<SignInAccountResult> Handle(SignInAccountCommand command, CancellationToken cancellationToken)
  {
    SignInAccountPayload payload = command.Payload;
    new SignInAccountValidator().ValidateAndThrow(payload);
    LocaleUnit locale = new(payload.Locale);

    if (payload.Credentials != null)
    {
      return await HandleCredentialsAsync(payload.Credentials, locale, command.CustomAttributes, cancellationToken);
    }
    else if (!string.IsNullOrWhiteSpace(payload.AuthenticationToken))
    {
      return await HandleAuthenticationToken(payload.AuthenticationToken.Trim(), command.CustomAttributes, cancellationToken);
    }
    else if (payload.OneTimePassword != null)
    {
      return await HandleOneTimePasswordAsync(payload.OneTimePassword, command.CustomAttributes, cancellationToken);
    }

    throw new ArgumentException($"Exactly one of the following must be specified: {nameof(payload.Credentials)}, {nameof(payload.AuthenticationToken)}.", nameof(command));
  }

  private async Task<SignInAccountResult> HandleCredentialsAsync(Credentials credentials, LocaleUnit locale, IEnumerable<CustomAttribute> customAttributes, CancellationToken cancellationToken)
  {
    User? user = await _userService.FindAsync(credentials.EmailAddress, cancellationToken);
    if (user == null || !user.HasPassword)
    {
      Email? email = user?.Email ?? new(credentials.EmailAddress);
      CreatedToken authentication = await _tokenService.CreateAsync(user, email, TokenTypes.Authentication, cancellationToken);

      Dictionary<string, string> variables = new()
      {
        [Variables.Token] = authentication.Token
      };
      SentMessages sentMessages = user == null
        ? await _messageService.SendAsync(Templates.AccountAuthentication, email, locale, variables, cancellationToken)
        : await _messageService.SendAsync(Templates.AccountAuthentication, user, ContactType.Email, locale, variables, cancellationToken);
      SentMessage sentMessage = sentMessages.ToSentMessage(email);
      return SignInAccountResult.AuthenticationLinkSent(sentMessage);
    }
    else if (credentials.Password == null)
    {
      return SignInAccountResult.RequirePassword();
    }

    MultiFactorAuthenticationMode? multiFactorAuthenticationMode = user.GetMultiFactorAuthenticationMode();
    if (multiFactorAuthenticationMode == MultiFactorAuthenticationMode.None && user.IsProfileCompleted())
    {
      Session session = await _sessionService.SignInAsync(user, credentials.Password, customAttributes, cancellationToken);
      await _publisher.Publish(new UserSignedInEvent(session), cancellationToken);
      return SignInAccountResult.Success(session);
    }
    else
    {
      user = await _userService.AuthenticateAsync(user, credentials.Password, cancellationToken);
    }

    return multiFactorAuthenticationMode switch
    {
      MultiFactorAuthenticationMode.Email => await SendMultiFactorAuthenticationMessageAsync(user, ContactType.Email, locale, cancellationToken),
      MultiFactorAuthenticationMode.Phone => await SendMultiFactorAuthenticationMessageAsync(user, ContactType.Phone, locale, cancellationToken),
      _ => await EnsureProfileIsCompletedAsync(user, customAttributes, cancellationToken),
    };
  }
  private async Task<SignInAccountResult> SendMultiFactorAuthenticationMessageAsync(User user, ContactType contactType, LocaleUnit locale, CancellationToken cancellationToken)
  {
    Contact contact = contactType switch
    {
      ContactType.Email => user.Email ?? throw new ArgumentException($"The user 'Id={user.Id}' has no email.", nameof(user)),
      ContactType.Phone => user.Phone ?? throw new ArgumentException($"The user 'Id={user.Id}' has no phone.", nameof(user)),
      _ => throw new ArgumentException($"The contact type '{contactType}' is not supported.", nameof(contactType)),
    };
    OneTimePassword oneTimePassword = await _oneTimePasswordService.CreateAsync(user, Purposes.MultiFactorAuthentication, cancellationToken);
    if (oneTimePassword.Password == null)
    {
      throw new InvalidOperationException($"The One-Time Password (OTP) 'Id={oneTimePassword.Id}' has no password.");
    }
    Dictionary<string, string> variables = new()
    {
      [Variables.OneTimePassword] = oneTimePassword.Password
    };
    string template = Templates.GetMultiFactorAuthentication(contactType);
    SentMessages sentMessages = await _messageService.SendAsync(template, user, contactType, locale, variables, cancellationToken);
    SentMessage sentMessage = sentMessages.ToSentMessage(contact);
    return SignInAccountResult.RequireOneTimePasswordValidation(oneTimePassword, sentMessage);
  }

  private async Task<SignInAccountResult> HandleAuthenticationToken(string authenticationToken, IEnumerable<CustomAttribute> customAttributes, CancellationToken cancellationToken)
  {
    ValidatedToken token = await _tokenService.ValidateAsync(authenticationToken, TokenTypes.Authentication, cancellationToken);
    if (token.Email == null)
    {
      throw new ArgumentException("The email claims are required.", nameof(authenticationToken));
    }
    EmailPayload email = new(token.Email.Address, isVerified: true);

    User user;
    if (token.Subject == null)
    {
      user = await _userService.CreateAsync(email, cancellationToken);
    }
    else
    {
      if (!Guid.TryParse(token.Subject, out Guid userId))
      {
        throw new ArgumentException($"The value '{token.Subject}' is not a valid Guid.", nameof(authenticationToken));
      }
      user = await _userService.FindAsync(userId, cancellationToken) ?? throw new ArgumentException($"The user 'Id={userId}' could not be found.", nameof(authenticationToken));
      user = await _userService.UpdateAsync(user, email, cancellationToken);
    }

    return await EnsureProfileIsCompletedAsync(user, customAttributes, cancellationToken);
  }

  private async Task<SignInAccountResult> HandleOneTimePasswordAsync(OneTimePasswordPayload payload, IEnumerable<CustomAttribute> customAttributes, CancellationToken cancellationToken)
  {
    OneTimePassword oneTimePassword = await _oneTimePasswordService.ValidateAsync(payload, Purposes.MultiFactorAuthentication, cancellationToken);
    Guid userId = oneTimePassword.GetUserId();
    User user = await _userService.FindAsync(userId, cancellationToken) ?? throw new ArgumentException($"The user 'Id={userId}' could not be found.", nameof(payload));

    return await EnsureProfileIsCompletedAsync(user, customAttributes, cancellationToken);
  }

  private async Task<SignInAccountResult> EnsureProfileIsCompletedAsync(User user, IEnumerable<CustomAttribute> customAttributes, CancellationToken cancellationToken)
  {
    if (!user.IsProfileCompleted())
    {
      CreatedToken token = await _tokenService.CreateAsync(user, TokenTypes.Profile, cancellationToken);
      return SignInAccountResult.RequireProfileCompletion(token);
    }

    Session session = await _sessionService.CreateAsync(user, customAttributes, cancellationToken);
    await _publisher.Publish(new UserSignedInEvent(session), cancellationToken);
    return SignInAccountResult.Success(session);
  }
}
