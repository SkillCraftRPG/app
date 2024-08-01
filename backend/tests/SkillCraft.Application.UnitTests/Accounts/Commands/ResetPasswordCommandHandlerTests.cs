using Bogus;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using Moq;
using SkillCraft.Application.Accounts.Constants;
using SkillCraft.Application.Actors;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Application.Accounts.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class ResetPasswordCommandHandlerTests
{
  private const string PasswordString = "P@s$W0rD";

  private static readonly LocaleUnit _locale = new("fr");

  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IActorService> _actorService = new();
  private readonly Mock<IMessageService> _messageService = new();
  private readonly Mock<ISessionService> _sessionService = new();
  private readonly Mock<ITokenService> _tokenService = new();
  private readonly Mock<IUserService> _userService = new();

  private readonly ResetPasswordCommandHandler _handler;

  public ResetPasswordCommandHandlerTests()
  {
    _handler = new(_actorService.Object, _messageService.Object, _sessionService.Object, _tokenService.Object, _userService.Object);
  }

  [Fact(DisplayName = "It should fake sending a message when the user has no password.")]
  public async Task It_should_fake_sending_a_message_when_the_user_has_no_password()
  {
    User user = new(_faker.Person.Email)
    {
      Email = new(_faker.Person.Email)
    };
    _userService.Setup(x => x.FindAsync(user.Email.Address, _cancellationToken)).ReturnsAsync(user);

    ResetPasswordPayload payload = new(_locale.Code, user.Email.Address);
    ResetPasswordCommand command = new(payload, CustomAttributes: []);
    ResetPasswordResult result = await _handler.Handle(command, _cancellationToken);

    Assert.NotNull(result.RecoveryLinkSentTo);
    Assert.Equal(ContactType.Email, result.RecoveryLinkSentTo.ContactType);
    Assert.Equal(payload.EmailAddress, result.RecoveryLinkSentTo.MaskedContact);
    Assert.False(string.IsNullOrWhiteSpace(result.RecoveryLinkSentTo.ConfirmationNumber));
    Assert.Null(result.Session);

    _tokenService.VerifyNoOtherCalls();
    _messageService.VerifyNoOtherCalls();
  }

  [Fact(DisplayName = "It should fake sending a message when the user is not found.")]
  public async Task It_should_fake_sending_a_message_when_the_user_is_not_found()
  {
    ResetPasswordPayload payload = new(_locale.Code, _faker.Person.Email);
    ResetPasswordCommand command = new(payload, CustomAttributes: []);
    ResetPasswordResult result = await _handler.Handle(command, _cancellationToken);

    Assert.NotNull(result.RecoveryLinkSentTo);
    Assert.Equal(ContactType.Email, result.RecoveryLinkSentTo.ContactType);
    Assert.Equal(payload.EmailAddress, result.RecoveryLinkSentTo.MaskedContact);
    Assert.False(string.IsNullOrWhiteSpace(result.RecoveryLinkSentTo.ConfirmationNumber));
    Assert.Null(result.Session);

    _tokenService.VerifyNoOtherCalls();
    _messageService.VerifyNoOtherCalls();
  }

  [Fact(DisplayName = "It should reset the user password.")]
  public async Task It_should_reset_the_user_password()
  {
    ResetPasswordPayload payload = new(_locale.Code, new ResetPayload("PasswordRecoveryToken", PasswordString));
    Assert.NotNull(payload.Reset);

    User user = new(_faker.Person.UserName)
    {
      Id = Guid.NewGuid(),
      HasPassword = true
    };
    _userService.Setup(x => x.FindAsync(user.Id, _cancellationToken)).ReturnsAsync(user);
    _userService.Setup(x => x.ResetPasswordAsync(user, PasswordString, _cancellationToken)).ReturnsAsync(user);

    ValidatedToken validatedToken = new()
    {
      Subject = user.GetSubject()
    };
    _tokenService.Setup(x => x.ValidateAsync(payload.Reset.Token, TokenTypes.PasswordRecovery, _cancellationToken)).ReturnsAsync(validatedToken);

    CustomAttribute[] customAttributes =
    [
      new("AdditionalInformation", $@"{{""User-Agent"":""{_faker.Internet.UserAgent()}""}}"),
      new("IpAddress", _faker.Internet.Ip())
    ];
    Session session = new(user);
    _sessionService.Setup(x => x.CreateAsync(user, customAttributes, _cancellationToken)).ReturnsAsync(session);

    ResetPasswordCommand command = new(payload, customAttributes);
    ResetPasswordResult result = await _handler.Handle(command, _cancellationToken);

    Assert.Null(result.RecoveryLinkSentTo);
    Assert.Same(session, result.Session);

    _userService.Verify(x => x.ResetPasswordAsync(user, PasswordString, _cancellationToken), Times.Once);
    _actorService.Verify(x => x.SaveAsync(user, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should send a recovery message when the user has a password.")]
  public async Task It_should_send_a_recovery_message_when_the_user_has_a_password()
  {
    User user = new(_faker.Person.Email)
    {
      HasPassword = true,
      Email = new(_faker.Person.Email)
    };
    _userService.Setup(x => x.FindAsync(user.Email.Address, _cancellationToken)).ReturnsAsync(user);

    CreatedToken createdToken = new("PasswordRecoveryToken");
    _tokenService.Setup(x => x.CreateAsync(user, TokenTypes.PasswordRecovery, _cancellationToken)).ReturnsAsync(createdToken);

    SentMessages sentMessages = new([Guid.NewGuid()]);
    _messageService.Setup(x => x.SendAsync(Templates.PasswordRecovery, user, ContactType.Email, _locale, It.Is<IReadOnlyDictionary<string, string>>(v => v.Single().Key == Variables.Token && v.Single().Value == createdToken.Token), _cancellationToken))
    .ReturnsAsync(sentMessages);

    ResetPasswordPayload payload = new(_locale.Code, user.Email.Address);
    ResetPasswordCommand command = new(payload, CustomAttributes: []);
    ResetPasswordResult result = await _handler.Handle(command, _cancellationToken);

    SentMessage sentMessage = sentMessages.ToSentMessage(user.Email);
    Assert.Equal(sentMessage, result.RecoveryLinkSentTo);
    Assert.Null(result.Session);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the user could not be found.")]
  public async Task It_should_throw_ArgumentException_when_the_user_could_not_be_found()
  {
    ResetPasswordPayload payload = new(_locale.Code, new ResetPayload("PasswordRecoveryToken", PasswordString));
    Assert.NotNull(payload.Reset);

    Guid userId = Guid.NewGuid();
    ValidatedToken validatedToken = new()
    {
      Subject = userId.ToString()
    };
    _tokenService.Setup(x => x.ValidateAsync(payload.Reset.Token, TokenTypes.PasswordRecovery, _cancellationToken)).ReturnsAsync(validatedToken);

    ResetPasswordCommand command = new(payload, CustomAttributes: []);
    var exception = await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, _cancellationToken));
    Assert.StartsWith($"The user 'Id={userId}' could not be found.", exception.Message);
    Assert.Equal("payload", exception.ParamName);
  }
}
