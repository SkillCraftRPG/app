using Bogus;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using Moq;
using SkillCraft.Application.Accounts.Constants;
using SkillCraft.Contracts.Accounts;
using System.Text.Json;

namespace SkillCraft.Application.Accounts.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class VerifyPhoneCommandHandlerTests
{
  private const string PhoneNumber = "(514) 845-4636";

  private static readonly LocaleUnit _locale = new("fr");

  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IMessageService> _messageService = new();
  private readonly Mock<IOneTimePasswordService> _oneTimePasswordService = new();
  private readonly Mock<ITokenService> _tokenService = new();
  private readonly Mock<IUserService> _userService = new();

  private readonly VerifyPhoneCommandHandler _handler;

  public VerifyPhoneCommandHandlerTests()
  {
    _handler = new(_messageService.Object, _oneTimePasswordService.Object, _tokenService.Object, _userService.Object);
  }

  [Fact(DisplayName = "It should handle a One-Time Password.")]
  public async Task It_should_handle_a_One_Time_Password()
  {
    VerifyPhonePayload payload = new(_locale.Code, "ProfileCompletionToken", new OneTimePasswordPayload());
    Assert.NotNull(payload.OneTimePassword);

    User user = new(_faker.Person.UserName)
    {
      Id = Guid.NewGuid(),
      Phone = new Phone("CA", "(514) 845-4636", extension: null, "+15148454636")
      {
        IsVerified = true
      }
    };
    _userService.Setup(x => x.FindAsync(user.Id, _cancellationToken)).ReturnsAsync(user);

    ValidatedToken validatedToken = new()
    {
      Subject = user.GetSubject()
    };
    _tokenService.Setup(x => x.ValidateAsync(payload.ProfileCompletionToken, false, TokenTypes.Profile, _cancellationToken)).ReturnsAsync(validatedToken);

    OneTimePassword oneTimePassword = new()
    {
      Id = Guid.NewGuid()
    };
    oneTimePassword.CustomAttributes.Add(new("Phone", JsonSerializer.Serialize(user.Phone)));
    oneTimePassword.CustomAttributes.Add(new("UserId", user.Id.ToString()));
    _oneTimePasswordService.Setup(x => x.ValidateAsync(payload.OneTimePassword, Purposes.ContactVerification, _cancellationToken)).ReturnsAsync(oneTimePassword);

    CreatedToken profileCompletion = new("ProfileCompletionTokenWithPhone");
    _tokenService.Setup(x => x.CreateAsync(user, user.Phone, TokenTypes.Profile, _cancellationToken)).ReturnsAsync(profileCompletion);

    VerifyPhoneCommand command = new(payload);
    VerifyPhoneResult result = await _handler.Handle(command, _cancellationToken);

    Assert.Null(result.OneTimePasswordValidation);
    Assert.Equal(profileCompletion.Token, result.ProfileCompletionToken);

    _tokenService.Verify(x => x.ValidateAsync(payload.ProfileCompletionToken, true, TokenTypes.Profile, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should handle a phone.")]
  public async Task It_should_handle_a_phone()
  {
    AccountPhone accountPhone = new("CA", PhoneNumber);
    Phone phone = accountPhone.ToPhone();
    VerifyPhonePayload payload = new(_locale.Code, "ProfileCompletionToken", accountPhone);

    User user = new(_faker.Person.UserName)
    {
      Id = Guid.NewGuid()
    };
    _userService.Setup(x => x.FindAsync(user.Id, _cancellationToken)).ReturnsAsync(user);

    ValidatedToken validatedToken = new()
    {
      Subject = user.GetSubject()
    };
    _tokenService.Setup(x => x.ValidateAsync(payload.ProfileCompletionToken, false, TokenTypes.Profile, _cancellationToken)).ReturnsAsync(validatedToken);

    OneTimePassword oneTimePassword = new()
    {
      Id = Guid.NewGuid(),
      Password = "123456"
    };
    _oneTimePasswordService.Setup(x => x.CreateAsync(user, phone, Purposes.ContactVerification, _cancellationToken)).ReturnsAsync(oneTimePassword);

    SentMessages sentMessages = new([Guid.NewGuid()]);
    SentMessage sentMessage = sentMessages.ToSentMessage(phone);
    _messageService.Setup(x => x.SendAsync("ContactVerificationPhone", phone, _locale, It.Is<IReadOnlyDictionary<string, string>>(v => v.Single().Key == Variables.OneTimePassword && v.Single().Value == oneTimePassword.Password), _cancellationToken))
      .ReturnsAsync(sentMessages);

    VerifyPhoneCommand command = new(payload);
    VerifyPhoneResult result = await _handler.Handle(command, _cancellationToken);

    Assert.NotNull(result.OneTimePasswordValidation);
    Assert.Equal(oneTimePassword.Id, result.OneTimePasswordValidation.Id);
    Assert.Equal(sentMessage, result.OneTimePasswordValidation.SentMessage);
    Assert.Null(result.ProfileCompletionToken);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the user cannot be found.")]
  public async Task It_should_throw_ArgumentException_when_the_user_cannot_be_found()
  {
    VerifyPhonePayload payload = new(_locale.Code, "ProfileCompletionToken", new AccountPhone());
    Guid userId = Guid.NewGuid();
    ValidatedToken validatedToken = new()
    {
      Subject = userId.ToString()
    };
    _tokenService.Setup(x => x.ValidateAsync(payload.ProfileCompletionToken, false, TokenTypes.Profile, _cancellationToken)).ReturnsAsync(validatedToken);

    VerifyPhoneCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.StartsWith($"The user 'Id={userId}' could not be found.", exception.Message);
    Assert.Equal("profileCompletionToken", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw InvalidOneTimePasswordUserException when the token and OTP user do not match.")]
  public async Task It_should_throw_InvalidOneTimePasswordUserException_when_the_token_and_Otp_user_do_not_match()
  {
    VerifyPhonePayload payload = new(_locale.Code, "ProfileCompletionToken", new OneTimePasswordPayload());
    Assert.NotNull(payload.OneTimePassword);

    User user = new(_faker.Person.UserName)
    {
      Id = Guid.NewGuid()
    };
    _userService.Setup(x => x.FindAsync(user.Id, _cancellationToken)).ReturnsAsync(user);

    ValidatedToken validatedToken = new()
    {
      Subject = user.GetSubject()
    };
    _tokenService.Setup(x => x.ValidateAsync(payload.ProfileCompletionToken, false, TokenTypes.Profile, _cancellationToken)).ReturnsAsync(validatedToken);

    OneTimePassword oneTimePassword = new()
    {
      Id = Guid.NewGuid()
    };
    Guid otherUserId = Guid.NewGuid();
    oneTimePassword.CustomAttributes.Add(new("UserId", otherUserId.ToString()));
    _oneTimePasswordService.Setup(x => x.ValidateAsync(payload.OneTimePassword, Purposes.ContactVerification, _cancellationToken)).ReturnsAsync(oneTimePassword);

    VerifyPhoneCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<InvalidOneTimePasswordUserException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(oneTimePassword.Id, exception.OneTimePasswordId);
    Assert.Equal(user.Id, exception.ActualUserId);
    Assert.Equal(otherUserId, exception.ExpectedUserId);
  }

  [Fact(DisplayName = "It should throw InvalidOperationException when the OTP has no password.")]
  public async Task It_should_throw_InvalidOperationException_when_the_Otp_has_no_password()
  {
    AccountPhone accountPhone = new("CA", PhoneNumber);
    Phone phone = accountPhone.ToPhone();
    VerifyPhonePayload payload = new(_locale.Code, "ProfileCompletionToken", accountPhone);

    User user = new(_faker.Person.UserName)
    {
      Id = Guid.NewGuid()
    };
    _userService.Setup(x => x.FindAsync(user.Id, _cancellationToken)).ReturnsAsync(user);

    ValidatedToken validatedToken = new()
    {
      Subject = user.GetSubject()
    };
    _tokenService.Setup(x => x.ValidateAsync(payload.ProfileCompletionToken, false, TokenTypes.Profile, _cancellationToken)).ReturnsAsync(validatedToken);

    OneTimePassword oneTimePassword = new()
    {
      Id = Guid.NewGuid()
    };
    _oneTimePasswordService.Setup(x => x.CreateAsync(user, phone, Purposes.ContactVerification, _cancellationToken)).ReturnsAsync(oneTimePassword);

    VerifyPhoneCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal($"The One-Time Password (OTP) 'Id={oneTimePassword.Id}' has no password.", exception.Message);
  }
}
