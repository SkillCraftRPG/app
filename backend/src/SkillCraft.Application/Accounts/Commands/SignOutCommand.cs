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

  public static SignOutCommand SignOutUser(Guid id) => new()
  {
    UserId = id
  };
}
