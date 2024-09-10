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
      UserEntity[] users = await _context.Users.AsNoTracking()
        .Where(a => missingIds.Contains(a.Id))
        .ToArrayAsync(cancellationToken);

      foreach (UserEntity user in users)
      {
        Actor actor = Mapper.ToActor(user);
        ActorId id = new(user.Id);

        actors[id] = actor;
        _cacheService.SetActor(actor);
      }
    }

    return actors.Values;
  }

  public async Task SaveAsync(User user, CancellationToken cancellationToken)
  {
    UserEntity? entity = await _context.Users.SingleOrDefaultAsync(x => x.Id == user.Id, cancellationToken);
    if (entity == null)
    {
      entity = new(user);
      _context.Users.Add(entity);
    }
    else
    {
      entity.Update(user);
    }

    await _context.SaveChangesAsync(cancellationToken);

    ActorId id = new(user.Id);
    if (_cacheService.GetActor(id) != null)
    {
      Actor actor = new(user);
      _cacheService.SetActor(actor);
    }
  }
}
