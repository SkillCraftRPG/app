using Logitar.EventSourcing;
using MediatR;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Domain.Aspects;
using SkillCraft.Domain.Castes;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Educations;
using SkillCraft.Domain.Lineages;
using SkillCraft.Domain.Personalities;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Domain.Characters;

public class Character : AggregateRoot
{
  public new CharacterId Id => new(base.Id);

  public WorldId WorldId { get; private set; }

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
    CharacterId? id = null) : base(id?.AggregateId)
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

    Raise(new CreatedEvent(worldId, name, player, lineage.Id, height, weight, age, personality.Id, customizationIds, aspectIds, baseAttributes, caste.Id,
      education.Id), userId.ActorId);
  }
  protected virtual void Apply(CreatedEvent @event)
  {
    WorldId = @event.WorldId;

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
    public WorldId WorldId { get; }

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

    public CreatedEvent(WorldId worldId, Name name, PlayerName? player, LineageId lineageId, double height, double weight, int age,
      PersonalityId personalityId, IReadOnlyCollection<CustomizationId> customizationIds, IReadOnlyCollection<AspectId> aspectIds,
      BaseAttributes baseAttributes, CasteId casteId, EducationId educationId)
    {
      WorldId = worldId;

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
}
