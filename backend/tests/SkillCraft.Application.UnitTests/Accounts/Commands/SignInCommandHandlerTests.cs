using Bogus;
using FluentValidation.Results;
using Logitar;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using Moq;
using SkillCraft.Application.Accounts.Constants;
using SkillCraft.Application.Actors;
using SkillCraft.Contracts.Accounts;
using SkillCraft.Domain;

namespace SkillCraft.Application.Accounts.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SignInCommandHandlerTests
{
  private const string PasswordString = "P@s$W0rD";

  private static readonly LocaleUnit _locale = new("fr");

  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IActorService> _actorService = new();
  private readonly Mock<IMessageService> _messageService = new();
  private readonly Mock<IOneTimePasswordService> _oneTimePasswordService = new();
  private readonly Mock<ISessionService> _sessionService = new();
  private readonly Mock<ITokenService> _tokenService = new();
  private readonly Mock<IUserService> _userService = new();

  private readonly SignInCommandHandler _handler;

  public SignInCommandHandlerTests()
  {
    _handler = new(_actorService.Object, _messageService.Object, _oneTimePasswordService.Object, _sessionService.Object, _tokenService.Object, _userService.Object);
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
    _userService.Setup(x => x.UpdateAsync(user, It.Is<EmailPayload>(e => e.Address == user.Email.Address && e.IsVerified), _cancellationToken)).ReturnsAsync(user);

    ValidatedToken validatedToken = new()
    {
      Subject = user.Id.ToString(),
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
      Subject = user.Id.ToString(),
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

  [Fact(DisplayName = "It should throw ArgumentException when the email claims are missing.")]
  public async Task It_should_throw_ArgumentException_when_the_email_claims_are_missing()
  {
    SignInPayload payload = new(_locale.Code)
    {
      AuthenticationToken = "AuthenticationToken"
    };

    ValidatedToken validatedToken = new();
    _tokenService.Setup(x => x.ValidateAsync(payload.AuthenticationToken, TokenTypes.Authentication, _cancellationToken)).ReturnsAsync(validatedToken);

    SignInCommand command = new(payload, CustomAttributes: []);
    var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.StartsWith("The email claims are required.", exception.Message);
    Assert.Equal("authenticationToken", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the subject claim is not a valid Guid.")]
  public async Task It_should_throw_ArgumentException_when_the_subject_claim_is_not_a_valid_Guid()
  {
    SignInPayload payload = new(_locale.Code)
    {
      AuthenticationToken = "AuthenticationToken"
    };

    ValidatedToken validatedToken = new()
    {
      Subject = _faker.Person.Email,
      Email = new Email(_faker.Person.Email)
    };
    _tokenService.Setup(x => x.ValidateAsync(payload.AuthenticationToken, TokenTypes.Authentication, _cancellationToken)).ReturnsAsync(validatedToken);

    SignInCommand command = new(payload, CustomAttributes: []);
    var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.StartsWith($"The value '{validatedToken.Subject}' is not a valid Guid.", exception.Message);
    Assert.Equal("authenticationToken", exception.ParamName);
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

  [Fact(DisplayName = "It should throw ArgumentException when the user could not be found from token subject.")]
  public async Task It_should_throw_ArgumentException_when_the_user_could_not_be_found_from_token_subject()
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
}
