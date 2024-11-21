using SkillCraft.Contracts.Customizations;
using SkillCraft.Domain;
using SkillCraft.Domain.Aspects;
using SkillCraft.Domain.Castes;
using SkillCraft.Domain.Characters;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Educations;
using SkillCraft.Domain.Lineages;
using SkillCraft.Domain.Natures;
using SkillCraft.Domain.Worlds;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Application.Characters;

internal class CharacterBuilder
{
  public World World { get; }
  public Guid? EntityId { get; private set; }

  public Name Name { get; }
  public PlayerName? Player { get; }

  public Lineage Lineage { get; }
  public Nature Nature { get; }
  public IReadOnlyCollection<Customization> Customizations { get; }
  public IReadOnlyCollection<Aspect> Aspects { get; }
  public BaseAttributes BaseAttributes { get; }
  public Caste Caste { get; }
  public Education Education { get; }

  public CharacterBuilder(World? world = null)
  {
    World = world ?? new WorldMock();

    Name = new Name("Heracles Aetos");

    Lineage parent = new(World.Id, parent: null, new Name("Humain"), World.OwnerId);
    Lineage = new Lineage(World.Id, parent, new Name("Orrin"), World.OwnerId);
    Nature = new Nature(World.Id, new Name("Courroucé"), World.OwnerId);
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
    BaseAttributes = new BaseAttributes(agility: 9, coordination: 8, intellect: 8, presence: 8, sensitivity: 8, spirit: 8, vigor: 8,
      best: Attribute.Vigor, worst: Attribute.Sensitivity, mandatory: [Attribute.Agility, Attribute.Agility],
      optional: [Attribute.Sensitivity, Attribute.Vigor], extra: [Attribute.Agility, Attribute.Vigor]);
    Caste = new Caste(World.Id, new Name("Exilé"), World.OwnerId);
    Education = new Education(World.Id, new Name("Champs de bataille"), World.OwnerId);
  }

  public CharacterBuilder WithId(CharacterId id)
  {
    EntityId = id.EntityId;
    return this;
  }

  public Character Build() => new(World.Id, Name, Player, Lineage, height: 1.67, weight: 62.8, age: 20,
    Nature, Customizations, Aspects, BaseAttributes, Caste, Education, World.OwnerId, EntityId);
}
