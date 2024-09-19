using Logitar.EventSourcing;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Languages;
using SkillCraft.Contracts.Lineages;
using SkillCraft.Domain.Lineages;

namespace SkillCraft.EntityFrameworkCore.Entities;

internal class LineageEntity : AggregateEntity
{
  public int LineageId { get; private set; }
  public Guid Id { get; private set; }

  public WorldEntity? World { get; private set; }
  public int WorldId { get; private set; }

  public LineageEntity? Species { get; private set; }
  public int? ParentId { get; private set; }
  public List<LineageEntity> Nations { get; private set; } = [];

  public string Name { get; private set; } = string.Empty;
  public string? Description { get; private set; }

  public int Agility { get; private set; }
  public int Coordination { get; private set; }
  public int Intellect { get; private set; }
  public int Presence { get; private set; }
  public int Sensitivity { get; private set; }
  public int Spirit { get; private set; }
  public int Vigor { get; private set; }
  public int ExtraAttributes { get; private set; }
  public string? Traits { get; private set; }

  public List<LanguageEntity> Languages { get; private set; } = [];
  public int ExtraLanguages { get; private set; }
  public string? LanguagesText { get; private set; }
  public string? NamesText { get; private set; }
  public string? FamilyNames { get; private set; }
  public string? FemaleNames { get; private set; }
  public string? MaleNames { get; private set; }
  public string? UnisexNames { get; private set; }
  public string? CustomNames { get; private set; }

  public int WalkSpeed { get; private set; }
  public int ClimbSpeed { get; private set; }
  public int SwimSpeed { get; private set; }
  public int FlySpeed { get; private set; }
  public int HoverSpeed { get; private set; }
  public int BurrowSpeed { get; private set; }
  public SizeCategory SizeCategory { get; private set; }
  public string? SizeRoll { get; private set; }
  public string? StarvedRoll { get; private set; }
  public string? SkinnyRoll { get; private set; }
  public string? NormalRoll { get; private set; }
  public string? OverweightRoll { get; private set; }
  public string? ObeseRoll { get; private set; }
  public int? AdolescentAge { get; private set; }
  public int? AdultAge { get; private set; }
  public int? MatureAge { get; private set; }
  public int? VenerableAge { get; private set; }

  public LineageEntity(WorldEntity world, LineageEntity? parent, Lineage.CreatedEvent @event) : base(@event)
  {
    Id = @event.AggregateId.ToGuid();

    World = world;
    WorldId = world.WorldId;

    Species = parent;
    ParentId = parent?.ParentId;

    Name = @event.Name.Value;
  }

  private LineageEntity() : base()
  {
  }

  public override IEnumerable<ActorId> GetActorIds()
  {
    List<ActorId> actorIds = base.GetActorIds().ToList();
    if (World != null)
    {
      actorIds.AddRange(World.GetActorIds());
    }
    if (Species != null)
    {
      actorIds.AddRange(Species.GetActorIds());
    }
    foreach (LanguageEntity language in Languages)
    {
      actorIds.AddRange(language.GetActorIds());
    }
    return actorIds.AsReadOnly();
  }

  public AgesModel GetAges() => new()
  {
    Adolescent = AdolescentAge,
    Adult = AdultAge,
    Mature = MatureAge,
    Venerable = VenerableAge
  };
  public AttributeBonusesModel GetAttributes() => new()
  {
    Agility = Agility,
    Coordination = Coordination,
    Intellect = Intellect,
    Presence = Presence,
    Sensitivity = Sensitivity,
    Spirit = Spirit,
    Vigor = Vigor,
    Extra = ExtraAttributes
  };
  public LanguagesModel GetLanguages(Func<LanguageEntity, LanguageModel> map)
  {
    IEnumerable<LanguageModel> languages = Languages.Select(map).OrderBy(x => x.Name);
    return new LanguagesModel(languages)
    {
      Extra = ExtraLanguages,
      Text = LanguagesText
    };
  }
  public NamesModel GetNames()
  {
    Dictionary<string, List<string>> custom = (CustomNames == null ? null : JsonSerializer.Deserialize<Dictionary<string, List<string>>>(CustomNames)) ?? [];
    return new NamesModel
    {
      Text = NamesText,
      Family = (FamilyNames == null ? null : JsonSerializer.Deserialize<List<string>>(FamilyNames)) ?? [],
      Female = (FemaleNames == null ? null : JsonSerializer.Deserialize<List<string>>(FemaleNames)) ?? [],
      Male = (MaleNames == null ? null : JsonSerializer.Deserialize<List<string>>(MaleNames)) ?? [],
      Unisex = (UnisexNames == null ? null : JsonSerializer.Deserialize<List<string>>(UnisexNames)) ?? [],
      Custom = [.. custom.Select(x => new NameCategory(x.Key, x.Value)).OrderBy(x => x.Key)]
    };
  }
  public SizeModel GetSize() => new(SizeCategory, SizeRoll);
  public SpeedsModel GetSpeeds() => new()
  {
    Walk = WalkSpeed,
    Climb = ClimbSpeed,
    Swim = SwimSpeed,
    Fly = FlySpeed,
    Hover = HoverSpeed,
    Burrow = BurrowSpeed
  };
  public List<TraitModel> GetTraits()
  {
    Dictionary<Guid, TraitEntity> traits = DeserializeTraits();
    return [.. traits.Select(trait => new TraitModel(trait.Value.Name)
    {
      Id = trait.Key,
      Description = trait.Value.Description
    }).OrderBy(x => x.Name)];
  }
  public WeightModel GetWeight() => new()
  {
    Starved = StarvedRoll,
    Skinny = SkinnyRoll,
    Normal = NormalRoll,
    Overweight = OverweightRoll,
    Obese = ObeseRoll
  };

  public void Update(Lineage.UpdatedEvent @event)
  {
    base.Update(@event);

    if (@event.Name != null)
    {
      Name = @event.Name.Value;
    }
    if (@event.Description != null)
    {
      Description = @event.Description.Value?.Value;
    }

    if (@event.Attributes != null)
    {
      SetAttributes(@event.Attributes);
    }
    if (@event.Traits.Count > 0)
    {
      SetTraits(@event.Traits);
    }

    if (@event.Languages != null)
    {
      SetLanguages(@event.Languages);
    }
    if (@event.Names != null)
    {
      SetNames(@event.Names);
    }

    if (@event.Speeds != null)
    {
      SetSpeeds(@event.Speeds);
    }
    if (@event.Size != null)
    {
      SetSize(@event.Size);
    }
    if (@event.Weight != null)
    {
      SetWeight(@event.Weight);
    }
    if (@event.Ages != null)
    {
      SetAges(@event.Ages);
    }
  }
  private void SetAges(IAges ages)
  {
    AdolescentAge = ages.Adolescent;
    AdultAge = ages.Adult;
    MatureAge = ages.Mature;
    VenerableAge = ages.Venerable;
  }
  private void SetAttributes(IAttributeBonuses attributes)
  {
    Agility = attributes.Agility;
    Coordination = attributes.Coordination;
    Intellect = attributes.Intellect;
    Presence = attributes.Presence;
    Sensitivity = attributes.Sensitivity;
    Spirit = attributes.Spirit;
    Vigor = attributes.Vigor;
    ExtraAttributes = attributes.Extra;
  }
  private void SetLanguages(Languages languages)
  {
    ExtraLanguages = languages.Extra;
    LanguagesText = languages.Text;
  }
  private void SetNames(Names names)
  {
    NamesText = names.Text;
    FamilyNames = names.Family.Count == 0 ? null : JsonSerializer.Serialize(names.Family);
    FemaleNames = names.Female.Count == 0 ? null : JsonSerializer.Serialize(names.Female);
    MaleNames = names.Male.Count == 0 ? null : JsonSerializer.Serialize(names.Male);
    UnisexNames = names.Unisex.Count == 0 ? null : JsonSerializer.Serialize(names.Unisex);
    CustomNames = names.Custom.Count == 0 ? null : JsonSerializer.Serialize(names.Custom);
  }
  private void SetSize(Size size)
  {
    SizeCategory = size.Category;
    SizeRoll = size.Roll?.Value;
  }
  private void SetSpeeds(ISpeeds speeds)
  {
    WalkSpeed = speeds.Walk;
    ClimbSpeed = speeds.Climb;
    SwimSpeed = speeds.Swim;
    FlySpeed = speeds.Fly;
    HoverSpeed = speeds.Hover;
    BurrowSpeed = speeds.Burrow;
  }
  private void SetTraits(Dictionary<Guid, Trait?> traits)
  {
    Dictionary<Guid, TraitEntity> entities = DeserializeTraits();
    foreach (KeyValuePair<Guid, Trait?> trait in traits)
    {
      if (trait.Value == null)
      {
        entities.Remove(trait.Key);
      }
      else
      {
        entities[trait.Key] = new TraitEntity(trait.Value.Name.Value, trait.Value.Description?.Value);
      }
    }
    Traits = entities.Count == 0 ? null : JsonSerializer.Serialize(entities);
  }
  private void SetWeight(Weight weight)
  {
    StarvedRoll = weight.Starved?.Value;
    SkinnyRoll = weight.Skinny?.Value;
    NormalRoll = weight.Normal?.Value;
    OverweightRoll = weight.Overweight?.Value;
    ObeseRoll = weight.Obese?.Value;
  }

  private Dictionary<Guid, TraitEntity> DeserializeTraits()
  {
    return (Traits == null ? null : JsonSerializer.Deserialize<Dictionary<Guid, TraitEntity>>(Traits)) ?? [];
  }
}
