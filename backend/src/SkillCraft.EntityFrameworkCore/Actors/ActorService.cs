using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Users;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Application.Actors;
using SkillCraft.Application.Caching;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Actors;

internal class ActorService : IActorService
{
  private readonly ICacheService _cacheService;
  private readonly SkillCraftContext _context;

  public ActorService(ICacheService cacheService, SkillCraftContext context)
  {
    _cacheService = cacheService;
    _context = context;
  }

  public async Task<IReadOnlyCollection<Actor>> FindAsync(IEnumerable<ActorId> ids, CancellationToken cancellationToken)
  {
    int capacity = ids.Count();
    Dictionary<ActorId, Actor> actors = new(capacity);
    HashSet<Guid> missingIds = new(capacity);

    foreach (ActorId id in ids)
    {
      if (id != default)
      {
        Actor? actor = _cacheService.GetActor(id);
        if (actor == null)
        {
          missingIds.Add(id.ToGuid());
        }
        else
        {
          actors[id] = actor;
          _cacheService.SetActor(actor);
        }
      }
    }

    if (missingIds.Count > 0)
    {
      ActorEntity[] entities = await _context.Actors.AsNoTracking()
        .Where(a => missingIds.Contains(a.Id))
        .ToArrayAsync(cancellationToken);

      foreach (ActorEntity entity in entities)
      {
        Actor actor = Mapper.ToActor(entity);
        ActorId id = new(entity.Id);

        actors[id] = actor;
        _cacheService.SetActor(actor);
      }
    }

    return actors.Values;
  }

  public async Task SaveAsync(User user, CancellationToken cancellationToken)
  {
    ActorEntity? actor = await _context.Actors.SingleOrDefaultAsync(x => x.Id == user.Id, cancellationToken);
    if (actor == null)
    {
      actor = new(user);
      _context.Actors.Add(actor);
    }
    else
    {
      actor.Update(user);
    }

    await _context.SaveChangesAsync(cancellationToken);
  }
}
