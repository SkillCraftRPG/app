using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Users;

namespace SkillCraft.Application.Actors;

public interface IActorService
{
  Task<IReadOnlyCollection<Actor>> FindAsync(IEnumerable<ActorId> ids, CancellationToken cancellationToken = default);
  Task SaveAsync(User user, CancellationToken cancellationToken = default);
}
