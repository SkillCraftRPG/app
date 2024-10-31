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

  public LineageEntity? Lineage { get; private set; }
  public int LineageId { get; private set; }
  public double Height { get; private set; }
  public double Weight { get; private set; }
  public int Age { get; private set; }
  public List<CharacterLanguageEntity> Languages { get; private set; } = [];

  public PersonalityEntity? Personality { get; private set; }
  public int PersonalityId { get; private set; }
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

  public List<InventoryEntity> Inventory { get; private set; } = [];
  public List<CharacterTalentEntity> Talents { get; private set; } = [];

  public CharacterEntity(
    WorldEntity world,
    LineageEntity lineage,
    PersonalityEntity personality,
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

    Personality = personality;
    PersonalityId = personality.PersonalityId;
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
    if (Personality != null)
    {
      actorIds.AddRange(Personality.GetActorIds());
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

  public void SetItem(ItemEntity item, Character.InventoryUpdatedEvent @event)
  {
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
}
