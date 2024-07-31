using Moq;

namespace SkillCraft.Application.Accounts.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SignOutCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ISessionService> _sessionService = new();
  private readonly Mock<IUserService> _userService = new();

  private readonly SignOutCommandHandler _handler;

  public SignOutCommandHandlerTests()
  {
    _handler = new(_sessionService.Object, _userService.Object);
  }

  [Fact(DisplayName = "It should sign-out the specified session.")]
  public async Task It_should_sign_out_the_specified_session()
  {
    SignOutCommand command = SignOutCommand.Session(Guid.NewGuid());
    await _handler.Handle(command, _cancellationToken);

    Assert.True(command.SessionId.HasValue);
    _sessionService.Verify(x => x.SignOutAsync(command.SessionId.Value, _cancellationToken), Times.Once);
    _sessionService.VerifyNoOtherCalls();

    _userService.VerifyNoOtherCalls();
  }

  [Fact(DisplayName = "It should sign-out the specified user.")]
  public async Task It_should_sign_out_the_specified_user()
  {
    SignOutCommand command = SignOutCommand.SignOutUser(Guid.NewGuid());
    await _handler.Handle(command, _cancellationToken);

    _sessionService.VerifyNoOtherCalls();

    Assert.True(command.UserId.HasValue);
    _userService.Verify(x => x.SignOutAsync(command.UserId.Value, _cancellationToken), Times.Once);
    _userService.VerifyNoOtherCalls();
  }
}
