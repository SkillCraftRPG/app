using Logitar.EventSourcing;
using MediatR;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Domain.Aspects;
using SkillCraft.Domain.Castes;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Educations;
using SkillCraft.Domain.Items;
using SkillCraft.Domain.Languages;
using SkillCraft.Domain.Lineages;
using SkillCraft.Domain.Natures;
using SkillCraft.Domain.Talents;
using SkillCraft.Domain.Worlds;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Domain.Characters;

public class Character : AggregateRoot
{
  private UpdatedEvent _updatedEvent = new();

  public new CharacterId Id => new(base.Id);
  public WorldId WorldId => Id.WorldId;
  public Guid EntityId => Id.EntityId;

  private Name? _name = null;
  public Name Name
  {
    get => _name ?? throw new InvalidOperationException($"The {nameof(Name)} has not been initialized yet.");
    set
    {
      if (_name != value)
      {
        _name = value;
        _updatedEvent.Name = value;
      }
    }
  }
  private PlayerName? _player = null;
  public PlayerName? Player
  {
    get => _player;
    set
    {
      if (_player != value)
      {
        _player = value;
        _updatedEvent.Player = new Change<PlayerName>(value);
      }
    }
  }

  public LineageId SpeciesId { get; private set; }
  public LineageId? NationId { get; private set; }
  private readonly Dictionary<LineageId, AttributeBonuses> _lineageAttributes = [];
  private readonly Dictionary<LineageId, Speeds> _lineageSpeeds = [];

  public CharacterSpeeds Speeds
  {
    get
    {
      Dictionary<SpeedKind, int> speeds = new()
      {
        [SpeedKind.Walk] = _lineageSpeeds.Values.Max(speed => speed.Walk),
        [SpeedKind.Climb] = _lineageSpeeds.Values.Max(speed => speed.Climb),
        [SpeedKind.Swim] = _lineageSpeeds.Values.Max(speed => speed.Swim),
        [SpeedKind.Fly] = _lineageSpeeds.Values.Max(speed => speed.Fly),
        [SpeedKind.Hover] = _lineageSpeeds.Values.Max(speed => speed.Hover),
        [SpeedKind.Burrow] = _lineageSpeeds.Values.Max(speed => speed.Burrow)
      };
      foreach (Bonus bonus in _bonuses.Values)
      {
        if (bonus.Category == BonusCategory.Speed && Enum.TryParse(bonus.Target, out SpeedKind kind))
        {
          speeds[kind] += bonus.Value;
        }
      }
      return new CharacterSpeeds(speeds);
    }
  }

  private double _height = 0.0;
  public double Height
  {
    get => _height;
    set
    {
      ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, 0.0, nameof(Height));
      if (_height != value)
      {
        _height = value;
        _updatedEvent.Height = value;
      }
    }
  }
  private double _weight = 0.0;
  public double Weight
  {
    get => _weight;
    set
    {
      ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, 0.0, nameof(Weight));
      if (_weight != value)
      {
        _weight = value;
        _updatedEvent.Weight = value;
      }
    }
  }
  private int _age = 0;
  public int Age
  {
    get => _age;
    set
    {
      ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, 0, nameof(Age));
      if (_age != value)
      {
        _age = value;
        _updatedEvent.Age = value;
      }
    }
  }

  private readonly Dictionary<Guid, Bonus> _bonuses = [];
  public IReadOnlyDictionary<Guid, Bonus> Bonuses => _bonuses.AsReadOnly();

  public NatureId NatureId { get; private set; }
  private Attribute? _natureAttribute = null;
  public IReadOnlyCollection<CustomizationId> CustomizationIds { get; private set; } = [];

  public IReadOnlyCollection<AspectId> AspectIds { get; private set; } = [];

  private BaseAttributes? _baseAttributes = null;
  public BaseAttributes BaseAttributes => _baseAttributes ?? throw new InvalidOperationException($"The {nameof(BaseAttributes)} has not been initialized yet.");

  public CasteId CasteId { get; private set; }
  public EducationId EducationId { get; private set; }

  private int _experience = 0;
  public int Experience
  {
    get => _experience;
    set
    {
      int minimumExperience = ExperienceTable.GetTotalExperience(Level);
      ArgumentOutOfRangeException.ThrowIfLessThan(value, minimumExperience, nameof(Experience));
      if (_experience != value)
      {
        _experience = value;
        _updatedEvent.Experience = value;
      }
    }
  }
  public int Level => 0;
  public bool CanLevelUp => Level < 20 && Experience >= ExperienceTable.GetTotalExperience(Level + 1);
  public int Tier => 0;

  private int _vitality = 0;
  public int Vitality
  {
    get => _vitality;
    set
    {
      ArgumentOutOfRangeException.ThrowIfNegative(value, nameof(Vitality));
      if (_vitality != value)
      {
        _vitality = value;
        _updatedEvent.Vitality = value;
      }
    }
  }
  private int _stamina = 0;
  public int Stamina
  {
    get => _stamina;
    set
    {
      ArgumentOutOfRangeException.ThrowIfNegative(value, nameof(Stamina));
      if (_stamina != value)
      {
        _stamina = value;
        _updatedEvent.Stamina = value;
      }
    }
  }
  private int _bloodAlcoholContent = 0;
  public int BloodAlcoholContent
  {
    get => _bloodAlcoholContent;
    set
    {
      ArgumentOutOfRangeException.ThrowIfNegative(value, nameof(BloodAlcoholContent));
      if (_bloodAlcoholContent != value)
      {
        _bloodAlcoholContent = value;
        _updatedEvent.BloodAlcoholContent = value;
      }
    }
  }
  private int _intoxication = 0;
  public int Intoxication
  {
    get => _intoxication;
    set
    {
      ArgumentOutOfRangeException.ThrowIfNegative(value, nameof(Intoxication));
      if (_intoxication != value)
      {
        _intoxication = value;
        _updatedEvent.Intoxication = value;
      }
    }
  }

  private readonly Dictionary<LanguageId, LanguageMetadata> _languages = [];
  public IReadOnlyDictionary<LanguageId, LanguageMetadata> Languages => _languages.AsReadOnly();

  private readonly Dictionary<TalentId, HashSet<Guid>> _talentIds = [];
  private readonly Dictionary<Guid, CharacterTalent> _talents = [];
  public IReadOnlyDictionary<Guid, CharacterTalent> Talents => _talents.AsReadOnly();

  public int AvailableTalentPoints => 8 + (Level * 4);
  public int SpentTalentPoints => _talents.Values.Sum(t => t.Cost);
  public int RemainingTalentPoints => AvailableTalentPoints - SpentTalentPoints;

  private readonly Dictionary<Guid, CharacterItem> _inventory = [];
  public IReadOnlyDictionary<Guid, CharacterItem> Inventory => _inventory.AsReadOnly();

  public Character() : base()
  {
  }

  public Character(
    WorldId worldId,
    Name name,
    PlayerName? player,
    Lineage species,
    Lineage? nation,
    double height,
    double weight,
    int age,
    Nature nature,
    IEnumerable<Customization> customizations,
    IEnumerable<Aspect> aspects,
    BaseAttributes baseAttributes,
    Caste caste,
    Education education,
    UserId userId,
    Guid? entityId = null) : base(new CharacterId(worldId, entityId).AggregateId)
  {
    if (species.WorldId != worldId)
    {
      throw new ArgumentException("The species does not reside in the same world as the character.", nameof(species));
    }
    else if (species.ParentId.HasValue)
    {
      throw new ArgumentException("The species cannot be a nation.", nameof(species));
    }
    else if (nation != null && nation.ParentId != species.Id)
    {
      throw new ArgumentException("The nation must belong to the species.", nameof(nation));
    }
    ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(height, 0.0, nameof(height));
    ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(weight, 0.0, nameof(weight));
    ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(age, 0, nameof(age));
    if (nature.WorldId != worldId)
    {
      throw new ArgumentException("The nature does not reside in the same world as the character.", nameof(nature));
    }
    IReadOnlyCollection<CustomizationId> customizationIds = GetCustomizationIds(nature, customizations);
    IReadOnlyCollection<AspectId> aspectIds = GetAspectIds(worldId, aspects);
    if (caste.WorldId != worldId)
    {
      throw new ArgumentException("The caste does not reside in the same world as the character.", nameof(caste));
    }
    if (education.WorldId != worldId)
    {
      throw new ArgumentException("The education does not reside in the same world as the character.", nameof(education));
    }

    CreatedEvent @event = new(name, player, species.Id, species.Attributes, species.Speeds,
      nation?.Id, nation?.Attributes, nation?.Speeds, height, weight, age,
      nature.Id, nature.Attribute, customizationIds, aspectIds, baseAttributes, caste.Id, education.Id);
    Raise(@event, userId.ActorId);
  }
  protected virtual void Apply(CreatedEvent @event)
  {
    _name = @event.Name;
    _player = @event.Player;

    SpeciesId = @event.SpeciesId;
    _lineageAttributes[@event.SpeciesId] = @event.SpeciesAttributes;
    _lineageSpeeds[@event.SpeciesId] = @event.SpeciesSpeeds;

    if (@event.NationId.HasValue && @event.NationAttributes != null && @event.NationSpeeds != null)
    {
      NationId = @event.NationId.Value;
      _lineageAttributes[@event.NationId.Value] = @event.NationAttributes;
      _lineageSpeeds[@event.NationId.Value] = @event.NationSpeeds;
    }

    _height = @event.Height;
    _weight = @event.Weight;
    _age = @event.Age;

    NatureId = @event.NatureId;
    _natureAttribute = @event.NatureAttribute;
    CustomizationIds = @event.CustomizationIds;

    AspectIds = @event.AspectIds;

    _baseAttributes = @event.BaseAttributes;

    CasteId = @event.CasteId;
    EducationId = @event.EducationId;
  }

  public void AddBonus(Bonus bonus, UserId userId) => SetBonus(Guid.NewGuid(), bonus, userId);
  public void SetBonus(Guid id, Bonus bonus, UserId userId)
  {
    if (!_bonuses.TryGetValue(id, out Bonus? existingBonus) || existingBonus != bonus)
    {
      Raise(new BonusUpdatedEvent(id, bonus), userId.ActorId);
    }
  }
  protected virtual void Apply(BonusUpdatedEvent @event)
  {
    _bonuses[@event.BonusId] = @event.Bonus;
  }

  public void AddItem(Item item, UserId userId) => AddItem(item, options: null, userId);
  public void AddItem(Item item, SetItemOptions? options, UserId userId) => SetItem(Guid.NewGuid(), item, options, userId);
  public void SetItem(Guid inventoryId, Item item, UserId userId) => SetItem(inventoryId, item, options: null, userId);
  public void SetItem(Guid inventoryId, Item item, SetItemOptions? options, UserId userId)
  {
    if (item.WorldId != WorldId)
    {
      throw new ArgumentException("The item does not reside in the same world as the character.", nameof(item));
    }

    options ??= new();

    _ = _inventory.TryGetValue(inventoryId, out CharacterItem? existingItem);

    CharacterItem characterItem = new(
      item.Id,
      options.ContainingItemId != null ? options.ContainingItemId.Value : existingItem?.ContainingItemId,
      options.Quantity ?? existingItem?.Quantity ?? 1,
      options.IsAttuned != null ? options.IsAttuned.Value : existingItem?.IsAttuned,
      options.IsEquipped ?? existingItem?.IsEquipped ?? false,
      options.IsIdentified ?? existingItem?.IsIdentified ?? true,
      options.IsProficient != null ? options.IsProficient.Value : existingItem?.IsProficient,
      options.Skill != null ? options.Skill.Value : existingItem?.Skill,
      options.RemainingCharges != null ? options.RemainingCharges.Value : existingItem?.RemainingCharges,
      options.RemainingResistance != null ? options.RemainingResistance.Value : existingItem?.RemainingResistance,
      options.NameOverride != null ? options.NameOverride.Value : existingItem?.NameOverride,
      options.DescriptionOverride != null ? options.DescriptionOverride.Value : existingItem?.DescriptionOverride,
      options.ValueOverride != null ? options.ValueOverride.Value : existingItem?.ValueOverride);

    if (existingItem == null || existingItem != characterItem)
    {
      Raise(new InventoryUpdatedEvent(inventoryId, characterItem), userId.ActorId);
    }
  }
  protected virtual void Apply(InventoryUpdatedEvent @event)
  {
    _inventory[@event.InventoryId] = @event.Item;
  }

  public void GainExperience(int experience, UserId userId)
  {
    ArgumentOutOfRangeException.ThrowIfNegativeOrZero(experience, nameof(experience));

    Raise(new ExperienceGainedEvent(experience), userId.ActorId);
  }
  protected virtual void Apply(ExperienceGainedEvent @event)
  {
    _experience += @event.Experience;
  }

  public void RemoveBonus(Guid id, UserId userId)
  {
    if (_bonuses.ContainsKey(id))
    {
      Raise(new BonusRemovedEvent(id), userId.ActorId);
    }
  }
  protected virtual void Apply(BonusRemovedEvent @event)
  {
    _bonuses.Remove(@event.BonusId);
  }

  public void RemoveLanguage(LanguageId languageId, UserId userId)
  {
    if (_languages.ContainsKey(languageId))
    {
      Raise(new LanguageRemovedEvent(languageId), userId.ActorId);
    }
  }
  protected virtual void Apply(LanguageRemovedEvent @event)
  {
    _languages.Remove(@event.LanguageId);
  }

  public void SetLanguage(Language language, Description? notes, UserId userId)
  {
    if (language.WorldId != WorldId)
    {
      throw new ArgumentException("The language does not reside in the same world as the character.", nameof(language));
    }

    LanguageMetadata metadata = new(notes);
    if (!_languages.TryGetValue(language.Id, out LanguageMetadata? existingMetadata) || existingMetadata != metadata)
    {
      Raise(new LanguageUpdatedEvent(language.Id, metadata), userId.ActorId);
    }
  }
  protected virtual void Apply(LanguageUpdatedEvent @event)
  {
    _languages[@event.LanguageId] = @event.Metadata;
  }

  public void AddTalent(Talent talent, UserId userId) => AddTalent(talent, options: null, userId);
  public void AddTalent(Talent talent, SetTalentOptions? options, UserId userId) => SetTalent(Guid.NewGuid(), talent, options, userId);
  public void SetTalent(Guid id, Talent talent, UserId userId) => SetTalent(id, talent, options: null, userId);
  public void SetTalent(Guid id, Talent talent, SetTalentOptions? options, UserId userId)
  {
    if (talent.WorldId != WorldId)
    {
      throw new ArgumentException("The talent does not reside in the same world as the character.", nameof(talent));
    }
    else if (talent.Tier > Tier)
    {
      throw new TalentTierCannotExceedCharacterTierException(this, talent, "TalentId");
    }
    else if (talent.RequiredTalentId.HasValue && !_talentIds.ContainsKey(talent.RequiredTalentId.Value))
    {
      throw new RequiredTalentNotPurchasedException(this, talent, "TalentId");
    }

    if (!talent.AllowMultiplePurchases && _talentIds.TryGetValue(talent.Id, out HashSet<Guid>? relationIds) && relationIds.Any(relationId => relationId != id))
    {
      throw new TalentCannotBePurchasedMultipleTimesException(talent, "TalentId");
    }

    options ??= new();
    int cost = options.Cost ?? talent.MaximumCost;
    talent.ThrowIfMaximumCostExceeded(cost, "Cost");
    if (cost < 0)
    {
      throw new ArgumentOutOfRangeException(nameof(options), "The talent cost cannot be negative.");
    }

    CharacterTalent characterTalent = new(talent.Id, cost, options.Precision, options.Notes);
    if (!_talents.TryGetValue(id, out CharacterTalent? existingTalent) || existingTalent != characterTalent)
    {
      int actualCost = characterTalent.Cost - (existingTalent?.Cost ?? 0);
      if (actualCost > RemainingTalentPoints)
      {
        throw new NotEnoughRemainingTalentPointsException(this, talent, cost, "TalentId");
      }

      Raise(new TalentUpdatedEvent(id, characterTalent), userId.ActorId);
    }
  }
  protected virtual void Apply(TalentUpdatedEvent @event)
  {
    if (!_talentIds.TryGetValue(@event.Talent.Id, out HashSet<Guid>? relationIds))
    {
      relationIds = [];
      _talentIds[@event.Talent.Id] = relationIds;
    }
    relationIds.Add(@event.RelationId);

    _talents[@event.RelationId] = @event.Talent;
  }

  public void RemoveTalent(Guid id, UserId userId)
  {
    if (_talents.ContainsKey(id))
    {
      Raise(new TalentRemovedEvent(id), userId.ActorId);
    }
  }
  protected virtual void Apply(TalentRemovedEvent @event)
  {
    if (_talents.TryGetValue(@event.RelationId, out CharacterTalent? relation))
    {
      TalentId talentId = relation.Id;
      if (_talentIds.TryGetValue(talentId, out HashSet<Guid>? relationIds))
      {
        relationIds.Remove(@event.RelationId);
        if (relationIds.Count == 0)
        {
          _talentIds.Remove(talentId);
        }
      }

      _talents.Remove(@event.RelationId);
    }
  }

  public void Update(UserId userId)
  {
    if (_updatedEvent.HasChanges)
    {
      Raise(_updatedEvent, userId.ActorId, DateTime.Now);
      _updatedEvent = new();
    }
  }
  protected virtual void Apply(UpdatedEvent @event)
  {
    if (@event.Name != null)
    {
      _name = @event.Name;
    }
    if (@event.Player != null)
    {
      _player = @event.Player.Value;
    }

    if (@event.Height.HasValue)
    {
      _height = @event.Height.Value;
    }
    if (@event.Weight.HasValue)
    {
      _weight = @event.Weight.Value;
    }
    if (@event.Age.HasValue)
    {
      _age = @event.Age.Value;
    }

    if (@event.Experience.HasValue)
    {
      _experience = @event.Experience.Value;
    }
    if (@event.Vitality.HasValue)
    {
      _vitality = @event.Vitality.Value;
    }
    if (@event.Stamina.HasValue)
    {
      _stamina = @event.Stamina.Value;
    }
    if (@event.BloodAlcoholContent.HasValue)
    {
      _bloodAlcoholContent = @event.BloodAlcoholContent.Value;
    }
    if (@event.Intoxication.HasValue)
    {
      _intoxication = @event.Intoxication.Value;
    }
  }

  public override string ToString() => $"{Name} | {base.ToString()}";

  private static IReadOnlyCollection<AspectId> GetAspectIds(WorldId worldId, IEnumerable<Aspect> aspects)
  {
    HashSet<AspectId> aspectIds = new(capacity: aspects.Count());
    foreach (Aspect aspect in aspects)
    {
      if (!aspectIds.Contains(aspect.Id))
      {
        aspectIds.Add(aspect.Id);

        if (aspect.WorldId != worldId)
        {
          throw new ArgumentException("One or more aspects do not reside in the same world as the character.", nameof(aspects));
        }
      }
    }
    if (aspectIds.Count != 2)
    {
      throw new ArgumentException("Exactly 2 different aspects should be provided.", nameof(aspects));
    }
    return aspectIds;
  }

  private static IReadOnlyCollection<CustomizationId> GetCustomizationIds(Nature nature, IEnumerable<Customization> customizations)
  {
    HashSet<CustomizationId> customizationIds = new(capacity: customizations.Count());
    int gifts = 0;
    int disabilities = 0;
    foreach (Customization customization in customizations)
    {
      if (!customizationIds.Contains(customization.Id))
      {
        customizationIds.Add(customization.Id);

        if (customization.WorldId != nature.WorldId)
        {
          throw new ArgumentException("One or more customizations do not reside in the same world as the character.", nameof(customizations));
        }
        else if (customization.Id == nature.GiftId)
        {
          throw new ArgumentException("The customizations cannot include the gift of the nature.", nameof(customizations));
        }

        switch (customization.Type)
        {
          case CustomizationType.Disability:
            disabilities++;
            break;
          case CustomizationType.Gift:
            gifts++;
            break;
        }
      }
    }
    if (gifts != disabilities)
    {
      throw new ArgumentException("The customizations must contain an equal number of gifts and disabilities.", nameof(customizations));
    }
    return customizationIds;
  }

  public class BonusRemovedEvent : DomainEvent, INotification
  {
    public Guid BonusId { get; }

    public BonusRemovedEvent(Guid bonusId)
    {
      BonusId = bonusId;
    }
  }

  public class BonusUpdatedEvent : DomainEvent, INotification
  {
    public Guid BonusId { get; }
    public Bonus Bonus { get; }

    public BonusUpdatedEvent(Guid bonusId, Bonus bonus)
    {
      BonusId = bonusId;
      Bonus = bonus;
    }
  }

  public class CreatedEvent : DomainEvent, INotification
  {
    public Name Name { get; }
    public PlayerName? Player { get; }

    public LineageId SpeciesId { get; }
    public AttributeBonuses SpeciesAttributes { get; }
    public Speeds SpeciesSpeeds { get; }

    public LineageId? NationId { get; }
    public AttributeBonuses? NationAttributes { get; }
    public Speeds? NationSpeeds { get; }

    public double Height { get; }
    public double Weight { get; }
    public int Age { get; }

    public NatureId NatureId { get; }
    public Attribute? NatureAttribute { get; }
    public IReadOnlyCollection<CustomizationId> CustomizationIds { get; }

    public IReadOnlyCollection<AspectId> AspectIds { get; }

    public BaseAttributes BaseAttributes { get; }

    public CasteId CasteId { get; }
    public EducationId EducationId { get; }

    public CreatedEvent(Name name, PlayerName? player, LineageId speciesId, AttributeBonuses speciesAttributes, Speeds speciesSpeeds,
      LineageId? nationId, AttributeBonuses? nationAttributes, Speeds? nationSpeeds, double height, double weight, int age,
      NatureId natureId, Attribute? natureAttribute, IReadOnlyCollection<CustomizationId> customizationIds,
      IReadOnlyCollection<AspectId> aspectIds, BaseAttributes baseAttributes, CasteId casteId, EducationId educationId)
    {
      Name = name;
      Player = player;

      SpeciesId = speciesId;
      SpeciesAttributes = speciesAttributes;
      SpeciesSpeeds = speciesSpeeds;

      NationId = nationId;
      NationAttributes = nationAttributes;
      NationSpeeds = nationSpeeds;

      Height = height;
      Weight = weight;
      Age = age;

      NatureId = natureId;
      NatureAttribute = natureAttribute;
      CustomizationIds = customizationIds;

      AspectIds = aspectIds;

      BaseAttributes = baseAttributes;

      CasteId = casteId;
      EducationId = educationId;
    }
  }

  public class ExperienceGainedEvent : DomainEvent, INotification
  {
    public int Experience { get; }

    public ExperienceGainedEvent(int experience)
    {
      Experience = experience;
    }
  }

  public class InventoryUpdatedEvent : DomainEvent, INotification
  {
    public Guid InventoryId { get; }
    public CharacterItem Item { get; }

    public InventoryUpdatedEvent(Guid inventoryId, CharacterItem item)
    {
      InventoryId = inventoryId;
      Item = item;
    }
  }

  public class LanguageRemovedEvent : DomainEvent, INotification
  {
    public LanguageId LanguageId { get; }

    public LanguageRemovedEvent(LanguageId languageId)
    {
      LanguageId = languageId;
    }
  }

  public class LanguageUpdatedEvent : DomainEvent, INotification
  {
    public LanguageId LanguageId { get; }
    public LanguageMetadata Metadata { get; }

    public LanguageUpdatedEvent(LanguageId languageId, LanguageMetadata metadata)
    {
      LanguageId = languageId;
      Metadata = metadata;
    }
  }

  public class TalentRemovedEvent : DomainEvent, INotification
  {
    public Guid RelationId { get; }

    public TalentRemovedEvent(Guid relationId)
    {
      RelationId = relationId;
    }
  }

  public class TalentUpdatedEvent : DomainEvent, INotification
  {
    public Guid RelationId { get; }
    public CharacterTalent Talent { get; }

    public TalentUpdatedEvent(Guid relationId, CharacterTalent talent)
    {
      RelationId = relationId;
      Talent = talent;
    }
  }

  public class UpdatedEvent : DomainEvent, INotification
  {
    public Name? Name { get; set; }
    public Change<PlayerName>? Player { get; set; }

    public double? Height { get; set; }
    public double? Weight { get; set; }
    public int? Age { get; set; }

    public int? Experience { get; set; }
    public int? Vitality { get; set; }
    public int? Stamina { get; set; }
    public int? BloodAlcoholContent { get; set; }
    public int? Intoxication { get; set; }

    public bool HasChanges => Name != null || Player != null || Height != null || Weight != null || Age != null
      || Experience != null || Vitality != null || Stamina != null || BloodAlcoholContent != null || Intoxication != null;
  }
}
