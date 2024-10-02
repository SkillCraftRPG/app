using Logitar.Portal.Contracts.Actors;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Educations;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Application.Educations.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadEducationQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IEducationQuerier> _educationQuerier = new();
  private readonly Mock<IPermissionService> _permissionService = new();

  private readonly ReadEducationQueryHandler _handler;

  private readonly UserMock _user = new();
  private readonly WorldMock _world;
  private readonly EducationModel _education;

  public ReadEducationQueryHandlerTests()
  {
    _handler = new(_educationQuerier.Object, _permissionService.Object);

    _world = new(_user);
    WorldModel worldModel = new(new Actor(_user), _world.Slug.Value)
    {
      Id = _world.Id.ToGuid()
    };
    _education = new(worldModel, "Classique")
    {
      Id = Guid.NewGuid()
    };
    _educationQuerier.Setup(x => x.ReadAsync(_world.Id, _education.Id, _cancellationToken)).ReturnsAsync(_education);
  }

  [Fact(DisplayName = "It should return null when no education is found.")]
  public async Task It_should_return_null_when_no_education_is_found()
  {
    ReadEducationQuery query = new(Guid.Empty);
    query.Contextualize(_world);

    Assert.Null(await _handler.Handle(query, _cancellationToken));

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(query, EntityType.Education, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return the education found by ID.")]
  public async Task It_should_return_the_education_found_by_Id()
  {
    ReadEducationQuery query = new(_education.Id);
    query.Contextualize(_world);

    EducationModel? education = await _handler.Handle(query, _cancellationToken);
    Assert.Same(_education, education);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(query, EntityType.Education, _cancellationToken), Times.Once);
  }
}
