using Moq;
using SkillCraft.Application.Storages;
using SkillCraft.Domain;
using SkillCraft.Domain.Talents;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Talents.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SaveTalentCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Talent _talent = new(WorldId.NewId(), tier: 1, new Name("Ambidextre"), UserId.NewId());

  private readonly Mock<IStorageService> _storageService = new();
  private readonly Mock<ITalentRepository> _talentRepository = new();

  private readonly SaveTalentCommandHandler _handler;

  public SaveTalentCommandHandlerTests()
  {
    _handler = new(_storageService.Object, _talentRepository.Object);
  }

  [Fact(DisplayName = "It should save the talent.")]
  public async Task It_should_save_the_talent()
  {
    SaveTalentCommand command = new(_talent);

    await _handler.Handle(command, _cancellationToken);

    _talentRepository.Verify(x => x.SaveAsync(_talent, _cancellationToken), Times.Once);

    EntityMetadata entity = _talent.GetMetadata();
    _storageService.Verify(x => x.EnsureAvailableAsync(entity, _cancellationToken), Times.Once);
    _storageService.Verify(x => x.UpdateAsync(entity, _cancellationToken), Times.Once);
  }
}
