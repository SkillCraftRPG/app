using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using MediatR;
using SkillCraft.Application.Accounts.Constants;
using SkillCraft.Application.Accounts.Validators;
using SkillCraft.Application.Actors;
using SkillCraft.Application.Settings;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Application.Accounts.Commands;

internal class SignInCommandHandler : IRequestHandler<SignInCommand, SignInCommandResult>
{
  private readonly AccountSettings _accountSettings;
  private readonly IActorService _actorService;
  private readonly IGoogleService _googleService;
  private readonly IMessageService _messageService;
  private readonly IOneTimePasswordService _oneTimePasswordService;
  private readonly IRealmService _realmService;
  private readonly ISessionService _sessionService;
  private readonly ITokenService _tokenService;
  private readonly IUserService _userService;

  public SignInCommandHandler(
    AccountSettings accountSettings,
    IActorService actorService,
    IGoogleService googleService,
    IMessageService messageService,
    IOneTimePasswordService oneTimePasswordService,
    IRealmService realmService,
    ISessionService sessionService,
    ITokenService tokenService,
    IUserService userService)
  {
    _accountSettings = accountSettings;
    _actorService = actorService;
    _googleService = googleService;
    _messageService = messageService;
    _oneTimePasswordService = oneTimePasswordService;
    _realmService = realmService;
    _sessionService = sessionService;
    _tokenService = tokenService;
    _userService = userService;
  }

  public async Task<SignInCommandResult> Handle(SignInCommand command, CancellationToken cancellationToken)
  {
    Realm realm = await _realmService.FindAsync(cancellationToken);

    SignInPayload payload = command.Payload;
    new SignInValidator(realm.PasswordSettings).ValidateAndThrow(payload);
    LocaleUnit locale = new(payload.Locale);

    if (payload.Credentials != null)
    {
      return await HandleCredentialsAsync(payload.Credentials, locale, command.CustomAttributes, cancellationToken);
    }
    else if (!string.IsNullOrWhiteSpace(payload.AuthenticationToken))
    {
      return await HandleAuthenticationToken(payload.AuthenticationToken.Trim(), command.CustomAttributes, cancellationToken);
    }
    else if (!string.IsNullOrWhiteSpace(payload.GoogleIdToken))
    {
      return await HandleGoogleIdTokenAsync(payload.GoogleIdToken.Trim(), locale, command.CustomAttributes, cancellationToken);
    }
    else if (payload.OneTimePassword != null)
    {
      return await HandleOneTimePasswordAsync(payload.OneTimePassword, command.CustomAttributes, cancellationToken);
    }
    else if (payload.Profile != null)
    {
      return await HandleProfileAsync(payload.Profile, command.CustomAttributes, cancellationToken);
    }

    throw new ArgumentException($"Exactly one of the following must be specified: {nameof(payload.Credentials)}, {nameof(payload.AuthenticationToken)}, {nameof(payload.GoogleIdToken)}, {nameof(payload.OneTimePassword)}, {nameof(payload.Profile)}.", nameof(command));
  }

  private async Task<SignInCommandResult> HandleCredentialsAsync(Credentials credentials, LocaleUnit locale, IEnumerable<CustomAttribute> customAttributes, CancellationToken cancellationToken)
  {
    User? user = await _userService.FindAsync(credentials.EmailAddress, cancellationToken);
    if (user == null || !user.HasPassword)
    {
      Email email = user?.Email ?? new(credentials.EmailAddress);
      CreatedToken authentication = await _tokenService.CreateAsync(user, email, TokenTypes.Authentication, cancellationToken);
      Dictionary<string, string> variables = new()
      {
        [Variables.Token] = authentication.Token
      };
      SentMessages sentMessages = user == null
        ? await _messageService.SendAsync(Templates.AccountAuthentication, email, locale, variables, cancellationToken)
        : await _messageService.SendAsync(Templates.AccountAuthentication, user, ContactType.Email, locale, variables, cancellationToken);
      SentMessage sentMessage = sentMessages.ToSentMessage(email);
      return SignInCommandResult.AuthenticationLinkSent(sentMessage);
    }
    else if (credentials.Password == null)
    {
      return SignInCommandResult.RequirePassword();
    }

    MultiFactorAuthenticationMode? multiFactorAuthenticationMode = user.GetMultiFactorAuthenticationMode();
    if (multiFactorAuthenticationMode == MultiFactorAuthenticationMode.None && user.IsProfileCompleted())
    {
      Session session = await _sessionService.SignInAsync(user, credentials.Password, customAttributes, cancellationToken);
      await _actorService.SaveAsync(user, cancellationToken);
      return SignInCommandResult.Success(session);
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
  private async Task<SignInCommandResult> SendMultiFactorAuthenticationMessageAsync(User user, ContactType contactType, LocaleUnit locale, CancellationToken cancellationToken)
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
    return SignInCommandResult.RequireOneTimePasswordValidation(oneTimePassword, sentMessage);
  }

  private async Task<SignInCommandResult> HandleAuthenticationToken(string authenticationToken, IEnumerable<CustomAttribute> customAttributes, CancellationToken cancellationToken)
  {
    ValidatedToken validatedToken = await _tokenService.ValidateAsync(authenticationToken, TokenTypes.Authentication, cancellationToken);
    EmailPayload email = validatedToken.GetEmailPayload();
    email.IsVerified = true;

    User user;
    if (validatedToken.Subject == null)
    {
      user = await _userService.CreateAsync(email, cancellationToken);
    }
    else
    {
      Guid userId = validatedToken.GetUserId();
      user = await _userService.FindAsync(userId, cancellationToken) ?? throw new ArgumentException($"The user 'Id={userId}' could not be found.", nameof(authenticationToken));

      if (user.Email == null || !user.Email.IsEqualTo(email))
      {
        user = await _userService.UpdateAsync(user, email, cancellationToken);
      }
    }

    return await EnsureProfileIsCompletedAsync(user, customAttributes, cancellationToken);
  }

  private async Task<SignInCommandResult> HandleGoogleIdTokenAsync(string googleIdToken, LocaleUnit locale, IEnumerable<CustomAttribute> customAttributes, CancellationToken cancellationToken)
  {
    GoogleIdentity identity = await _googleService.GetIdentityAsync(googleIdToken, cancellationToken);

    CustomIdentifier identifier = new(Identifiers.Google, identity.Id);
    User? user = await _userService.FindAsync(identifier, cancellationToken);
    if (user == null)
    {
      user = await _userService.FindAsync(identity.Email.Address, cancellationToken);
      if (user == null)
      {
        user = await _userService.CreateAsync(identity.Email, identifier, cancellationToken);
      }
      else
      {
        user = await _userService.SaveIdentifierAsync(user, identifier, cancellationToken);
      }
    }

    if (!user.IsProfileCompleted() && identity.FirstName != null && identity.LastName != null)
    {
      CompleteProfilePayload payload = new(googleIdToken, identity.FirstName, identity.LastName, locale.Code, _accountSettings.DefaultTimeZone)
      {
        Picture = identity.Picture
      };
      user = await _userService.CompleteProfileAsync(user, payload, phone: null, cancellationToken);
    }

    return await EnsureProfileIsCompletedAsync(user, customAttributes, cancellationToken);
  }

  private async Task<SignInCommandResult> HandleOneTimePasswordAsync(OneTimePasswordPayload payload, IEnumerable<CustomAttribute> customAttributes, CancellationToken cancellationToken)
  {
    OneTimePassword oneTimePassword = await _oneTimePasswordService.ValidateAsync(payload, Purposes.MultiFactorAuthentication, cancellationToken);
    Guid userId = oneTimePassword.GetUserId();
    User user = await _userService.FindAsync(userId, cancellationToken) ?? throw new ArgumentException($"The user 'Id={userId}' could not be found.", nameof(payload));

    return await EnsureProfileIsCompletedAsync(user, customAttributes, cancellationToken);
  }

  private async Task<SignInCommandResult> HandleProfileAsync(CompleteProfilePayload payload, IEnumerable<CustomAttribute> customAttributes, CancellationToken cancellationToken)
  {
    ValidatedToken validatedToken = await _tokenService.ValidateAsync(payload.Token, TokenTypes.Profile, cancellationToken);
    if (validatedToken.Subject == null)
    {
      throw new ArgumentException($"The '{nameof(validatedToken.Subject)}' claim is required.", nameof(payload));
    }
    Guid userId = validatedToken.GetUserId();
    User user = await _userService.FindAsync(userId, cancellationToken) ?? throw new ArgumentException($"The user 'Id={userId}' could not be found.", nameof(payload));
    PhonePayload? phone = validatedToken.TryGetPhonePayload();
    user = await _userService.CompleteProfileAsync(user, payload, phone, cancellationToken);

    return await EnsureProfileIsCompletedAsync(user, customAttributes, cancellationToken);
  }

  private async Task<SignInCommandResult> EnsureProfileIsCompletedAsync(User user, IEnumerable<CustomAttribute> customAttributes, CancellationToken cancellationToken)
  {
    if (!user.IsProfileCompleted())
    {
      CreatedToken profileCompletion = await _tokenService.CreateAsync(user, TokenTypes.Profile, cancellationToken);
      return SignInCommandResult.RequireProfileCompletion(profileCompletion);
    }

    Session session = await _sessionService.CreateAsync(user, customAttributes, cancellationToken);
    await _actorService.SaveAsync(user, cancellationToken);
    return SignInCommandResult.Success(session);
  }
}
