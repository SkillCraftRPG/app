using Bogus;
using FluentValidation.Results;
using Logitar;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using Logitar.Security.Claims;
using Moq;
using SkillCraft.Application.Accounts.Constants;
using SkillCraft.Application.Actors;
using SkillCraft.Application.Settings;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Application.Accounts.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SignInCommandHandlerTests
{
  private const string PasswordString = "P@s$W0rD";
  private const string TimeZoneId = "America/Montreal";

  private static readonly LocaleUnit _locale = new("fr");

  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IActorService> _actorService = new();
  private readonly Mock<IGoogleService> _googleService = new();
  private readonly Mock<IMessageService> _messageService = new();
  private readonly Mock<IOneTimePasswordService> _oneTimePasswordService = new();
  private readonly Mock<IRealmService> _realmService = new();
  private readonly Mock<ISessionService> _sessionService = new();
  private readonly Mock<ITokenService> _tokenService = new();
  private readonly Mock<IUserService> _userService = new();

  private readonly SignInSettings _settings = new(TimeZoneId);
  private readonly SignInCommandHandler _handler;

  private readonly RealmMock _realm = new();

  public SignInCommandHandlerTests()
  {
    _realmService.Setup(x => x.FindAsync(_cancellationToken)).ReturnsAsync(_realm);

    _handler = new(_actorService.Object, _googleService.Object, _messageService.Object, _oneTimePasswordService.Object, _realmService.Object, _sessionService.Object, _settings, _tokenService.Object, _userService.Object);
  }

  [Fact(DisplayName = "It should assign a new identifier to the found user without identifier (Google).")]
  public async Task It_should_assign_a_new_identifier_to_the_found_user_without_identifier_Google()
  {
    SignInPayload payload = new(_locale.Code)
    {
      GoogleIdToken = "GoogleIdToken"
    };

    EmailPayload email = new(_faker.Person.Email, isVerified: true);
    GoogleIdentity identity = new("GoogleUserId", email, FirstName: null, LastName: null, Locale: null, Picture: null);
    _googleService.Setup(x => x.GetIdentityAsync(payload.GoogleIdToken, _cancellationToken)).ReturnsAsync(identity);

    User user = new(_faker.Person.UserName)
    {
      Email = new(_faker.Person.Email)
      {
        IsVerified = true
      }
    };
    user.CustomAttributes.Add(new("ProfileCompletedOn", DateTime.Now.ToISOString()));
    _userService.Setup(x => x.FindAsync(user.Email.Address, _cancellationToken)).ReturnsAsync(user);
    _userService.Setup(x => x.SaveIdentifierAsync(user, It.Is<CustomIdentifier>(i => i.Key == "Google" && i.Value == identity.Id), _cancellationToken)).ReturnsAsync(user);

    Session session = new(user);
    _sessionService.Setup(x => x.CreateAsync(user, It.IsAny<IEnumerable<CustomAttribute>>(), _cancellationToken)).ReturnsAsync(session);

    SignInCommand command = new(payload, CustomAttributes: []);
    _ = await _handler.Handle(command, _cancellationToken);

    _userService.Verify(x => x.SaveIdentifierAsync(user, It.Is<CustomIdentifier>(i => i.Key == "Google" && i.Value == identity.Id), _cancellationToken), Times.Once);
  }

  [Theory(DisplayName = "It should complete the user profile and sign-in the user.")]
  [InlineData(null, null, null, false)]
  [InlineData(null, "CA", "12345", true)]
  [InlineData("+15148454636", null, null, false)]
  [InlineData("+15148454636", "CA", "12345", true)]
  public async Task It_should_complete_the_user_profile_and_sign_in_the_user(string? phoneNumber, string? phoneCountryCode, string? phoneExtension, bool isPhoneVerified)
  {
    SignInPayload payload = new(_locale.Code)
    {
      Profile = new CompleteProfilePayload("ProfileToken", _faker.Person.FirstName, _faker.Person.LastName, _locale.Code, TimeZoneId)
    };

    User user = new(_faker.Person.UserName)
    {
      Id = Guid.NewGuid()
    };
    user.CustomAttributes.Add(new("ProfileCompletedOn", DateTime.Now.ToISOString()));
    _userService.Setup(x => x.FindAsync(user.Id, _cancellationToken)).ReturnsAsync(user);

    ValidatedToken validatedToken = new()
    {
      Subject = user.GetSubject()
    };
    PhonePayload? phone = null;
    if (phoneNumber != null)
    {
      phone = new(phoneCountryCode, phoneNumber, phoneExtension, isPhoneVerified);

      validatedToken.Claims.Add(new TokenClaim(ClaimNames.PhoneNumberRaw, phone.Number));
      validatedToken.Claims.Add(new TokenClaim(Rfc7519ClaimNames.IsPhoneVerified, phone.IsVerified.ToString()));

      if (phone.CountryCode != null)
      {
        validatedToken.Claims.Add(new TokenClaim(ClaimNames.PhoneCountryCode, phone.CountryCode));
      }

      validatedToken.Claims.Add(new TokenClaim(Rfc7519ClaimNames.PhoneNumber, phone.Extension == null ? phone.Number : $"{phone.Number};ext={phone.Extension}"));
    }
    _tokenService.Setup(x => x.ValidateAsync(payload.Profile.Token, TokenTypes.Profile, _cancellationToken)).ReturnsAsync(validatedToken);
    _userService.Setup(x => x.CompleteProfileAsync(user, payload.Profile, phone, _cancellationToken)).ReturnsAsync(user);

    CustomAttribute[] customAttributes =
    [
      new("AdditionalInformation", $@"{{""User-Agent"":""{_faker.Internet.UserAgent()}""}}"),
      new("IpAddress", _faker.Internet.Ip())
    ];
    Session session = new(user);
    _sessionService.Setup(x => x.CreateAsync(user, customAttributes, _cancellationToken)).ReturnsAsync(session);

    SignInCommand command = new(payload, customAttributes);
    SignInCommandResult result = await _handler.Handle(command, _cancellationToken);

    Assert.Null(result.AuthenticationLinkSentTo);
    Assert.False(result.IsPasswordRequired);
    Assert.Null(result.OneTimePasswordValidation);
    Assert.Null(result.ProfileCompletionToken);
    Assert.Same(session, result.Session);
  }

  [Fact(DisplayName = "It should complete the user profile from Google identity.")]
  public async Task It_should_complete_the_user_profile_from_Google_identity()
  {
    SignInPayload payload = new(_locale.Code)
    {
      GoogleIdToken = "GoogleIdToken"
    };

    EmailPayload email = new(_faker.Person.Email, isVerified: true);
    GoogleIdentity identity = new("GoogleUserId", email, _faker.Person.FirstName, _faker.Person.LastName, _faker.Locale, _faker.Person.Avatar);
    _googleService.Setup(x => x.GetIdentityAsync(payload.GoogleIdToken, _cancellationToken)).ReturnsAsync(identity);

    User user = new(_faker.Person.UserName)
    {
      Email = new(_faker.Person.Email),
      FirstName = identity.FirstName,
      LastName = identity.LastName,
      Birthdate = _faker.Person.DateOfBirth,
      Gender = _faker.Person.Gender.ToString().ToLowerInvariant(),
      Locale = new(_locale.Code),
      TimeZone = TimeZoneId,
      Picture = identity.Picture
    };
    user.CustomAttributes.Add(new(nameof(MultiFactorAuthenticationMode), MultiFactorAuthenticationMode.None.ToString()));
    user.CustomAttributes.Add(new(nameof(UserType), UserType.Player.ToString()));
    user.CustomIdentifiers.Add(new("Google", identity.Id));
    _userService.Setup(x => x.FindAsync(user.CustomIdentifiers.Single(), _cancellationToken)).ReturnsAsync(user);
    _userService.Setup(x => x.CompleteProfileAsync(user, It.Is<CompleteProfilePayload>(y => y.Token == payload.GoogleIdToken
        && y.FirstName == identity.FirstName && y.LastName == identity.LastName
        && y.Locale == _locale.Code && y.TimeZone == TimeZoneId && y.Picture == identity.Picture), null, _cancellationToken)
      )
     .Callback(() => user.CustomAttributes.Add(new("ProfileCompletedOn", DateTime.Now.ToISOString())))
     .ReturnsAsync(user);

    CustomAttribute[] customAttributes =
    [
      new("AdditionalInformation", $@"{{""User-Agent"":""{_faker.Internet.UserAgent()}""}}"),
      new("IpAddress", _faker.Internet.Ip())
    ];
    Session session = new(user);
    _sessionService.Setup(x => x.CreateAsync(user, customAttributes, _cancellationToken)).ReturnsAsync(session);

    SignInCommand command = new(payload, customAttributes);
    SignInCommandResult result = await _handler.Handle(command, _cancellationToken);

    Assert.Null(result.AuthenticationLinkSentTo);
    Assert.False(result.IsPasswordRequired);
    Assert.Null(result.OneTimePasswordValidation);
    Assert.Null(result.ProfileCompletionToken);
    Assert.Same(session, result.Session);

    _userService.Verify(x => x.CompleteProfileAsync(user, It.Is<CompleteProfilePayload>(y => y.Token == payload.GoogleIdToken
      && y.FirstName == identity.FirstName && y.LastName == identity.LastName
      && y.Locale == _locale.Code && y.TimeZone == TimeZoneId && y.Picture == identity.Picture), null, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should create a new user from Google identity.")]
  public async Task It_should_create_a_new_user_from_Google_identity()
  {
    SignInPayload payload = new(_locale.Code)
    {
      GoogleIdToken = "GoogleIdToken"
    };

    EmailPayload email = new(_faker.Person.Email, isVerified: true);
    GoogleIdentity identity = new("GoogleUserId", email, FirstName: null, LastName: null, Locale: null, Picture: null);
    _googleService.Setup(x => x.GetIdentityAsync(payload.GoogleIdToken, _cancellationToken)).ReturnsAsync(identity);

    User user = new(_faker.Person.UserName)
    {
      Email = new(email.Address)
      {
        IsVerified = email.IsVerified
      }
    };
    user.CustomIdentifiers.Add(new("Google", identity.Id));
    _userService.Setup(x => x.CreateAsync(email, user.CustomIdentifiers.Single(), _cancellationToken)).ReturnsAsync(user);

    CreatedToken profileCompletion = new("ProfileCompletionToken");
    _tokenService.Setup(x => x.CreateAsync(user, TokenTypes.Profile, _cancellationToken)).ReturnsAsync(profileCompletion);

    SignInCommand command = new(payload, CustomAttributes: []);
    SignInCommandResult result = await _handler.Handle(command, _cancellationToken);

    Assert.Null(result.AuthenticationLinkSentTo);
    Assert.False(result.IsPasswordRequired);
    Assert.Null(result.OneTimePasswordValidation);
    Assert.Equal(profileCompletion.Token, result.ProfileCompletionToken);
    Assert.Null(result.Session);
  }

  [Fact(DisplayName = "It should create a new user.")]
  public async Task It_should_create_a_new_user()
  {
    SignInPayload payload = new(_locale.Code)
    {
      AuthenticationToken = "AuthenticationToken"
    };

    ValidatedToken validatedToken = new()
    {
      Email = new(_faker.Person.Email)
    };
    _tokenService.Setup(x => x.ValidateAsync(payload.AuthenticationToken, TokenTypes.Authentication, _cancellationToken)).ReturnsAsync(validatedToken);

    User user = new(_faker.Person.Email)
    {
      Email = new(_faker.Person.Email)
      {
        IsVerified = true
      }
    };
    _userService.Setup(x => x.CreateAsync(It.Is<EmailPayload>(e => e.Address == user.Email.Address && e.IsVerified), _cancellationToken)).ReturnsAsync(user);

    CreatedToken createdToken = new("ProfileToken");
    _tokenService.Setup(x => x.CreateAsync(user, TokenTypes.Profile, _cancellationToken)).ReturnsAsync(createdToken);

    SignInCommand command = new(payload, CustomAttributes: []);
    SignInCommandResult result = await _handler.Handle(command, _cancellationToken);

    Assert.Null(result.AuthenticationLinkSentTo);
    Assert.False(result.IsPasswordRequired);
    Assert.Null(result.OneTimePasswordValidation);
    Assert.Equal(createdToken.Token, result.ProfileCompletionToken);
    Assert.Null(result.Session);
  }

  [Theory(DisplayName = "It should enforce Multi-Factor Authentication.")]
  [InlineData(ContactType.Email)]
  [InlineData(ContactType.Phone)]
  public async Task It_should_enforce_Multi_Factor_Authentication(ContactType contactType)
  {
    User user = new(_faker.Person.Email)
    {
      HasPassword = true,
      Email = new(_faker.Person.Email),
      Phone = new(countryCode: null, _faker.Person.Phone, extension: null, _faker.Person.Phone)
    };
    user.CustomAttributes.Add(new("MultiFactorAuthenticationMode", contactType.ToString()));
    user.CustomAttributes.Add(new("ProfileCompletedOn", DateTime.Now.ToISOString()));
    _userService.Setup(x => x.FindAsync(user.UniqueName, _cancellationToken)).ReturnsAsync(user);
    _userService.Setup(x => x.AuthenticateAsync(user, PasswordString, _cancellationToken)).ReturnsAsync(user);

    OneTimePassword oneTimePassword = new()
    {
      Id = Guid.NewGuid(),
      Password = "123456"
    };
    _oneTimePasswordService.Setup(x => x.CreateAsync(user, Purposes.MultiFactorAuthentication, _cancellationToken)).ReturnsAsync(oneTimePassword);

    SentMessages sentMessages = new([Guid.NewGuid()]);
    SentMessage? sentMessage = null;
    switch (contactType)
    {
      case ContactType.Email:
        _messageService.Setup(x => x.SendAsync("MultiFactorAuthenticationEmail", user, ContactType.Email, _locale, It.Is<IReadOnlyDictionary<string, string>>(y => y.Single().Key == Variables.OneTimePassword && y.Single().Value == oneTimePassword.Password), _cancellationToken))
          .ReturnsAsync(sentMessages);
        sentMessage = sentMessages.ToSentMessage(user.Email);
        break;
      case ContactType.Phone:
        _messageService.Setup(x => x.SendAsync("MultiFactorAuthenticationPhone", user, ContactType.Phone, _locale, It.Is<IReadOnlyDictionary<string, string>>(y => y.Single().Key == Variables.OneTimePassword && y.Single().Value == oneTimePassword.Password), _cancellationToken))
          .ReturnsAsync(sentMessages);
        sentMessage = sentMessages.ToSentMessage(user.Phone);
        break;
    }

    SignInPayload payload = new(_locale.Code)
    {
      Credentials = new(user.UniqueName, PasswordString)
    };
    SignInCommand command = new(payload, CustomAttributes: []);
    SignInCommandResult result = await _handler.Handle(command, _cancellationToken);

    Assert.Null(result.AuthenticationLinkSentTo);
    Assert.False(result.IsPasswordRequired);
    Assert.NotNull(result.OneTimePasswordValidation);
    Assert.Equal(oneTimePassword.Id, result.OneTimePasswordValidation.Id);
    Assert.Equal(sentMessage, result.OneTimePasswordValidation.SentMessage);
    Assert.Null(result.ProfileCompletionToken);
    Assert.Null(result.Session);
  }

  [Fact(DisplayName = "It should require the password when it is missing.")]
  public async Task It_should_require_the_password_when_it_is_missing()
  {
    User user = new(_faker.Person.Email)
    {
      HasPassword = true
    };
    _userService.Setup(x => x.FindAsync(user.UniqueName, _cancellationToken)).ReturnsAsync(user);

    SignInPayload payload = new(_locale.Code)
    {
      Credentials = new(user.UniqueName)
    };
    SignInCommand command = new(payload, CustomAttributes: []);
    SignInCommandResult result = await _handler.Handle(command, _cancellationToken);

    Assert.Null(result.AuthenticationLinkSentTo);
    Assert.True(result.IsPasswordRequired);
    Assert.Null(result.OneTimePasswordValidation);
    Assert.Null(result.ProfileCompletionToken);
    Assert.Null(result.Session);
  }

  [Fact(DisplayName = "It should require the user to complete its profile (AuthenticationToken).")]
  public async Task It_should_require_the_user_to_complete_its_profile_AuthenticationToken()
  {
    SignInPayload payload = new(_locale.Code)
    {
      AuthenticationToken = "AuthenticationToken"
    };

    User user = new(_faker.Person.Email)
    {
      Id = Guid.NewGuid(),
      Email = new(_faker.Person.Email)
      {
        IsVerified = true
      }
    };
    _userService.Setup(x => x.FindAsync(user.Id, _cancellationToken)).ReturnsAsync(user);

    ValidatedToken validatedToken = new()
    {
      Subject = user.GetSubject(),
      Email = user.Email
    };
    _tokenService.Setup(x => x.ValidateAsync(payload.AuthenticationToken, TokenTypes.Authentication, _cancellationToken)).ReturnsAsync(validatedToken);

    CreatedToken createdToken = new("ProfileToken");
    _tokenService.Setup(x => x.CreateAsync(user, TokenTypes.Profile, _cancellationToken)).ReturnsAsync(createdToken);

    SignInCommand command = new(payload, CustomAttributes: []);
    SignInCommandResult result = await _handler.Handle(command, _cancellationToken);

    Assert.Null(result.AuthenticationLinkSentTo);
    Assert.False(result.IsPasswordRequired);
    Assert.Null(result.OneTimePasswordValidation);
    Assert.Equal(createdToken.Token, result.ProfileCompletionToken);
    Assert.Null(result.Session);

    _userService.Verify(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<EmailPayload>(), It.IsAny<CancellationToken>()), Times.Never);
  }

  [Fact(DisplayName = "It should require the user to complete its profile (Credentials).")]
  public async Task It_should_require_the_user_to_complete_its_profile_Credentials()
  {
    User user = new(_faker.Person.Email)
    {
      HasPassword = true
    };
    _userService.Setup(x => x.FindAsync(user.UniqueName, _cancellationToken)).ReturnsAsync(user);
    _userService.Setup(x => x.AuthenticateAsync(user, PasswordString, _cancellationToken)).ReturnsAsync(user);

    CreatedToken createdToken = new("ProfileToken");
    _tokenService.Setup(x => x.CreateAsync(user, TokenTypes.Profile, _cancellationToken)).ReturnsAsync(createdToken);

    SignInPayload payload = new(_locale.Code)
    {
      Credentials = new(user.UniqueName, PasswordString)
    };
    SignInCommand command = new(payload, CustomAttributes: []);
    SignInCommandResult result = await _handler.Handle(command, _cancellationToken);

    Assert.Null(result.AuthenticationLinkSentTo);
    Assert.False(result.IsPasswordRequired);
    Assert.Null(result.OneTimePasswordValidation);
    Assert.Equal(createdToken.Token, result.ProfileCompletionToken);
    Assert.Null(result.Session);
  }

  [Fact(DisplayName = "It should require the user to complete its profile (GoogleIdToken).")]
  public async Task It_should_require_the_user_to_complete_its_profile_GoogleIdToken()
  {
    SignInPayload payload = new(_locale.Code)
    {
      GoogleIdToken = "GoogleIdToken"
    };

    EmailPayload email = new(_faker.Person.Email, isVerified: true);
    GoogleIdentity identity = new("GoogleUserId", email, FirstName: null, LastName: null, Locale: null, Picture: null);
    _googleService.Setup(x => x.GetIdentityAsync(payload.GoogleIdToken, _cancellationToken)).ReturnsAsync(identity);

    User user = new(_faker.Person.UserName);
    user.CustomIdentifiers.Add(new("Google", identity.Id));
    _userService.Setup(x => x.FindAsync(user.CustomIdentifiers.Single(), _cancellationToken)).ReturnsAsync(user);

    CreatedToken profileCompletion = new("ProfileCompletionToken");
    _tokenService.Setup(x => x.CreateAsync(user, TokenTypes.Profile, _cancellationToken)).ReturnsAsync(profileCompletion);

    SignInCommand command = new(payload, CustomAttributes: []);
    SignInCommandResult result = await _handler.Handle(command, _cancellationToken);

    Assert.Null(result.AuthenticationLinkSentTo);
    Assert.False(result.IsPasswordRequired);
    Assert.Null(result.OneTimePasswordValidation);
    Assert.Equal(profileCompletion.Token, result.ProfileCompletionToken);
    Assert.Null(result.Session);
  }

  [Fact(DisplayName = "It should require the user to complete its profile (OTP).")]
  public async Task It_should_require_the_user_to_complete_its_profile_Otp()
  {
    User user = new(_faker.Person.Email)
    {
      Id = Guid.NewGuid()
    };
    _userService.Setup(x => x.FindAsync(user.Id, _cancellationToken)).ReturnsAsync(user);

    OneTimePassword oneTimePassword = new()
    {
      Id = Guid.NewGuid()
    };
    oneTimePassword.CustomAttributes.Add(new("UserId", user.Id.ToString()));

    SignInPayload payload = new(_locale.Code)
    {
      OneTimePassword = new(oneTimePassword.Id, "123456")
    };
    _oneTimePasswordService.Setup(x => x.ValidateAsync(payload.OneTimePassword, Purposes.MultiFactorAuthentication, _cancellationToken)).ReturnsAsync(oneTimePassword);

    CreatedToken createdToken = new("ProfileToken");
    _tokenService.Setup(x => x.CreateAsync(user, TokenTypes.Profile, _cancellationToken)).ReturnsAsync(createdToken);

    SignInCommand command = new(payload, CustomAttributes: []);
    SignInCommandResult result = await _handler.Handle(command, _cancellationToken);

    Assert.Null(result.AuthenticationLinkSentTo);
    Assert.False(result.IsPasswordRequired);
    Assert.Null(result.OneTimePasswordValidation);
    Assert.Equal(createdToken.Token, result.ProfileCompletionToken);
    Assert.Null(result.Session);
  }

  [Fact(DisplayName = "It should send an authentication email when the user does not exist.")]
  public async Task It_should_send_an_authentication_email_when_the_user_does_not_exist()
  {
    Email email = new(_faker.Person.Email);
    CreatedToken createdToken = new("AuthenticationToken");
    _tokenService.Setup(x => x.CreateAsync(null, email, TokenTypes.Authentication, _cancellationToken)).ReturnsAsync(createdToken);

    SentMessages sentMessages = new([Guid.NewGuid()]);
    _messageService.Setup(x => x.SendAsync(Templates.AccountAuthentication, email, _locale, It.Is<IReadOnlyDictionary<string, string>>(y => y.Single().Key == Variables.Token && y.Single().Value == createdToken.Token), _cancellationToken))
      .ReturnsAsync(sentMessages);

    SignInPayload payload = new(_locale.Code)
    {
      Credentials = new(email.Address)
    };
    SignInCommand command = new(payload, CustomAttributes: []);
    SignInCommandResult result = await _handler.Handle(command, _cancellationToken);

    Assert.Equal(sentMessages.ToSentMessage(email), result.AuthenticationLinkSentTo);
    Assert.False(result.IsPasswordRequired);
    Assert.Null(result.OneTimePasswordValidation);
    Assert.Null(result.ProfileCompletionToken);
    Assert.Null(result.Session);

    _userService.Verify(x => x.FindAsync(payload.Credentials.EmailAddress, _cancellationToken), Times.Once());
  }

  [Fact(DisplayName = "It should send an authentication email when the user has no password.")]
  public async Task It_should_send_an_authentication_email_when_the_user_has_no_password()
  {
    User user = new(_faker.Person.Email)
    {
      Email = new(_faker.Person.Email)
    };
    _userService.Setup(x => x.FindAsync(user.UniqueName, _cancellationToken)).ReturnsAsync(user);

    CreatedToken createdToken = new("AuthenticationToken");
    _tokenService.Setup(x => x.CreateAsync(user, user.Email, TokenTypes.Authentication, _cancellationToken)).ReturnsAsync(createdToken);

    SentMessages sentMessages = new([Guid.NewGuid()]);
    _messageService.Setup(x => x.SendAsync(Templates.AccountAuthentication, user, ContactType.Email, _locale, It.Is<IReadOnlyDictionary<string, string>>(y => y.Single().Key == Variables.Token && y.Single().Value == createdToken.Token), _cancellationToken))
      .ReturnsAsync(sentMessages);

    SignInPayload payload = new(_locale.Code)
    {
      Credentials = new(user.UniqueName)
    };
    SignInCommand command = new(payload, CustomAttributes: []);
    SignInCommandResult result = await _handler.Handle(command, _cancellationToken);

    Assert.Equal(sentMessages.ToSentMessage(user.Email), result.AuthenticationLinkSentTo);
    Assert.False(result.IsPasswordRequired);
    Assert.Null(result.OneTimePasswordValidation);
    Assert.Null(result.ProfileCompletionToken);
    Assert.Null(result.Session);
  }

  [Fact(DisplayName = "It should sign-in the user when it has no MFA and completed its profile.")]
  public async Task It_should_sign_in_the_user_when_it_has_no_Mfa_and_completed_its_profile()
  {
    User user = new(_faker.Person.Email)
    {
      HasPassword = true
    };
    user.CustomAttributes.Add(new("MultiFactorAuthenticationMode", MultiFactorAuthenticationMode.None.ToString()));
    user.CustomAttributes.Add(new("ProfileCompletedOn", DateTime.Now.ToISOString()));
    _userService.Setup(x => x.FindAsync(user.UniqueName, _cancellationToken)).ReturnsAsync(user);

    CustomAttribute[] customAttributes =
    [
      new("AdditionalInformation", $@"{{""User-Agent"":""{_faker.Internet.UserAgent()}""}}"),
      new("IpAddress", _faker.Internet.Ip())
    ];
    Session session = new(user);
    _sessionService.Setup(x => x.SignInAsync(user, PasswordString, customAttributes, _cancellationToken)).ReturnsAsync(session);

    SignInPayload payload = new(_locale.Code)
    {
      Credentials = new(user.UniqueName, PasswordString)
    };
    SignInCommand command = new(payload, customAttributes);
    SignInCommandResult result = await _handler.Handle(command, _cancellationToken);

    Assert.Null(result.AuthenticationLinkSentTo);
    Assert.False(result.IsPasswordRequired);
    Assert.Null(result.OneTimePasswordValidation);
    Assert.Null(result.ProfileCompletionToken);
    Assert.Same(session, result.Session);

    _actorService.Verify(x => x.SaveAsync(user, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should sign-in the user (AuthenticationToken).")]
  public async Task It_should_sign_in_the_user_AuthenticationToken()
  {
    SignInPayload payload = new(_locale.Code)
    {
      AuthenticationToken = "AuthenticationToken"
    };

    User user = new(_faker.Person.Email)
    {
      Id = Guid.NewGuid(),
      Email = new(_faker.Person.Email)
    };
    user.CustomAttributes.Add(new("MultiFactorAuthenticationMode", MultiFactorAuthenticationMode.None.ToString()));
    user.CustomAttributes.Add(new("ProfileCompletedOn", DateTime.Now.ToISOString()));
    _userService.Setup(x => x.FindAsync(user.Id, _cancellationToken)).ReturnsAsync(user);
    _userService.Setup(x => x.UpdateAsync(user, It.Is<EmailPayload>(e => e.Address == user.Email.Address && e.IsVerified), _cancellationToken)).ReturnsAsync(user);

    ValidatedToken validatedToken = new()
    {
      Subject = user.GetSubject(),
      Email = user.Email
    };
    _tokenService.Setup(x => x.ValidateAsync(payload.AuthenticationToken, TokenTypes.Authentication, _cancellationToken)).ReturnsAsync(validatedToken);

    CustomAttribute[] customAttributes =
    [
      new("AdditionalInformation", $@"{{""User-Agent"":""{_faker.Internet.UserAgent()}""}}"),
      new("IpAddress", _faker.Internet.Ip())
    ];
    Session session = new(user);
    _sessionService.Setup(x => x.CreateAsync(user, customAttributes, _cancellationToken)).ReturnsAsync(session);

    SignInCommand command = new(payload, customAttributes);
    SignInCommandResult result = await _handler.Handle(command, _cancellationToken);

    Assert.Null(result.AuthenticationLinkSentTo);
    Assert.False(result.IsPasswordRequired);
    Assert.Null(result.OneTimePasswordValidation);
    Assert.Null(result.ProfileCompletionToken);
    Assert.Same(session, result.Session);

    _actorService.Verify(x => x.SaveAsync(user, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should sign-in the user (GoogleIdToken).")]
  public async Task It_should_sign_in_the_user_GoogleIdToken()
  {
    SignInPayload payload = new(_locale.Code)
    {
      GoogleIdToken = "GoogleIdToken"
    };

    EmailPayload email = new(_faker.Person.Email, isVerified: true);
    GoogleIdentity identity = new("GoogleUserId", email, FirstName: null, LastName: null, Locale: null, Picture: null);
    _googleService.Setup(x => x.GetIdentityAsync(payload.GoogleIdToken, _cancellationToken)).ReturnsAsync(identity);

    User user = new(_faker.Person.UserName);
    user.CustomAttributes.Add(new("ProfileCompletedOn", DateTime.Now.ToISOString()));
    user.CustomIdentifiers.Add(new("Google", identity.Id));
    _userService.Setup(x => x.FindAsync(user.CustomIdentifiers.Single(), _cancellationToken)).ReturnsAsync(user);

    CustomAttribute[] customAttributes =
    [
      new("AdditionalInformation", $@"{{""User-Agent"":""{_faker.Internet.UserAgent()}""}}"),
      new("IpAddress", _faker.Internet.Ip())
    ];
    Session session = new(user);
    _sessionService.Setup(x => x.CreateAsync(user, customAttributes, _cancellationToken)).ReturnsAsync(session);

    SignInCommand command = new(payload, customAttributes);
    SignInCommandResult result = await _handler.Handle(command, _cancellationToken);

    Assert.Null(result.AuthenticationLinkSentTo);
    Assert.False(result.IsPasswordRequired);
    Assert.Null(result.OneTimePasswordValidation);
    Assert.Null(result.ProfileCompletionToken);
    Assert.Same(session, result.Session);
  }

  [Fact(DisplayName = "It should sign-in the user (OTP).")]
  public async Task It_should_sign_in_the_user_Otp()
  {
    User user = new(_faker.Person.Email)
    {
      Id = Guid.NewGuid()
    };
    user.CustomAttributes.Add(new("MultiFactorAuthenticationMode", MultiFactorAuthenticationMode.None.ToString()));
    user.CustomAttributes.Add(new("ProfileCompletedOn", DateTime.Now.ToISOString()));
    _userService.Setup(x => x.FindAsync(user.Id, _cancellationToken)).ReturnsAsync(user);

    OneTimePassword oneTimePassword = new()
    {
      Id = Guid.NewGuid()
    };
    oneTimePassword.CustomAttributes.Add(new("UserId", user.Id.ToString()));

    SignInPayload payload = new(_locale.Code)
    {
      OneTimePassword = new(oneTimePassword.Id, "123456")
    };
    _oneTimePasswordService.Setup(x => x.ValidateAsync(payload.OneTimePassword, Purposes.MultiFactorAuthentication, _cancellationToken)).ReturnsAsync(oneTimePassword);

    CustomAttribute[] customAttributes =
    [
      new("AdditionalInformation", $@"{{""User-Agent"":""{_faker.Internet.UserAgent()}""}}"),
      new("IpAddress", _faker.Internet.Ip())
    ];
    Session session = new(user);
    _sessionService.Setup(x => x.CreateAsync(user, customAttributes, _cancellationToken)).ReturnsAsync(session);

    SignInCommand command = new(payload, customAttributes);
    SignInCommandResult result = await _handler.Handle(command, _cancellationToken);

    Assert.Null(result.AuthenticationLinkSentTo);
    Assert.False(result.IsPasswordRequired);
    Assert.Null(result.OneTimePasswordValidation);
    Assert.Null(result.ProfileCompletionToken);
    Assert.Same(session, result.Session);

    _actorService.Verify(x => x.SaveAsync(user, _cancellationToken), Times.Once);
  }

  [Theory(DisplayName = "It should throw ArgumentException when sending MFA message and no contact.")]
  [InlineData(ContactType.Email)]
  [InlineData(ContactType.Phone)]
  public async Task It_should_throw_ArgumentException_when_sending_Mfa_message_and_no_contact(ContactType contactType)
  {
    User user = new(_faker.Person.Email)
    {
      Id = Guid.NewGuid(),
      HasPassword = true
    };
    user.CustomAttributes.Add(new("MultiFactorAuthenticationMode", contactType.ToString()));
    user.CustomAttributes.Add(new("ProfileCompletedOn", DateTime.Now.ToISOString()));
    _userService.Setup(x => x.FindAsync(user.UniqueName, _cancellationToken)).ReturnsAsync(user);
    _userService.Setup(x => x.AuthenticateAsync(user, PasswordString, _cancellationToken)).ReturnsAsync(user);

    SignInPayload payload = new(_locale.Code)
    {
      Credentials = new(user.UniqueName, PasswordString)
    };
    SignInCommand command = new(payload, CustomAttributes: []);
    var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await _handler.Handle(command, _cancellationToken));

    Assert.StartsWith($"The user 'Id={user.Id}' has no {contactType.ToString().ToLowerInvariant()}.", exception.Message);
    Assert.Equal("user", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the subject claim is missing.")]
  public async Task It_should_throw_ArgumentException_when_the_subject_claim_is_missing()
  {
    SignInPayload payload = new(_locale.Code)
    {
      Profile = new CompleteProfilePayload("ProfileToken", _faker.Person.FirstName, _faker.Person.LastName, _locale.Code, TimeZoneId)
    };

    ValidatedToken validatedToken = new();
    _tokenService.Setup(x => x.ValidateAsync(payload.Profile.Token, TokenTypes.Profile, _cancellationToken)).ReturnsAsync(validatedToken);

    SignInCommand command = new(payload, CustomAttributes: []);
    var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.StartsWith("The 'Subject' claim is required.", exception.Message);
    Assert.Equal("payload", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the user could not be found from OTP UserId.")]
  public async Task It_should_throw_ArgumentException_when_the_user_could_not_be_found_from_Otp_UserId()
  {
    OneTimePassword oneTimePassword = new()
    {
      Id = Guid.NewGuid()
    };
    string userId = Guid.NewGuid().ToString();
    oneTimePassword.CustomAttributes.Add(new("UserId", userId));

    SignInPayload payload = new(_locale.Code)
    {
      OneTimePassword = new(oneTimePassword.Id, "123456")
    };
    _oneTimePasswordService.Setup(x => x.ValidateAsync(payload.OneTimePassword, Purposes.MultiFactorAuthentication, _cancellationToken)).ReturnsAsync(oneTimePassword);

    SignInCommand command = new(payload, CustomAttributes: []);
    var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.StartsWith($"The user 'Id={userId}' could not be found.", exception.Message);
    Assert.Equal("payload", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the user could not be found from authentication token subject.")]
  public async Task It_should_throw_ArgumentException_when_the_user_could_not_be_found_from_authentication_token_subject()
  {
    SignInPayload payload = new(_locale.Code)
    {
      AuthenticationToken = "AuthenticationToken"
    };

    ValidatedToken validatedToken = new()
    {
      Subject = Guid.Empty.ToString(),
      Email = new Email(_faker.Person.Email)
    };
    _tokenService.Setup(x => x.ValidateAsync(payload.AuthenticationToken, TokenTypes.Authentication, _cancellationToken)).ReturnsAsync(validatedToken);

    SignInCommand command = new(payload, CustomAttributes: []);
    var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.StartsWith($"The user 'Id={validatedToken.Subject}' could not be found.", exception.Message);
    Assert.Equal("authenticationToken", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the user could not be found from profile token subject.")]
  public async Task It_should_throw_ArgumentException_when_the_user_could_not_be_found_from_profile_token_subject()
  {
    SignInPayload payload = new(_locale.Code)
    {
      Profile = new CompleteProfilePayload("ProfileToken", _faker.Person.FirstName, _faker.Person.LastName, _locale.Code, TimeZoneId)
    };

    ValidatedToken validatedToken = new()
    {
      Subject = Guid.Empty.ToString()
    };
    _tokenService.Setup(x => x.ValidateAsync(payload.Profile.Token, TokenTypes.Profile, _cancellationToken)).ReturnsAsync(validatedToken);

    SignInCommand command = new(payload, CustomAttributes: []);
    var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.StartsWith($"The user 'Id={validatedToken.Subject}' could not be found.", exception.Message);
    Assert.Equal("payload", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw InvalidOperationException when the created OTP has no password.")]
  public async Task It_should_throw_InvalidOperationException_when_the_created_Otp_has_no_password()
  {
    User user = new(_faker.Person.Email)
    {
      HasPassword = true,
      Email = new(_faker.Person.Email)
    };
    user.CustomAttributes.Add(new("MultiFactorAuthenticationMode", "Email"));
    user.CustomAttributes.Add(new("ProfileCompletedOn", DateTime.Now.ToISOString()));
    _userService.Setup(x => x.FindAsync(user.UniqueName, _cancellationToken)).ReturnsAsync(user);
    _userService.Setup(x => x.AuthenticateAsync(user, PasswordString, _cancellationToken)).ReturnsAsync(user);

    OneTimePassword oneTimePassword = new()
    {
      Id = Guid.NewGuid()
    };
    _oneTimePasswordService.Setup(x => x.CreateAsync(user, Purposes.MultiFactorAuthentication, _cancellationToken)).ReturnsAsync(oneTimePassword);

    SignInPayload payload = new(_locale.Code)
    {
      Credentials = new(user.UniqueName, PasswordString)
    };
    SignInCommand command = new(payload, CustomAttributes: []);
    var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal($"The One-Time Password (OTP) 'Id={oneTimePassword.Id}' has no password.", exception.Message);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_it_not_valid()
  {
    SignInPayload payload = new(_locale.Code)
    {
      Credentials = new(_faker.Person.Email),
      AuthenticationToken = "AuthenticationToken"
    };
    SignInCommand command = new(payload, CustomAttributes: []);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("SignInValidator", error.ErrorCode);
  }

  [Fact(DisplayName = "It should update an user email when it has changed.")]
  public async Task It_should_update_an_user_email_when_it_has_changed()
  {
    SignInPayload payload = new(_locale.Code)
    {
      AuthenticationToken = "AuthenticationToken"
    };

    User user = new(_faker.Person.Email)
    {
      Id = Guid.NewGuid(),
      Email = new(_faker.Person.Email)
      {
        IsVerified = false
      }
    };
    EmailPayload email = new(user.Email.Address, isVerified: true);
    _userService.Setup(x => x.FindAsync(user.Id, _cancellationToken)).ReturnsAsync(user);
    _userService.Setup(x => x.UpdateAsync(user, email, _cancellationToken)).ReturnsAsync(user);

    ValidatedToken validatedToken = new()
    {
      Subject = user.GetSubject(),
      Email = new Email(user.Email.Address)
      {
        IsVerified = false
      }
    };
    _tokenService.Setup(x => x.ValidateAsync(payload.AuthenticationToken, TokenTypes.Authentication, _cancellationToken)).ReturnsAsync(validatedToken);

    CreatedToken createdToken = new("ProfileToken");
    _tokenService.Setup(x => x.CreateAsync(user, TokenTypes.Profile, _cancellationToken)).ReturnsAsync(createdToken);

    SignInCommand command = new(payload, CustomAttributes: []);
    SignInCommandResult result = await _handler.Handle(command, _cancellationToken);

    Assert.Null(result.AuthenticationLinkSentTo);
    Assert.False(result.IsPasswordRequired);
    Assert.Null(result.OneTimePasswordValidation);
    Assert.Equal(createdToken.Token, result.ProfileCompletionToken);
    Assert.Null(result.Session);

    _userService.Verify(x => x.UpdateAsync(user, email, _cancellationToken), Times.Once);
  }
}
