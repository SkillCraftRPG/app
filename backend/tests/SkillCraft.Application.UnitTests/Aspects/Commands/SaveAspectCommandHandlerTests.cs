using Moq;
using SkillCraft.Application.Storages;
using SkillCraft.Domain;
using SkillCraft.Domain.Aspects;

namespace SkillCraft.Application.Aspects.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SaveAspectCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IAspectRepository> _aspectRepository = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly SaveAspectCommandHandler _handler;

  public SaveAspectCommandHandlerTests()
  {
    _handler = new(_aspectRepository.Object, _storageService.Object);
  }

  [Fact(DisplayName = "It should save the aspect.")]
  public async Task It_should_save_the_aspect()
  {
    WorldMock world = new();
    Aspect aspect = new(world.Id, new Name("Œil-de-lynx"), world.OwnerId);

    SaveAspectCommand command = new(aspect);
    await _handler.Handle(command, _cancellationToken);

    _aspectRepository.Verify(x => x.SaveAsync(aspect, _cancellationToken), Times.Once);

    EntityMetadata entity = aspect.GetMetadata();
    _storageService.Verify(x => x.EnsureAvailableAsync(entity, _cancellationToken), Times.Once);
    _storageService.Verify(x => x.UpdateAsync(entity, _cancellationToken), Times.Once);
  }
}
