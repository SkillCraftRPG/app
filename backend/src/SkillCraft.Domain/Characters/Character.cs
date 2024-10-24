using Logitar.EventSourcing;
using MediatR;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Domain.Aspects;
using SkillCraft.Domain.Castes;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Educations;
using SkillCraft.Domain.Languages;
using SkillCraft.Domain.Lineages;
using SkillCraft.Domain.Personalities;
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

  public PersonalityId PersonalityId { get; private set; }
  public IReadOnlyCollection<CustomizationId> CustomizationIds { get; private set; } = [];

  public IReadOnlyCollection<AspectId> AspectIds { get; private set; } = [];

  private BaseAttributes? _baseAttributes = null;
  public BaseAttributes BaseAttributes => _baseAttributes ?? throw new InvalidOperationException($"The {nameof(BaseAttributes)} has not been initialized yet.");

  public CasteId CasteId { get; private set; }
  public EducationId EducationId { get; private set; }

  private readonly Dictionary<LanguageId, LanguageMetadata> _languages = [];
  public IReadOnlyDictionary<LanguageId, LanguageMetadata> Languages => _languages.AsReadOnly();

  private readonly Dictionary<TalentId, TalentMetadata> _talents = [];
  public IReadOnlyDictionary<TalentId, TalentMetadata> Talents => _talents.AsReadOnly();

  public int Level => 0;
  public int Tier => 0;

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
    Personality personality,
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
    if (personality.WorldId != worldId)
    {
      throw new ArgumentException("The personality does not reside in the same world as the character.", nameof(personality));
    }
    IReadOnlyCollection<CustomizationId> customizationIds = GetCustomizationIds(personality, customizations);
    IReadOnlyCollection<AspectId> aspectIds = GetAspectIds(worldId, aspects);
    if (caste.WorldId != worldId)
    {
      throw new ArgumentException("The caste does not reside in the same world as the character.", nameof(caste));
    }
    if (education.WorldId != worldId)
    {
      throw new ArgumentException("The education does not reside in the same world as the character.", nameof(education));
    }

    CreatedEvent @event = new(name, player, lineage.Id, height, weight, age, personality.Id, customizationIds, aspectIds, baseAttributes, caste.Id, education.Id);
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

    PersonalityId = @event.PersonalityId;
    CustomizationIds = @event.CustomizationIds;

    AspectIds = @event.AspectIds;

    _baseAttributes = @event.BaseAttributes;

    CasteId = @event.CasteId;
    EducationId = @event.EducationId;
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
      Raise(new LanguageSet(language.Id, metadata), userId.ActorId);
    }
  }
  protected virtual void Apply(LanguageSet @event)
  {
    _languages[@event.LanguageId] = @event.Metadata;
  }

  public void SetTalent(Talent talent, UserId userId) => SetTalent(talent, options: null, userId);
  public void SetTalent(Talent talent, SetTalentOptions? options, UserId userId)
  {
    if (talent.WorldId != WorldId)
    {
      throw new ArgumentException("The talent does not reside in the same world as the character.", nameof(talent));
    }
    else if (talent.Tier > Tier)
    {
      throw new ArgumentException($"The talent tier ({talent.Tier}) cannot exceed the character tier ({Tier}).", nameof(talent));
    }

    options ??= new();
    int maximumCost = talent.Tier + 2;
    int cost = options.Cost ?? maximumCost;
    if (cost > maximumCost)
    {
      throw new ArgumentException($"The cost cannot exceed the maximum cost ({maximumCost}) for the talent '{talent}' of tier {talent.Tier}.", nameof(options));
    }

    TalentMetadata metadata = new(cost, options.Precision, options.Notes);
    if (!_talents.TryGetValue(talent.Id, out TalentMetadata? existingMetadata) || existingMetadata != metadata)
    {
      Raise(new TalentSet(talent.Id, metadata), userId.ActorId);
    }
  }
  protected virtual void Apply(TalentSet @event)
  {
    _talents[@event.TalentId] = @event.Metadata;
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

  private static IReadOnlyCollection<CustomizationId> GetCustomizationIds(Personality personality, IEnumerable<Customization> customizations)
  {
    HashSet<CustomizationId> customizationIds = new(capacity: customizations.Count());
    int gifts = 0;
    int disabilities = 0;
    foreach (Customization customization in customizations)
    {
      if (!customizationIds.Contains(customization.Id))
      {
        customizationIds.Add(customization.Id);

        if (customization.WorldId != personality.WorldId)
        {
          throw new ArgumentException("One or more customizations do not reside in the same world as the character.", nameof(customizations));
        }
        else if (customization.Id == personality.GiftId)
        {
          throw new ArgumentException("The customizations cannot include the same gift as the personality.", nameof(customizations));
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

    public PersonalityId PersonalityId { get; }
    public IReadOnlyCollection<CustomizationId> CustomizationIds { get; }

    public IReadOnlyCollection<AspectId> AspectIds { get; }

    public BaseAttributes BaseAttributes { get; }

    public CasteId CasteId { get; }
    public EducationId EducationId { get; }

    public CreatedEvent(Name name, PlayerName? player, LineageId lineageId, double height, double weight, int age,
      PersonalityId personalityId, IReadOnlyCollection<CustomizationId> customizationIds, IReadOnlyCollection<AspectId> aspectIds,
      BaseAttributes baseAttributes, CasteId casteId, EducationId educationId)
    {
      Name = name;
      Player = player;

      LineageId = lineageId;
      Height = height;
      Weight = weight;
      Age = age;

      PersonalityId = personalityId;
      CustomizationIds = customizationIds;

      AspectIds = aspectIds;

      BaseAttributes = baseAttributes;

      CasteId = casteId;
      EducationId = educationId;
    }
  }

  public class LanguageSet : DomainEvent, INotification
  {
    public LanguageId LanguageId { get; }
    public LanguageMetadata Metadata { get; }

    public LanguageSet(LanguageId languageId, LanguageMetadata metadata)
    {
      LanguageId = languageId;
      Metadata = metadata;
    }
  }

  public class TalentSet : DomainEvent, INotification
  {
    public TalentId TalentId { get; }
    public TalentMetadata Metadata { get; }

    public TalentSet(TalentId talentId, TalentMetadata metadata)
    {
      TalentId = talentId;
      Metadata = metadata;
    }
  }
}
