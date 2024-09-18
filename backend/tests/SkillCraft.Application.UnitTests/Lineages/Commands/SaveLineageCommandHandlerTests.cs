using Moq;
using SkillCraft.Application.Storages;
using SkillCraft.Domain;
using SkillCraft.Domain.Lineages;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Lineages.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SaveLineageCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Lineage _lineage = new(WorldId.NewId(), parent: null, new Name("Humain"), UserId.NewId());

  private readonly Mock<ILineageRepository> _lineageRepository = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly SaveLineageCommandHandler _handler;

  public SaveLineageCommandHandlerTests()
  {
    _handler = new(_lineageRepository.Object, _storageService.Object);
  }

  [Fact(DisplayName = "It should save the lineage.")]
  public async Task It_should_save_the_lineage()
  {
    SaveLineageCommand command = new(_lineage);

    await _handler.Handle(command, _cancellationToken);

    _lineageRepository.Verify(x => x.SaveAsync(_lineage, _cancellationToken), Times.Once);

    EntityMetadata entity = _lineage.GetMetadata();
    _storageService.Verify(x => x.EnsureAvailableAsync(entity, _cancellationToken), Times.Once);
    _storageService.Verify(x => x.UpdateAsync(entity, _cancellationToken), Times.Once);
  }
}
