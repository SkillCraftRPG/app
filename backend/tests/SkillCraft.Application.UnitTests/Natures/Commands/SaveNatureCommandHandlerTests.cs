using Moq;
using SkillCraft.Application.Storages;
using SkillCraft.Domain;
using SkillCraft.Domain.Natures;

namespace SkillCraft.Application.Natures.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SaveNatureCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<INatureRepository> _natureRepository = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly SaveNatureCommandHandler _handler;

  public SaveNatureCommandHandlerTests()
  {
    _handler = new(_natureRepository.Object, _storageService.Object);
  }

  [Fact(DisplayName = "It should save the nature.")]
  public async Task It_should_save_the_nature()
  {
    WorldMock world = new();
    Nature nature = new(world.Id, new Name("Mystérieux"), world.OwnerId);

    SaveNatureCommand command = new(nature);
    await _handler.Handle(command, _cancellationToken);

    _natureRepository.Verify(x => x.SaveAsync(nature, _cancellationToken), Times.Once);

    EntityMetadata entity = nature.GetMetadata();
    _storageService.Verify(x => x.EnsureAvailableAsync(entity, _cancellationToken), Times.Once);
    _storageService.Verify(x => x.UpdateAsync(entity, _cancellationToken), Times.Once);
  }
}
