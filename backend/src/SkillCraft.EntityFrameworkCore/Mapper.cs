using Logitar;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;
using SkillCraft.Contracts.Castes;
using SkillCraft.Contracts.Educations;
using SkillCraft.Contracts.Worlds;
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

  public CasteModel ToCaste(CasteEntity source)
  {
    WorldModel world = source.World == null
      ? throw new ArgumentException($"The {nameof(source.World)} is required.", nameof(source))
      : ToWorld(source.World);

    CasteModel destination = new(world, source.Name)
    {
      Description = source.Description,
      Skill = source.Skill,
      WealthRoll = source.WealthRoll
    };

    foreach (KeyValuePair<Guid, TraitEntity> trait in source.Traits)
    {
      destination.Traits.Add(new TraitModel(trait.Value.Name)
      {
        Id = trait.Key,
        Description = trait.Value.Description
      });
    }

    MapAggregate(source, destination);

    return destination;
  }

  public EducationModel ToEducation(EducationEntity source)
  {
    WorldModel world = source.World == null
      ? throw new ArgumentException($"The {nameof(source.World)} is required.", nameof(source))
      : ToWorld(source.World);

    EducationModel destination = new(world, source.Name)
    {
      Description = source.Description,
      Skill = source.Skill,
      WealthMultiplier = source.WealthMultiplier
    };

    MapAggregate(source, destination);

    return destination;
  }

  public WorldModel ToWorld(WorldEntity source)
  {
    WorldModel destination = new(FindActor(source.OwnerId), source.Slug)
    {
      Name = source.Name,
      Description = source.Description
    };

    MapAggregate(source, destination);

    return destination;
  }

  private void MapAggregate(AggregateEntity source, Aggregate destination)
  {
    destination.Id = new AggregateId(source.AggregateId).ToGuid();
    destination.Version = source.Version;

    destination.CreatedBy = FindActor(new ActorId(source.CreatedBy));
    destination.CreatedOn = source.CreatedOn.AsUniversalTime();

    destination.UpdatedBy = FindActor(new ActorId(source.UpdatedBy));
    destination.UpdatedOn = source.UpdatedOn.AsUniversalTime();
  }

  private Actor FindActor(Guid id) => FindActor(new ActorId(id));
  private Actor FindActor(ActorId id) => _actors.TryGetValue(id, out Actor? actor) ? actor : _system;
}
