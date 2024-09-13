using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Educations;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain;

namespace SkillCraft.Application.Educations.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadEducationQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IEducationQuerier> _educationQuerier = new();

  private readonly ReadEducationQueryHandler _handler;

  private readonly EducationModel _education = new(new WorldModel(), "Classique") { Id = Guid.NewGuid() };

  public ReadEducationQueryHandlerTests()
  {
    _handler = new(_educationQuerier.Object, _permissionService.Object);

    _educationQuerier.Setup(x => x.ReadAsync(_education.Id, _cancellationToken)).ReturnsAsync(_education);
  }

  [Fact(DisplayName = "It should return null when no education is found.")]
  public async Task It_should_return_null_when_no_education_is_found()
  {
    ReadEducationQuery query = new(Guid.Empty);
    Assert.Null(await _handler.Handle(query, _cancellationToken));

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(It.IsAny<ReadEducationQuery>(), It.IsAny<EntityMetadata>(), It.IsAny<CancellationToken>()), Times.Never);
  }

  [Fact(DisplayName = "It should return the education found by ID.")]
  public async Task It_should_return_the_education_found_by_Id()
  {
    ReadEducationQuery query = new(_education.Id);
    EducationModel? education = await _handler.Handle(query, _cancellationToken);
    Assert.Same(_education, education);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(
      query,
      It.Is<EntityMetadata>(y => y.WorldId.ToGuid() == _education.World.Id && y.Key.Type == EntityType.Education && y.Key.Id == _education.Id && y.Size > 0),
      _cancellationToken), Times.Once);
  }
}
