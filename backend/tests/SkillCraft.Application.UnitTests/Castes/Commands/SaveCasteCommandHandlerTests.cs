using Moq;
using SkillCraft.Application.Storages;
using SkillCraft.Domain;
using SkillCraft.Domain.Castes;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Castes.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SaveCasteCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Caste _caste = new(WorldId.NewId(), new Name("Artisan"), UserId.NewId());

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
    SaveCasteCommand command = new(_caste);

    await _handler.Handle(command, _cancellationToken);

    _casteRepository.Verify(x => x.SaveAsync(_caste, _cancellationToken), Times.Once);

    EntityMetadata entity = _caste.GetMetadata();
    _storageService.Verify(x => x.EnsureAvailableAsync(entity, _cancellationToken), Times.Once);
    _storageService.Verify(x => x.UpdateAsync(entity, _cancellationToken), Times.Once);
  }
}
