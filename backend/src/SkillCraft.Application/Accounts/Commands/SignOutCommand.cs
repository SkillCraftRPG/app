using MediatR;

namespace SkillCraft.Application.Accounts.Commands;

public record SignOutCommand : IRequest
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
