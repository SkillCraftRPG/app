using Moq;
using SkillCraft.Application.Storages;
using SkillCraft.Domain;
using SkillCraft.Domain.Languages;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Languages.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SaveLanguageCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Language _language = new(WorldId.NewId(), new Name("Orrinique"), UserId.NewId());

  private readonly Mock<ILanguageRepository> _languageRepository = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly SaveLanguageCommandHandler _handler;

  public SaveLanguageCommandHandlerTests()
  {
    _handler = new(_languageRepository.Object, _storageService.Object);
  }

  [Fact(DisplayName = "It should save the language.")]
  public async Task It_should_save_the_language()
  {
    SaveLanguageCommand command = new(_language);

    await _handler.Handle(command, _cancellationToken);

    _languageRepository.Verify(x => x.SaveAsync(_language, _cancellationToken), Times.Once);

    EntityMetadata entity = _language.GetMetadata();
    _storageService.Verify(x => x.EnsureAvailableAsync(entity, _cancellationToken), Times.Once);
    _storageService.Verify(x => x.UpdateAsync(entity, _cancellationToken), Times.Once);
  }
}
