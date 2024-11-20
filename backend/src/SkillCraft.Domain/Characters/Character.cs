using Logitar.EventSourcing;
using MediatR;
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

namespace SkillCraft.Domain.Characters;

public class Character : AggregateRoot
{
  public new CharacterId Id => new(base.Id);
  public WorldId WorldId => Id.WorldId;
  public Guid EntityId => Id.EntityId;

  private Name? _name = null;
  public Name Name => _name ?? throw new InvalidOperationException($"The {nameof(Name)} has not been initialized yet.");
  public PlayerName? Player { get; private set; }

  public LineageId LineageId { get; private set; }
  public double Height { get; private set; }
  public double Weight { get; private set; }
  public int Age { get; private set; }

  public NatureId NatureId { get; private set; }
  public IReadOnlyCollection<CustomizationId> CustomizationIds { get; private set; } = [];

  public IReadOnlyCollection<AspectId> AspectIds { get; private set; } = [];

  private BaseAttributes? _baseAttributes = null;
  public BaseAttributes BaseAttributes => _baseAttributes ?? throw new InvalidOperationException($"The {nameof(BaseAttributes)} has not been initialized yet.");

  public CasteId CasteId { get; private set; }
  public EducationId EducationId { get; private set; }

  public int Level => 0;
  public int Tier => 0;

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
    Lineage lineage,
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
    if (lineage.WorldId != worldId)
    {
      throw new ArgumentException("The lineage does not reside in the same world as the character.", nameof(lineage));
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

    CreatedEvent @event = new(name, player, lineage.Id, height, weight, age, nature.Id, customizationIds, aspectIds, baseAttributes, caste.Id, education.Id);
    Raise(@event, userId.ActorId);
  }
  protected virtual void Apply(CreatedEvent @event)
  {
    _name = @event.Name;
    Player = @event.Player;

    LineageId = @event.LineageId;
    Height = @event.Height;
    Weight = @event.Weight;
    Age = @event.Age;

    NatureId = @event.NatureId;
    CustomizationIds = @event.CustomizationIds;

    AspectIds = @event.AspectIds;

    _baseAttributes = @event.BaseAttributes;

    CasteId = @event.CasteId;
    EducationId = @event.EducationId;
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
      throw new ArgumentException($"The talent tier ({talent.Tier}) cannot exceed the character tier ({Tier}).", nameof(talent));
    }
    else if (talent.RequiredTalentId.HasValue && !_talentIds.ContainsKey(talent.RequiredTalentId.Value))
    {
      throw new ArgumentException("The character did not purchase the required talent yet.", nameof(talent));
    }

    if (!talent.AllowMultiplePurchases && _talentIds.TryGetValue(talent.Id, out HashSet<Guid>? relationIds) && relationIds.Any(relationId => relationId != id))
    {
      throw new ArgumentException("The talent cannot be purchased multiple times.", nameof(talent));
    }

    options ??= new();
    int maximumCost = talent.Tier + 2;
    int cost = options.Cost ?? maximumCost;
    if (cost > maximumCost)
    {
      throw new ArgumentException($"The cost cannot exceed the maximum cost ({maximumCost}) for the talent '{talent}' of tier {talent.Tier}.", nameof(options));
    }
    else if (cost > RemainingTalentPoints)
    {
      throw new ArgumentException($"The cost ({cost}) exceeds the remaining talent points ({RemainingTalentPoints}).", nameof(talent));
    }

    CharacterTalent characterTalent = new(talent.Id, cost, options.Precision, options.Notes);
    if (!_talents.TryGetValue(id, out CharacterTalent? existingTalent) || existingTalent != characterTalent)
    {
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

  public class CreatedEvent : DomainEvent, INotification
  {
    public Name Name { get; }
    public PlayerName? Player { get; }

    public LineageId LineageId { get; }
    public double Height { get; }
    public double Weight { get; }
    public int Age { get; }

    public NatureId NatureId { get; }
    public IReadOnlyCollection<CustomizationId> CustomizationIds { get; }

    public IReadOnlyCollection<AspectId> AspectIds { get; }

    public BaseAttributes BaseAttributes { get; }

    public CasteId CasteId { get; }
    public EducationId EducationId { get; }

    public CreatedEvent(Name name, PlayerName? player, LineageId lineageId, double height, double weight, int age,
      NatureId natureId, IReadOnlyCollection<CustomizationId> customizationIds, IReadOnlyCollection<AspectId> aspectIds,
      BaseAttributes baseAttributes, CasteId casteId, EducationId educationId)
    {
      Name = name;
      Player = player;

      LineageId = lineageId;
      Height = height;
      Weight = weight;
      Age = age;

      NatureId = natureId;
      CustomizationIds = customizationIds;

      AspectIds = aspectIds;

      BaseAttributes = baseAttributes;

      CasteId = casteId;
      EducationId = educationId;
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
}
