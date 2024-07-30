using Logitar.Portal.Contracts.Users;

namespace SkillCraft.Application.Actors;

public interface IActorService
{
  Task SaveAsync(User user, CancellationToken cancellationToken = default);
}
