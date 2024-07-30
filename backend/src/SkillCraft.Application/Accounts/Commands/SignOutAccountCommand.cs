using MediatR;

namespace SkillCraft.Application.Accounts.Commands;

public record SignOutAccountCommand : IRequest
{
  public Guid? SessionId { get; private init; }
  public Guid? UserId { get; private init; }

  private SignOutAccountCommand()
  {
  }

  public static SignOutAccountCommand Session(Guid id) => new()
  {
    SessionId = id
  };

  public static SignOutAccountCommand User(Guid id) => new()
  {
    UserId = id
  };
}
