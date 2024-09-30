using Logitar.Portal.Contracts.Actors;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Aspects;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Application.Aspects.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadAspectQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IAspectQuerier> _aspectQuerier = new();
  private readonly Mock<IPermissionService> _permissionService = new();

  private readonly ReadAspectQueryHandler _handler;

  private readonly UserMock _user = new();
  private readonly WorldMock _world;
  private readonly AspectModel _aspect;

  public ReadAspectQueryHandlerTests()
  {
    _handler = new(_aspectQuerier.Object, _permissionService.Object);

    _world = new(_user);
    WorldModel worldModel = new(new Actor(_user), _world.Slug.Value)
    {
      Id = _world.Id.ToGuid()
    };
    _aspect = new(worldModel, "Œil-de-lynx")
    {
      Id = Guid.NewGuid()
    };
    _aspectQuerier.Setup(x => x.ReadAsync(_world.Id, _aspect.Id, _cancellationToken)).ReturnsAsync(_aspect);
  }

  [Fact(DisplayName = "It should return null when no aspect is found.")]
  public async Task It_should_return_null_when_no_aspect_is_found()
  {
    ReadAspectQuery query = new(Guid.Empty);
    query.Contextualize(_world);

    Assert.Null(await _handler.Handle(query, _cancellationToken));

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(query, EntityType.Aspect, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return the aspect found by ID.")]
  public async Task It_should_return_the_aspect_found_by_Id()
  {
    ReadAspectQuery query = new(_aspect.Id);
    query.Contextualize(_world);

    AspectModel? aspect = await _handler.Handle(query, _cancellationToken);
    Assert.Same(_aspect, aspect);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(query, EntityType.Aspect, _cancellationToken), Times.Once);
  }
}
