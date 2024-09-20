using Bogus;
using Moq;
using SkillCraft.Application.Storages;
using SkillCraft.Domain;
using SkillCraft.Domain.Characters;
using SkillCraft.Domain.Lineages;

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
    Character character = new(world.Id, new Name("Heracles Aetos"), new PlayerName(_faker.Person.FullName), nation, height: 1.84, weight: 84.6, age: 30, world.OwnerId);

    SaveCharacterCommand command = new(character);

    await _handler.Handle(command, _cancellationToken);

    _characterRepository.Verify(x => x.SaveAsync(character, _cancellationToken), Times.Once);

    EntityMetadata entity = character.GetMetadata();
    _storageService.Verify(x => x.EnsureAvailableAsync(entity, _cancellationToken), Times.Once);
    _storageService.Verify(x => x.UpdateAsync(entity, _cancellationToken), Times.Once);
  }
}
