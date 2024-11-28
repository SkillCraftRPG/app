using Logitar.EventSourcing;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain.Characters;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.EntityFrameworkCore.Entities;

internal class CharacterEntity : AggregateEntity
{
  private const char Separator = ',';

  public int CharacterId { get; private set; }
  public Guid Id { get; private set; }

  public WorldEntity? World { get; private set; }
  public int WorldId { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? PlayerName { get; private set; }

  public int Experience { get; private set; }
  public int Level { get; private set; }
  public int Tier { get; private set; }

  public int Vitality { get; private set; }
  public int Stamina { get; private set; }
  public int BloodAlcoholContent { get; private set; }
  public int Intoxication { get; private set; }

  public LineageEntity? Lineage { get; private set; }
  public int LineageId { get; private set; }
  public double Height { get; private set; }
  public double Weight { get; private set; }
  public int Age { get; private set; }
  public List<CharacterLanguageEntity> Languages { get; private set; } = [];

  public NatureEntity? Nature { get; private set; }
  public int NatureId { get; private set; }
  public List<CustomizationEntity> Customizations { get; private set; } = [];

  public List<AspectEntity> Aspects { get; private set; } = [];

  public CasteEntity? Caste { get; private set; }
  public int CasteId { get; private set; }
  public EducationEntity? Education { get; private set; }
  public int EducationId { get; private set; }

  public int Agility { get; private set; }
  public int Coordination { get; private set; }
  public int Intellect { get; private set; }
  public int Presence { get; private set; }
  public int Sensitivity { get; private set; }
  public int Spirit { get; private set; }
  public int Vigor { get; private set; }

  public Attribute BestAttribute { get; private set; }
  public Attribute WorstAttribute { get; private set; }
  public string? MandatoryAttributes { get; private set; }
  public string? OptionalAttributes { get; private set; }
  public string? ExtraAttributes { get; private set; }

  public List<CharacterBonusEntity> Bonuses { get; private set; } = [];
  public List<InventoryEntity> Inventory { get; private set; } = [];
  public List<CharacterTalentEntity> Talents { get; private set; } = [];

  public string? LevelUps { get; private set; }

  public CharacterEntity(
    WorldEntity world,
    LineageEntity lineage,
    NatureEntity nature,
    IEnumerable<CustomizationEntity> customizations,
    IEnumerable<AspectEntity> aspects,
    CasteEntity caste,
    EducationEntity education,
    Character.CreatedEvent @event) : base(@event)
  {
    Id = new CharacterId(@event.AggregateId).EntityId;

    World = world;
    WorldId = world.WorldId;

    Name = @event.Name.Value;
    PlayerName = @event.Player?.Value;

    Lineage = lineage;
    LineageId = lineage.LineageId;
    Height = @event.Height;
    Weight = @event.Weight;
    Age = @event.Age;

    Nature = nature;
    NatureId = nature.NatureId;
    Customizations.AddRange(customizations);

    Aspects.AddRange(aspects);

    Caste = caste;
    CasteId = caste.CasteId;
    Education = education;
    EducationId = education.EducationId;

    Agility = @event.BaseAttributes.Agility;
    Coordination = @event.BaseAttributes.Coordination;
    Intellect = @event.BaseAttributes.Intellect;
    Presence = @event.BaseAttributes.Presence;
    Sensitivity = @event.BaseAttributes.Sensitivity;
    Spirit = @event.BaseAttributes.Spirit;
    Vigor = @event.BaseAttributes.Vigor;

    BestAttribute = @event.BaseAttributes.Best;
    WorstAttribute = @event.BaseAttributes.Worst;
    MandatoryAttributes = @event.BaseAttributes.Mandatory.Count == 0 ? null : string.Join(Separator, @event.BaseAttributes.Mandatory);
    OptionalAttributes = @event.BaseAttributes.Optional.Count == 0 ? null : string.Join(Separator, @event.BaseAttributes.Optional);
    ExtraAttributes = @event.BaseAttributes.Extra.Count == 0 ? null : string.Join(Separator, @event.BaseAttributes.Extra);
  }

  private CharacterEntity() : base()
  {
  }

  public override IEnumerable<ActorId> GetActorIds()
  {
    List<ActorId> actorIds = base.GetActorIds().ToList();
    if (World != null)
    {
      actorIds.AddRange(World.GetActorIds());
    }
    if (Lineage != null)
    {
      actorIds.AddRange(Lineage.GetActorIds());
    }
    foreach (CharacterLanguageEntity relation in Languages)
    {
      if (relation.Language != null)
      {
        actorIds.AddRange(relation.Language.GetActorIds());
      }
    }
    if (Nature != null)
    {
      actorIds.AddRange(Nature.GetActorIds());
    }
    foreach (CustomizationEntity customization in Customizations)
    {
      actorIds.AddRange(customization.GetActorIds());
    }
    foreach (AspectEntity aspect in Aspects)
    {
      actorIds.AddRange(aspect.GetActorIds());
    }
    if (Caste != null)
    {
      actorIds.AddRange(Caste.GetActorIds());
    }
    if (Education != null)
    {
      actorIds.AddRange(Education.GetActorIds());
    }
    foreach (InventoryEntity inventory in Inventory)
    {
      if (inventory.Item != null)
      {
        actorIds.AddRange(inventory.Item.GetActorIds());
      }
    }
    foreach (CharacterTalentEntity relation in Talents)
    {
      if (relation.Talent != null)
      {
        actorIds.AddRange(relation.Talent.GetActorIds());
      }
    }
    return actorIds.AsReadOnly();
  }

  public void GainExperience(Character.ExperienceGainedEvent @event)
  {
    base.Update(@event);

    Experience += @event.Experience;
  }

  public BaseAttributesModel GetBaseAttributes()
  {
    BaseAttributesModel model = new()
    {
      Agility = Agility,
      Coordination = Coordination,
      Intellect = Intellect,
      Presence = Presence,
      Sensitivity = Sensitivity,
      Spirit = Spirit,
      Vigor = Vigor,
      Best = BestAttribute,
      Worst = WorstAttribute
    };

    if (MandatoryAttributes != null)
    {
      model.Mandatory.AddRange(MandatoryAttributes.Split(Separator).Select(Enum.Parse<Attribute>));
    }
    if (OptionalAttributes != null)
    {
      model.Optional.AddRange(OptionalAttributes.Split(Separator).Select(Enum.Parse<Attribute>));
    }
    if (ExtraAttributes != null)
    {
      model.Extra.AddRange(ExtraAttributes.Split(Separator).Select(Enum.Parse<Attribute>));
    }

    return model;
  }

  public List<LevelUpModel> GetLevelUps()
  {
    return (LevelUps == null ? null : JsonSerializer.Deserialize<List<LevelUpModel>>(LevelUps)) ?? [];
  }

  public void CancelLevelUp(Character.LevelUpCancelledEvent @event)
  {
    base.Update(@event);

    List<LevelUpModel> levelUps = GetLevelUps();
    if (levelUps.Count > 0)
    {
      levelUps.RemoveAt(levelUps.Count - 1);
      SetLevelUps(levelUps);
    }
  }
  public void LevelUp(Character.LeveledUpEvent @event)
  {
    base.Update(@event);

    List<LevelUpModel> levelUps = GetLevelUps();
    levelUps.Add(new LevelUpModel
    {
      Attribute = @event.LevelUp.Attribute,
      Constitution = @event.LevelUp.Constitution,
      Initiative = @event.LevelUp.Initiative,
      Learning = @event.LevelUp.Learning,
      Power = @event.LevelUp.Power,
      Precision = @event.LevelUp.Precision,
      Reputation = @event.LevelUp.Reputation,
      Strength = @event.LevelUp.Strength
    });
    SetLevelUps(levelUps);
  }
  private void SetLevelUps(IEnumerable<LevelUpModel> levelUps)
  {
    Level = levelUps.Count();
    LevelUps = Level == 0 ? null : JsonSerializer.Serialize(levelUps);
  }

  public void RemoveBonus(Character.BonusRemovedEvent @event)
  {
    base.Update(@event);

    CharacterBonusEntity? bonus = Bonuses.SingleOrDefault(b => b.Id == @event.BonusId);
    if (bonus != null)
    {
      Bonuses.Remove(bonus);
    }
  }

  public void RemoveLanguage(Character.LanguageRemovedEvent @event)
  {
    base.Update(@event);

    CharacterLanguageEntity? relation = Languages.SingleOrDefault(l => l.Language?.Id == @event.LanguageId.EntityId);
    if (relation != null)
    {
      Languages.Remove(relation);
    }
  }

  public void RemoveTalent(Character.TalentRemovedEvent @event)
  {
    base.Update(@event);

    CharacterTalentEntity? relation = Talents.SingleOrDefault(t => t.Id == @event.RelationId);
    if (relation != null)
    {
      Talents.Remove(relation);
    }
  }

  public void SetBonus(Character.BonusUpdatedEvent @event)
  {
    base.Update(@event);

    CharacterBonusEntity? bonus = Bonuses.SingleOrDefault(b => b.Id == @event.BonusId);
    if (bonus == null)
    {
      bonus = new CharacterBonusEntity(this, @event);
      Bonuses.Add(bonus);
    }
    else
    {
      bonus.Update(@event);
    }
  }

  public void SetItem(ItemEntity item, Character.InventoryUpdatedEvent @event)
  {
    base.Update(@event);

    InventoryEntity? relation = Inventory.SingleOrDefault(i => i.Id == @event.InventoryId);
    if (relation == null)
    {
      relation = new InventoryEntity(this, item, @event);
      Inventory.Add(relation);
    }
    else
    {
      relation.Update(@event);
    }
  }

  public void SetLanguage(LanguageEntity language, Character.LanguageUpdatedEvent @event)
  {
    base.Update(@event);

    CharacterLanguageEntity? relation = Languages.SingleOrDefault(l => l.LanguageId == language.LanguageId);
    if (relation == null)
    {
      relation = new CharacterLanguageEntity(this, language, @event);
      Languages.Add(relation);
    }
    else
    {
      relation.Update(@event);
    }
  }

  public void SetTalent(TalentEntity talent, Character.TalentUpdatedEvent @event)
  {
    base.Update(@event);

    CharacterTalentEntity? relation = Talents.SingleOrDefault(t => t.Id == @event.RelationId);
    if (relation == null)
    {
      relation = new CharacterTalentEntity(this, talent, @event);
      Talents.Add(relation);
    }
    else
    {
      relation.Update(@event);
    }
  }

  public void Update(Character.UpdatedEvent @event)
  {
    base.Update(@event);

    if (@event.Name != null)
    {
      Name = @event.Name.Value;
    }
    if (@event.Player != null)
    {
      PlayerName = @event.Player.Value?.Value;
    }

    if (@event.Height.HasValue)
    {
      Height = @event.Height.Value;
    }
    if (@event.Weight.HasValue)
    {
      Weight = @event.Weight.Value;
    }
    if (@event.Age.HasValue)
    {
      Age = @event.Age.Value;
    }

    if (@event.Experience.HasValue)
    {
      Experience = @event.Experience.Value;
    }
    if (@event.Vitality.HasValue)
    {
      Vitality = @event.Vitality.Value;
    }
    if (@event.Stamina.HasValue)
    {
      Stamina = @event.Stamina.Value;
    }
    if (@event.BloodAlcoholContent.HasValue)
    {
      BloodAlcoholContent = @event.BloodAlcoholContent.Value;
    }
    if (@event.Intoxication.HasValue)
    {
      Intoxication = @event.Intoxication.Value;
    }
  }
}
