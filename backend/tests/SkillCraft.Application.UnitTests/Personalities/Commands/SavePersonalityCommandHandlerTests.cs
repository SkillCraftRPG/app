using Moq;
using SkillCraft.Application.Storages;
using SkillCraft.Domain;
using SkillCraft.Domain.Personalities;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Personalities.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SavePersonalityCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Personality _personality = new(WorldId.NewId(), new Name("Agile"), UserId.NewId());

  private readonly Mock<IPersonalityRepository> _personalityRepository = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly SavePersonalityCommandHandler _handler;

  public SavePersonalityCommandHandlerTests()
  {
    _handler = new(_personalityRepository.Object, _storageService.Object);
  }

  [Fact(DisplayName = "It should save the personality.")]
  public async Task It_should_save_the_personality()
  {
    SavePersonalityCommand command = new(_personality);

    await _handler.Handle(command, _cancellationToken);

    _personalityRepository.Verify(x => x.SaveAsync(_personality, _cancellationToken), Times.Once);

    EntityMetadata entity = _personality.GetMetadata();
    _storageService.Verify(x => x.EnsureAvailableAsync(entity, _cancellationToken), Times.Once);
    _storageService.Verify(x => x.UpdateAsync(entity, _cancellationToken), Times.Once);
  }
}
