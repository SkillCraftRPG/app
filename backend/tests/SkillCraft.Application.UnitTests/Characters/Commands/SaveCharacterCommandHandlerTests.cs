using Bogus;
using Moq;
using SkillCraft.Application.Storages;
using SkillCraft.Domain;
using SkillCraft.Domain.Aspects;
using SkillCraft.Domain.Castes;
using SkillCraft.Domain.Characters;
using SkillCraft.Domain.Educations;
using SkillCraft.Domain.Lineages;
using SkillCraft.Domain.Natures;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Application.Characters.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SaveCharacterCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<ICharacterRepository> _characterRepository = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly SaveCharacterCommandHandler _handler;

  public SaveCharacterCommandHandlerTests()
  {
    _handler = new(_characterRepository.Object, _storageService.Object);
  }

  [Fact(DisplayName = "It should save the character.")]
  public async Task It_should_save_the_character()
  {
    WorldMock world = new();
    Lineage species = new(world.Id, parent: null, new Name("Humain"), world.OwnerId);
    Lineage nation = new(world.Id, species, new Name("Orrin"), world.OwnerId);
    Nature nature = new(world.Id, new Name("Courroucé"), world.OwnerId);
    Aspect[] aspects =
    [
      new(world.Id, new Name("Farouche"), world.OwnerId),
      new(world.Id, new Name("Gymnaste"), world.OwnerId)
    ];
    BaseAttributes baseAttributes = new(agility: 9, coordination: 9, intellect: 6, presence: 10, sensitivity: 7, spirit: 6, vigor: 10,
      best: Attribute.Agility, worst: Attribute.Sensitivity, mandatory: [Attribute.Agility, Attribute.Vigor], optional: [Attribute.Coordination, Attribute.Vigor],
      extra: [Attribute.Agility, Attribute.Vigor]);
    Caste caste = new(world.Id, new Name("Milicien"), world.OwnerId);
    Education education = new(world.Id, new Name("Champs de bataille"), world.OwnerId);
    Character character = new(world.Id, new Name("Heracles Aetos"), new PlayerName(_faker.Person.FullName),
      species, nation, height: 1.84, weight: 84.6, age: 30, nature, customizations: [],
      aspects, baseAttributes, caste, education, world.OwnerId);

    SaveCharacterCommand command = new(character);

    await _handler.Handle(command, _cancellationToken);

    _characterRepository.Verify(x => x.SaveAsync(character, _cancellationToken), Times.Once);

    EntityMetadata entity = character.GetMetadata();
    _storageService.Verify(x => x.EnsureAvailableAsync(entity, _cancellationToken), Times.Once);
    _storageService.Verify(x => x.UpdateAsync(entity, _cancellationToken), Times.Once);
  }
}
