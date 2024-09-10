using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore;

internal class Mapper
{
  private readonly Dictionary<ActorId, Actor> _actors = [];
  private readonly Actor _system = new();

  public Mapper()
  {
  }

  public Mapper(IEnumerable<Actor> actors) : this()
  {
    foreach (Actor actor in actors)
    {
      ActorId id = new(actor.Id);
      _actors[id] = actor;
    }
  }

  public static Actor ToActor(UserEntity source) => new(source.DisplayName)
  {
    Id = source.Id,
    Type = ActorType.User,
    IsDeleted = source.IsDeleted,
    EmailAddress = source.EmailAddress,
    PictureUrl = source.PictureUrl
  };

  private Actor FindActor(Guid id) => FindActor(new ActorId(id));
  private Actor FindActor(ActorId id) => _actors.TryGetValue(id, out Actor? actor) ? actor : _system;
}
