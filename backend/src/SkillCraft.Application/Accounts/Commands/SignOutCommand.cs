using MediatR;

namespace SkillCraft.Application.Accounts.Commands;

public record SignOutCommand : Activity, IRequest<Unit>
{
  public Guid? SessionId { get; private init; }
  public Guid? UserId { get; private init; }

  private SignOutCommand()
  {
  }

  public static SignOutCommand Session(Guid id) => new()
  {
    SessionId = id
  };

  public static SignOutCommand User(Guid id) => new()
  {
    UserId = id
  };
}

internal class SignOutCommandHandler : IRequestHandler<SignOutCommand, Unit>
{
  private readonly ISessionService _sessionService;
  private readonly IUserService _userService;

  public SignOutCommandHandler(ISessionService sessionService, IUserService userService)
  {
    _sessionService = sessionService;
    _userService = userService;
  }

  public async Task<Unit> Handle(SignOutCommand command, CancellationToken cancellationToken)
  {
    if (command.SessionId.HasValue)
    {
      await _sessionService.SignOutAsync(command.SessionId.Value, cancellationToken);
    }

    if (command.UserId.HasValue)
    {
      await _userService.SignOutAsync(command.UserId.Value, cancellationToken);
    }

    return Unit.Value;
  }
}
