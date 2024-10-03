using Moq;
using SkillCraft.Application.Storages;
using SkillCraft.Domain;
using SkillCraft.Domain.Castes;

namespace SkillCraft.Application.Castes.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SaveCasteCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICasteRepository> _casteRepository = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly SaveCasteCommandHandler _handler;

  public SaveCasteCommandHandlerTests()
  {
    _handler = new(_casteRepository.Object, _storageService.Object);
  }

  [Fact(DisplayName = "It should save the caste.")]
  public async Task It_should_save_the_caste()
  {
    WorldMock world = new();
    Caste caste = new(world.Id, new Name("Artisan"), world.OwnerId);

    SaveCasteCommand command = new(caste);
    await _handler.Handle(command, _cancellationToken);

    _casteRepository.Verify(x => x.SaveAsync(caste, _cancellationToken), Times.Once);

    EntityMetadata entity = caste.GetMetadata();
    _storageService.Verify(x => x.EnsureAvailableAsync(entity, _cancellationToken), Times.Once);
    _storageService.Verify(x => x.UpdateAsync(entity, _cancellationToken), Times.Once);
  }
}
