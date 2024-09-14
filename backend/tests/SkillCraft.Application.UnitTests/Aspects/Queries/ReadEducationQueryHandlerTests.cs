using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Aspects;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain;

namespace SkillCraft.Application.Aspects.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadAspectQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IAspectQuerier> _aspectQuerier = new();

  private readonly ReadAspectQueryHandler _handler;

  private readonly AspectModel _aspect = new(new WorldModel(), "Œil-de-lynx") { Id = Guid.NewGuid() };

  public ReadAspectQueryHandlerTests()
  {
    _handler = new(_aspectQuerier.Object, _permissionService.Object);

    _aspectQuerier.Setup(x => x.ReadAsync(_aspect.Id, _cancellationToken)).ReturnsAsync(_aspect);
  }

  [Fact(DisplayName = "It should return null when no aspect is found.")]
  public async Task It_should_return_null_when_no_aspect_is_found()
  {
    ReadAspectQuery query = new(Guid.Empty);
    Assert.Null(await _handler.Handle(query, _cancellationToken));

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(It.IsAny<ReadAspectQuery>(), It.IsAny<EntityMetadata>(), It.IsAny<CancellationToken>()), Times.Never);
  }

  [Fact(DisplayName = "It should return the aspect found by ID.")]
  public async Task It_should_return_the_aspect_found_by_Id()
  {
    ReadAspectQuery query = new(_aspect.Id);
    AspectModel? aspect = await _handler.Handle(query, _cancellationToken);
    Assert.Same(_aspect, aspect);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(
      query,
      It.Is<EntityMetadata>(y => y.WorldId.ToGuid() == _aspect.World.Id && y.Key.Type == EntityType.Aspect && y.Key.Id == _aspect.Id && y.Size > 0),
      _cancellationToken), Times.Once);
  }
}
