using Moq;
using SkillCraft.Application.Storages;
using SkillCraft.Domain;
using SkillCraft.Domain.Educations;

namespace SkillCraft.Application.Educations.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SaveEducationCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

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
    WorldMock world = new();
    Education education = new(world.Id, new Name("Classique"), world.OwnerId);

    SaveEducationCommand command = new(education);
    await _handler.Handle(command, _cancellationToken);

    _educationRepository.Verify(x => x.SaveAsync(education, _cancellationToken), Times.Once);

    EntityMetadata entity = education.GetMetadata();
    _storageService.Verify(x => x.EnsureAvailableAsync(entity, _cancellationToken), Times.Once);
    _storageService.Verify(x => x.UpdateAsync(entity, _cancellationToken), Times.Once);
  }
}
