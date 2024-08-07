using Bogus;
using FluentValidation.Results;
using Logitar;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Users;
using Moq;
using SkillCraft.Application.Accounts.Constants;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Application.Accounts.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class ChangePhoneCommandHandlerTests
{
  private const string PhoneNumber = "(514) 845-4636";

  private static readonly LocaleUnit _locale = new("fr");

  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IMessageService> _messageService = new();
  private readonly Mock<IOneTimePasswordService> _oneTimePasswordService = new();
  private readonly Mock<IUserService> _userService = new();

  private readonly ChangePhoneCommandHandler _handler;

  public ChangePhoneCommandHandlerTests()
  {
    _handler = new(_messageService.Object, _oneTimePasswordService.Object, _userService.Object);
  }

  [Fact(DisplayName = "It should handle a One-Time Password.")]
  public async Task It_should_handle_a_One_Time_Password()
  {
    ChangePhonePayload payload = new(_locale.Code, new OneTimePasswordPayload(Guid.NewGuid(), "123456"));
    Assert.NotNull(payload.OneTimePassword);

    User user = new(_faker.Person.UserName)
    {
      Id = Guid.NewGuid(),
      Phone = new Phone("CA", "(514) 845-4636", extension: null, "+15148454636")
      {
        IsVerified = false
      }
    };
    user.CustomAttributes.Add(new("ProfileCompletedOn", DateTime.Now.ToISOString()));
    _userService.Setup(x => x.FindAsync(user.Id, _cancellationToken)).ReturnsAsync(user);

    OneTimePassword oneTimePassword = new()
    {
      Id = Guid.NewGuid()
    };
    oneTimePassword.CustomAttributes.Add(new("Phone", JsonSerializer.Serialize(user.Phone)));
    oneTimePassword.CustomAttributes.Add(new("UserId", user.Id.ToString()));
    _oneTimePasswordService.Setup(x => x.ValidateAsync(payload.OneTimePassword, Purposes.ContactVerification, _cancellationToken)).ReturnsAsync(oneTimePassword);

    PhonePayload phone = oneTimePassword.GetPhonePayload(isVerified: true);
    _userService.Setup(x => x.UpdateAsync(user, phone, _cancellationToken)).ReturnsAsync(user);

    ChangePhoneCommand command = new(payload);
    command.Contextualize(new ActivityContextMock(user));
    ChangePhoneResult result = await _handler.Handle(command, _cancellationToken);

    Assert.Null(result.OneTimePasswordValidation);
    Assert.Equal(user.ToUserProfile(), result.UserProfile);

    _userService.Verify(x => x.UpdateAsync(user, phone, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should handle a phone.")]
  public async Task It_should_handle_a_phone()
  {
    AccountPhone accountPhone = new("CA", PhoneNumber);
    Phone phone = accountPhone.ToPhone();
    ChangePhonePayload payload = new(_locale.Code, accountPhone);

    User user = new(_faker.Person.UserName)
    {
      Id = Guid.NewGuid()
    };
    _userService.Setup(x => x.FindAsync(user.Id, _cancellationToken)).ReturnsAsync(user);

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

    ChangePhoneCommand command = new(payload);
    command.Contextualize(new ActivityContextMock(user));
    ChangePhoneResult result = await _handler.Handle(command, _cancellationToken);

    Assert.NotNull(result.OneTimePasswordValidation);
    Assert.Equal(oneTimePassword.Id, result.OneTimePasswordValidation.Id);
    Assert.Equal(sentMessage, result.OneTimePasswordValidation.SentMessage);
    Assert.Null(result.UserProfile);
  }

  [Fact(DisplayName = "It should throw InvalidOneTimePasswordUserException when the token and OTP user do not match.")]
  public async Task It_should_throw_InvalidOneTimePasswordUserException_when_the_token_and_Otp_user_do_not_match()
  {
    ChangePhonePayload payload = new(_locale.Code, new OneTimePasswordPayload(Guid.NewGuid(), "123456"));
    Assert.NotNull(payload.OneTimePassword);

    User user = new(_faker.Person.UserName)
    {
      Id = Guid.NewGuid()
    };
    _userService.Setup(x => x.FindAsync(user.Id, _cancellationToken)).ReturnsAsync(user);

    OneTimePassword oneTimePassword = new()
    {
      Id = Guid.NewGuid()
    };
    Guid otherUserId = Guid.NewGuid();
    oneTimePassword.CustomAttributes.Add(new("UserId", otherUserId.ToString()));
    _oneTimePasswordService.Setup(x => x.ValidateAsync(payload.OneTimePassword, Purposes.ContactVerification, _cancellationToken)).ReturnsAsync(oneTimePassword);

    ChangePhoneCommand command = new(payload);
    command.Contextualize(new ActivityContextMock(user));
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
    ChangePhonePayload payload = new(_locale.Code, accountPhone);

    User user = new(_faker.Person.UserName)
    {
      Id = Guid.NewGuid()
    };
    _userService.Setup(x => x.FindAsync(user.Id, _cancellationToken)).ReturnsAsync(user);

    OneTimePassword oneTimePassword = new()
    {
      Id = Guid.NewGuid()
    };
    _oneTimePasswordService.Setup(x => x.CreateAsync(user, phone, Purposes.ContactVerification, _cancellationToken)).ReturnsAsync(oneTimePassword);

    ChangePhoneCommand command = new(payload);
    command.Contextualize(new ActivityContextMock(user));
    var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal($"The One-Time Password (OTP) 'Id={oneTimePassword.Id}' has no password.", exception.Message);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_it_not_valid()
  {
    ChangePhonePayload payload = new(_locale.Code)
    {
      Phone = new("CA", "(514) 845-4636"),
      OneTimePassword = new(Guid.NewGuid(), "123456")
    };
    ChangePhoneCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("ChangePhoneValidator", error.ErrorCode);
  }
}
