using SkillCraft.Contracts.Customizations;
using SkillCraft.Domain.Aspects;
using SkillCraft.Domain.Castes;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Educations;
using SkillCraft.Domain.Lineages;
using SkillCraft.Domain.Natures;
using SkillCraft.Domain.Worlds;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Domain.Characters;

public class CharacterBuilder
{
  public World World { get; }
  public Guid? EntityId { get; private set; }

  public Name Name { get; }
  public PlayerName? Player { get; }

  public Lineage Species { get; }
  public Lineage? Nation { get; }
  public Nature Nature { get; }
  public IReadOnlyCollection<Customization> Customizations { get; }
  public IReadOnlyCollection<Aspect> Aspects { get; }
  public BaseAttributes BaseAttributes { get; }
  public Caste Caste { get; }
  public Education Education { get; }

  public int Experience { get; private set; }

  public CharacterBuilder(World? world = null)
  {
    World = world ?? new WorldMock();

    Name = new Name("Heracles Aetos");

    Species = new Lineage(World.Id, parent: null, new Name("Humain"), World.OwnerId);
    Nation = new Lineage(World.Id, Species, new Name("Orrin"), World.OwnerId);
    Nature = new Nature(World.Id, new Name("Courroucé"), World.OwnerId)
    {
      Attribute = Attribute.Agility
    };
    Customizations =
    [
      new Customization(World.Id, CustomizationType.Gift, new Name("Dur à cuire"), World.OwnerId),
      new Customization(World.Id, CustomizationType.Disability, new Name("Chaotique"), World.OwnerId)
    ];
    Aspects =
    [
      new Aspect(World.Id, new Name("Farouche"), World.OwnerId),
      new Aspect(World.Id, new Name("Gymnaste"), World.OwnerId)
    ];
    BaseAttributes = new BaseAttributes(agility: 9, coordination: 9, intellect: 6, presence: 10, sensitivity: 7, spirit: 6, vigor: 10,
      best: Attribute.Agility, worst: Attribute.Sensitivity, mandatory: [Attribute.Agility, Attribute.Vigor],
      optional: [Attribute.Coordination, Attribute.Vigor], extra: [Attribute.Agility, Attribute.Vigor]);
    Caste = new Caste(World.Id, new Name("Exilé"), World.OwnerId);
    Education = new Education(World.Id, new Name("Champs de bataille"), World.OwnerId);
  }

  public CharacterBuilder CanLevelUpTo(int level)
  {
    Experience = ExperienceTable.GetTotalExperience(level);
    return this;
  }

  public CharacterBuilder WithId(CharacterId id)
  {
    EntityId = id.EntityId;
    return this;
  }

  public Character Build()
  {
    Character character = new(World.Id, Name, Player, Species, Nation, height: 1.67, weight: 62.8, age: 20,
      Nature, Customizations, Aspects, BaseAttributes, Caste, Education, World.OwnerId, EntityId)
    {
      Experience = Experience
    };
    character.Update(World.OwnerId);
    return character;
  }
}
