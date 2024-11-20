using Logitar.Portal.Contracts.Actors;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Natures;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Application.Natures.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadNatureQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<INatureQuerier> _natureQuerier = new();
  private readonly Mock<IPermissionService> _permissionService = new();

  private readonly ReadNatureQueryHandler _handler;

  private readonly UserMock _user = new();
  private readonly WorldMock _world;
  private readonly NatureModel _nature;

  public ReadNatureQueryHandlerTests()
  {
    _handler = new(_natureQuerier.Object, _permissionService.Object);

    _world = new(_user);
    WorldModel worldModel = new(new Actor(_user), _world.Slug.Value)
    {
      Id = _world.Id.ToGuid()
    };
    _nature = new(worldModel, "Agile")
    {
      Id = Guid.NewGuid()
    };
    _natureQuerier.Setup(x => x.ReadAsync(_world.Id, _nature.Id, _cancellationToken)).ReturnsAsync(_nature);
  }

  [Fact(DisplayName = "It should return null when no nature is found.")]
  public async Task It_should_return_null_when_no_nature_is_found()
  {
    ReadNatureQuery query = new(Guid.Empty);
    query.Contextualize(_world);

    Assert.Null(await _handler.Handle(query, _cancellationToken));

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(query, EntityType.Nature, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return the nature found by ID.")]
  public async Task It_should_return_the_nature_found_by_Id()
  {
    ReadNatureQuery query = new(_nature.Id);
    query.Contextualize(_world);

    NatureModel? nature = await _handler.Handle(query, _cancellationToken);
    Assert.Same(_nature, nature);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(query, EntityType.Nature, _cancellationToken), Times.Once);
  }
}
