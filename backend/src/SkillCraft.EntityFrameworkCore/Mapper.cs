using Logitar;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;
using SkillCraft.Contracts.Aspects;
using SkillCraft.Contracts.Castes;
using SkillCraft.Contracts.Characters;
using SkillCraft.Contracts.Comments;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Contracts.Educations;
using SkillCraft.Contracts.Items;
using SkillCraft.Contracts.Languages;
using SkillCraft.Contracts.Lineages;
using SkillCraft.Contracts.Natures;
using SkillCraft.Contracts.Parties;
using SkillCraft.Contracts.Talents;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain.Items;
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

  public AspectModel ToAspect(AspectEntity source) => ToAspect(source, world: null);
  public AspectModel ToAspect(AspectEntity source, WorldModel? world)
  {
    world ??= ToWorld(source.World ?? throw new ArgumentException($"The {nameof(source.World)} is required.", nameof(source)));

    AspectModel destination = new(world, source.Name)
    {
      Id = source.Id,
      Description = source.Description,
      Attributes = source.GetAttributes(),
      Skills = source.GetSkills()
    };

    MapAggregate(source, destination);

    return destination;
  }

  public CasteModel ToCaste(CasteEntity source) => ToCaste(source, world: null);
  public CasteModel ToCaste(CasteEntity source, WorldModel? world)
  {
    world ??= ToWorld(source.World ?? throw new ArgumentException($"The {nameof(source.World)} is required.", nameof(source)));

    CasteModel destination = new(world, source.Name)
    {
      Id = source.Id,
      Description = source.Description,
      Skill = source.Skill,
      WealthRoll = source.WealthRoll,
      Features = source.GetFeatures()
    };

    MapAggregate(source, destination);

    return destination;
  }

  public CharacterModel ToCharacter(CharacterEntity source)
  {
    WorldModel world = source.World == null
      ? throw new ArgumentException($"The {nameof(source.World)} is required.", nameof(source))
      : ToWorld(source.World);

    CharacterModel destination = new()
    {
      Id = source.Id,
      World = world,
      Name = source.Name,
      PlayerName = source.PlayerName,
      Height = source.Height,
      Weight = source.Weight,
      Age = source.Age,
      BaseAttributes = source.GetBaseAttributes()
    };

    if (source.Lineage != null)
    {
      destination.Lineage = ToLineage(source.Lineage, world);
    }
    if (source.Nature != null)
    {
      destination.Nature = ToNature(source.Nature, world);
    }
    if (source.Caste != null)
    {
      destination.Caste = ToCaste(source.Caste, world);
    }
    if (source.Education != null)
    {
      destination.Education = ToEducation(source.Education, world);
    }

    foreach (CustomizationEntity customization in source.Customizations)
    {
      destination.Customizations.Add(ToCustomization(customization, world));
    }
    foreach (AspectEntity aspect in source.Aspects)
    {
      destination.Aspects.Add(ToAspect(aspect, world));
    }

    foreach (CharacterLanguageEntity relation in source.Languages)
    {
      if (relation.Language != null)
      {
        LanguageModel language = ToLanguage(relation.Language, world);
        destination.Languages.Add(new CharacterLanguageModel(language)
        {
          Notes = relation.Notes
        });
      }
    }

    foreach (CharacterTalentEntity relation in source.Talents)
    {
      if (relation.Talent != null)
      {
        TalentModel talent = ToTalent(relation.Talent, world);
        destination.Talents.Add(new CharacterTalentModel(talent)
        {
          Id = relation.Id,
          Cost = relation.Cost,
          Precision = relation.Precision,
          Notes = relation.Notes
        });
      }
    }

    foreach (InventoryEntity inventory in source.Inventory)
    {
      if (inventory.Item != null)
      {
        ItemModel item = ToItem(inventory.Item, world);
        destination.Inventory.Add(new InventoryModel(item)
        {
          Id = inventory.Id,
          ContainingItemId = inventory.ContainingItemId,
          Quantity = inventory.Quantity,
          IsAttuned = inventory.IsAttuned,
          IsEquipped = inventory.IsEquipped,
          IsIdentified = inventory.IsIdentified,
          IsProficient = inventory.IsProficient,
          Skill = inventory.Skill,
          RemainingCharges = inventory.RemainingCharges,
          RemainingResistance = inventory.RemainingResistance,
          NameOverride = inventory.NameOverride,
          DescriptionOverride = inventory.DescriptionOverride,
          ValueOverride = inventory.ValueOverride
        });
      }
    }

    MapAggregate(source, destination);

    return destination;
  }

  public CommentModel ToComment(CommentEntity source)
  {
    WorldModel world = source.World == null
      ? throw new ArgumentException($"The {nameof(source.World)} is required.", nameof(source))
      : ToWorld(source.World);

    CommentModel destination = new(world, source.Text)
    {
      EntityType = source.EntityType,
      EntityId = source.EntityId
    };

    MapAggregate(source, destination);

    return destination;
  }

  public CustomizationModel ToCustomization(CustomizationEntity source) => ToCustomization(source, world: null);
  public CustomizationModel ToCustomization(CustomizationEntity source, WorldModel? world)
  {
    world ??= ToWorld(source.World ?? throw new ArgumentException($"The {nameof(source.World)} is required.", nameof(source)));

    CustomizationModel destination = new(world, source.Name)
    {
      Id = source.Id,
      Type = source.Type,
      Description = source.Description
    };

    MapAggregate(source, destination);

    return destination;
  }

  public EducationModel ToEducation(EducationEntity source) => ToEducation(source, world: null);
  public EducationModel ToEducation(EducationEntity source, WorldModel? world)
  {
    world ??= ToWorld(source.World ?? throw new ArgumentException($"The {nameof(source.World)} is required.", nameof(source)));

    EducationModel destination = new(world, source.Name)
    {
      Id = source.Id,
      Description = source.Description,
      Skill = source.Skill,
      WealthMultiplier = source.WealthMultiplier
    };

    MapAggregate(source, destination);

    return destination;
  }

  public ItemModel ToItem(ItemEntity source) => ToItem(source, world: null);
  public ItemModel ToItem(ItemEntity source, WorldModel? world)
  {
    world ??= ToWorld(source.World ?? throw new ArgumentException($"The {nameof(source.World)} is required.", nameof(source)));

    ItemModel destination = new(world, source.Name)
    {
      Id = source.Id,
      Description = source.Description,
      Value = source.Value,
      Weight = source.Weight,
      IsAttunementRequired = source.IsAttunementRequired,
      Category = source.Category
    };

    switch (source.Category)
    {
      case ItemCategory.Consumable:
        destination.Consumable = source.GetConsumableProperties();
        break;
      case ItemCategory.Container:
        destination.Container = source.GetContainerProperties();
        break;
      case ItemCategory.Device:
        destination.Device = source.GetDeviceProperties();
        break;
      case ItemCategory.Equipment:
        destination.Equipment = source.GetEquipmentProperties();
        break;
      case ItemCategory.Miscellaneous:
        destination.Miscellaneous = source.GetMiscellaneousProperties();
        break;
      case ItemCategory.Money:
        destination.Money = source.GetMoneyProperties();
        break;
      case ItemCategory.Weapon:
        destination.Weapon = source.GetWeaponProperties();
        break;
      default:
        throw new ItemCategoryNotSupportedException(source.Category);
    }

    MapAggregate(source, destination);

    return destination;
  }

  public LanguageModel ToLanguage(LanguageEntity source) => ToLanguage(source, world: null);
  public LanguageModel ToLanguage(LanguageEntity source, WorldModel? world)
  {
    world ??= ToWorld(source.World ?? throw new ArgumentException($"The {nameof(source.World)} is required.", nameof(source)));

    LanguageModel destination = new(world, source.Name)
    {
      Id = source.Id,
      Description = source.Description,
      Script = source.Script,
      TypicalSpeakers = source.TypicalSpeakers
    };

    MapAggregate(source, destination);

    return destination;
  }

  public LineageModel ToLineage(LineageEntity source) => ToLineage(source, world: null);
  public LineageModel ToLineage(LineageEntity source, WorldModel? world)
  {
    world ??= ToWorld(source.World ?? throw new ArgumentException($"The {nameof(source.World)} is required.", nameof(source)));

    LineageModel destination = new(world, source.Name)
    {
      Id = source.Id,
      Description = source.Description,
      Attributes = source.GetAttributes(),
      Traits = source.GetTraits(),
      Languages = source.GetLanguages(ToLanguage, world),
      Names = source.GetNames(),
      Speeds = source.GetSpeeds(),
      Size = source.GetSize(),
      Weight = source.GetWeight(),
      Ages = source.GetAges()
    };

    if (source.Species != null)
    {
      destination.Species = ToLineage(source.Species, world);
    }

    MapAggregate(source, destination);

    return destination;
  }

  public NatureModel ToNature(NatureEntity source) => ToNature(source, world: null);
  public NatureModel ToNature(NatureEntity source, WorldModel? world)
  {
    world ??= ToWorld(source.World ?? throw new ArgumentException($"The {nameof(source.World)} is required.", nameof(source)));

    if (source.GiftId.HasValue && source.Gift == null)
    {
      throw new ArgumentException($"The {nameof(source.Gift)} is required.", nameof(source));
    }
    CustomizationModel? gift = source.Gift == null ? null : ToCustomization(source.Gift, world);

    NatureModel destination = new(world, source.Name)
    {
      Id = source.Id,
      Description = source.Description,
      Attribute = source.Attribute,
      Gift = gift
    };

    MapAggregate(source, destination);

    return destination;
  }

  public PartyModel ToParty(PartyEntity source)
  {
    WorldModel world = source.World == null
      ? throw new ArgumentException($"The {nameof(source.World)} is required.", nameof(source))
      : ToWorld(source.World);

    PartyModel destination = new(world, source.Name)
    {
      Id = source.Id,
      Description = source.Description
    };

    MapAggregate(source, destination);

    return destination;
  }

  public TalentModel ToTalent(TalentEntity source) => ToTalent(source, world: null);
  public TalentModel ToTalent(TalentEntity source, WorldModel? world)
  {
    world ??= ToWorld(source.World ?? throw new ArgumentException($"The {nameof(source.World)} is required.", nameof(source)));

    TalentModel destination = new(world, source.Name)
    {
      Id = source.Id,
      Tier = source.Tier,
      Description = source.Description,
      AllowMultiplePurchases = source.AllowMultiplePurchases,
      Skill = source.Skill
    };

    if (source.RequiredTalent != null)
    {
      destination.RequiredTalent = ToTalent(source.RequiredTalent, world);
    }

    MapAggregate(source, destination);

    return destination;
  }

  public WorldModel ToWorld(WorldEntity source)
  {
    WorldModel destination = new(FindActor(source.OwnerId), source.Slug)
    {
      Id = source.Id,
      Name = source.Name,
      Description = source.Description
    };

    MapAggregate(source, destination);

    return destination;
  }

  private void MapAggregate(AggregateEntity source, Aggregate destination)
  {
    try
    {
      destination.Id = new AggregateId(source.AggregateId).ToGuid();
    }
    catch (Exception)
    {
    }

    destination.Version = source.Version;

    destination.CreatedBy = FindActor(new ActorId(source.CreatedBy));
    destination.CreatedOn = source.CreatedOn.AsUniversalTime();

    destination.UpdatedBy = FindActor(new ActorId(source.UpdatedBy));
    destination.UpdatedOn = source.UpdatedOn.AsUniversalTime();
  }

  private Actor FindActor(Guid id) => FindActor(new ActorId(id));
  private Actor FindActor(ActorId id) => _actors.TryGetValue(id, out Actor? actor) ? actor : _system;
}
