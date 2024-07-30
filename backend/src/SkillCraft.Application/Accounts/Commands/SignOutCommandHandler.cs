using MediatR;

namespace SkillCraft.Application.Accounts.Commands;

internal class SignOutCommandHandler : IRequestHandler<SignOutCommand>
{
  private readonly ISessionService _sessionService;
  private readonly IUserService _userService;

  public SignOutCommandHandler(ISessionService sessionService, IUserService userService)
  {
    _sessionService = sessionService;
    _userService = userService;
  }

  public async Task Handle(SignOutCommand command, CancellationToken cancellationToken)
  {
    if (command.SessionId.HasValue)
    {
      await _sessionService.SignOutAsync(command.SessionId.Value, cancellationToken);
    }

    if (command.UserId.HasValue)
    {
      await _userService.SignOutAsync(command.UserId.Value, cancellationToken);
    }
  }
}
