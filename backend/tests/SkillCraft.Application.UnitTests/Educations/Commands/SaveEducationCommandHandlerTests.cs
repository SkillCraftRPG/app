using Moq;
using SkillCraft.Application.Storages;
using SkillCraft.Domain;
using SkillCraft.Domain.Educations;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Educations.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SaveEducationCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Education _education = new(WorldId.NewId(), new Name("Classique"), UserId.NewId());

  private readonly Mock<IEducationRepository> _educationRepository = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly SaveEducationCommandHandler _handler;

  public SaveEducationCommandHandlerTests()
  {
    _handler = new(_educationRepository.Object, _storageService.Object);
  }

  [Fact(DisplayName = "It should save the education.")]
  public async Task It_should_save_the_education()
  {
    SaveEducationCommand command = new(_education);

    await _handler.Handle(command, _cancellationToken);

    _educationRepository.Verify(x => x.SaveAsync(_education, _cancellationToken), Times.Once);

    EntityMetadata entity = _education.GetMetadata();
    _storageService.Verify(x => x.EnsureAvailableAsync(entity, _cancellationToken), Times.Once);
    _storageService.Verify(x => x.UpdateAsync(entity, _cancellationToken), Times.Once);
  }
}
