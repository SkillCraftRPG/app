using Bogus;
using Logitar.Identity.Domain.Settings;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using Moq;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Application.Accounts.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class ResetPasswordCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();
  private readonly UserSettings _userSettings = new()
  {
    UniqueName = new UniqueNameSettings(),
    Password = new PasswordSettings(),
    RequireUniqueEmail = true
  };

  private readonly Mock<IMessageService> _messageService = new();
  private readonly Mock<IRealmService> _realmService = new();
  private readonly Mock<ITokenService> _tokenService = new();
  private readonly Mock<IUserService> _userService = new();

  private readonly ResetPasswordCommandHandler _handler;

  private readonly User _user;

  public ResetPasswordCommandHandlerTests()
  {
    _handler = new(_messageService.Object, _realmService.Object, _tokenService.Object, _userService.Object);

    _realmService.Setup(x => x.GetUserSettingsAsync(_cancellationToken)).ReturnsAsync(_userSettings);

    _user = new(_faker.Person.UserName)
    {
      Email = new Email(_faker.Person.Email)
    };
  }

  [Fact(DisplayName = "It should generate a token and send an email message when the user is found.")]
  public async Task It_should_generate_a_token_and_send_an_email_message_when_the_user_is_found()
  {
    ResetPasswordPayload payload = new(_faker.Locale)
    {
      EmailAddress = _faker.Person.Email
    };
    ResetPasswordCommand command = new(payload);
    _userService.Setup(x => x.FindAsync(payload.EmailAddress, _cancellationToken)).ReturnsAsync(_user);

    CreatedToken createdToken = new("token");
    _tokenService.Setup(x => x.CreateAsync(_user.GetSubject(), "reset_password+jwt", _cancellationToken)).ReturnsAsync(createdToken);

    SentMessages sentMessages = new([Guid.NewGuid()]);
    _messageService.Setup(x => x.SendAsync("PasswordRecovery", _user, payload.Locale,
      It.Is<IEnumerable<KeyValuePair<string, string>>>(x => x.Single().Key == "Token" && x.Single().Value == createdToken.Token),
      _cancellationToken)).ReturnsAsync(sentMessages);

    _ = await _handler.Handle(command, _cancellationToken);

    _messageService.Verify(x => x.SendAsync("PasswordRecovery", _user, payload.Locale,
      It.Is<IEnumerable<KeyValuePair<string, string>>>(x => x.Single().Key == "Token" && x.Single().Value == createdToken.Token),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should not do anything when the user cannot be found.")]
  public async Task It_should_not_do_anything_when_the_user_cannot_be_found()
  {
    ResetPasswordPayload payload = new(_faker.Locale)
    {
      EmailAddress = _faker.Person.Email
    };
    ResetPasswordCommand command = new(payload);

    _ = await _handler.Handle(command, _cancellationToken);

    _tokenService.Verify(x => x.CreateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    _messageService.Verify(x => x.SendAsync(It.IsAny<string>(), It.IsAny<User>(), It.IsAny<string>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<CancellationToken>()), Times.Never);
  }

  [Fact(DisplayName = "It should reset the user password when a token and password are provided.")]
  public async Task It_should_reset_the_user_password_when_a_token_and_password_are_provided()
  {
    ResetPasswordPayload payload = new(_faker.Locale)
    {
      Token = "token",
      Password = "Test123!"
    };
    ResetPasswordCommand command = new(payload);

    ValidatedToken validatedToken = new()
    {
      Subject = _user.GetSubject()
    };
    _tokenService.Setup(x => x.ValidateAsync(payload.Token, "reset_password+jwt", _cancellationToken)).ReturnsAsync(validatedToken);

    _ = await _handler.Handle(command, _cancellationToken);

    _userService.Verify(x => x.ResetPasswordAsync(_user.Id, payload.Password, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    ResetPasswordPayload payload = new(_faker.Locale)
    {
      EmailAddress = "    ",
      Token = "token",
      Password = "AAaa!!11"
    };
    ResetPasswordCommand command = new(payload);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.All(exception.Errors, e => Assert.Equal("Password", e.PropertyName));
  }
}
