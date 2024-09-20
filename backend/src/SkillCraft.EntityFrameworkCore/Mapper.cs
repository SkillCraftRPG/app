using Logitar;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;
using SkillCraft.Contracts.Aspects;
using SkillCraft.Contracts.Castes;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Contracts.Educations;
using SkillCraft.Contracts.Languages;
using SkillCraft.Contracts.Lineages;
using SkillCraft.Contracts.Personalities;
using SkillCraft.Contracts.Talents;
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

  public AspectModel ToAspect(AspectEntity source)
  {
    WorldModel world = source.World == null
      ? throw new ArgumentException($"The {nameof(source.World)} is required.", nameof(source))
      : ToWorld(source.World);

    AspectModel destination = new(world, source.Name)
    {
      Description = source.Description,
      Attributes = source.GetAttributes(),
      Skills = source.GetSkills()
    };

    MapAggregate(source, destination);

    return destination;
  }

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

  public CustomizationModel ToCustomization(CustomizationEntity source)
  {
    WorldModel world = source.World == null
      ? throw new ArgumentException($"The {nameof(source.World)} is required.", nameof(source))
      : ToWorld(source.World);

    CustomizationModel destination = new(world, source.Name)
    {
      Type = source.Type,
      Description = source.Description
    };

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

  public LanguageModel ToLanguage(LanguageEntity source)
  {
    WorldModel world = source.World == null
      ? throw new ArgumentException($"The {nameof(source.World)} is required.", nameof(source))
      : ToWorld(source.World);

    LanguageModel destination = new(world, source.Name)
    {
      Description = source.Description,
      Script = source.Script,
      TypicalSpeakers = source.TypicalSpeakers
    };

    MapAggregate(source, destination);

    return destination;
  }

  public LineageModel ToLineage(LineageEntity source)
  {
    WorldModel world = source.World == null
      ? throw new ArgumentException($"The {nameof(source.World)} is required.", nameof(source))
      : ToWorld(source.World);

    LineageModel destination = new(world, source.Name)
    {
      Description = source.Description,
      Attributes = source.GetAttributes(),
      Features = source.GetFeatures(),
      Languages = source.GetLanguages(ToLanguage),
      Names = source.GetNames(),
      Speeds = source.GetSpeeds(),
      Size = source.GetSize(),
      Weight = source.GetWeight(),
      Ages = source.GetAges()
    };

    MapAggregate(source, destination);

    return destination;
  }

  public PersonalityModel ToPersonality(PersonalityEntity source)
  {
    WorldModel world = source.World == null
      ? throw new ArgumentException($"The {nameof(source.World)} is required.", nameof(source))
      : ToWorld(source.World);

    if (source.GiftId.HasValue && source.Gift == null)
    {
      throw new ArgumentException($"The {nameof(source.Gift)} is required.", nameof(source));
    }
    CustomizationModel? gift = source.Gift == null ? null : ToCustomization(source.Gift);

    PersonalityModel destination = new(world, source.Name)
    {
      Description = source.Description,
      Attribute = source.Attribute,
      Gift = gift
    };

    MapAggregate(source, destination);

    return destination;
  }

  public TalentModel ToTalent(TalentEntity source)
  {
    WorldModel world = source.World == null
      ? throw new ArgumentException($"The {nameof(source.World)} is required.", nameof(source))
      : ToWorld(source.World);

    TalentModel destination = new(world, source.Name)
    {
      Tier = source.Tier,
      Description = source.Description,
      AllowMultiplePurchases = source.AllowMultiplePurchases
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
