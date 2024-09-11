using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Application.Worlds.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadWorldQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IWorldQuerier> _worldQuerier = new();

  private readonly ReadWorldQueryHandler _handler;

  private readonly WorldModel _world1 = new(Actor.System, "new-world") { Id = Guid.NewGuid() };
  private readonly WorldModel _world2 = new(Actor.System, "underworld") { Id = Guid.NewGuid() };

  public ReadWorldQueryHandlerTests()
  {
    _handler = new(_permissionService.Object, _worldQuerier.Object);

    _worldQuerier.Setup(x => x.ReadAsync(_world1.Id, _cancellationToken)).ReturnsAsync(_world1);
    _worldQuerier.Setup(x => x.ReadAsync(_world1.Slug, _cancellationToken)).ReturnsAsync(_world1);
    _worldQuerier.Setup(x => x.ReadAsync(_world2.Id, _cancellationToken)).ReturnsAsync(_world2);
    _worldQuerier.Setup(x => x.ReadAsync(_world2.Slug, _cancellationToken)).ReturnsAsync(_world2);
  }

  [Fact(DisplayName = "It should return null when no world is found.")]
  public async Task It_should_return_null_when_no_world_is_found()
  {
    ReadWorldQuery query = new(Id: null, Slug: null);
    Assert.Null(await _handler.Handle(query, _cancellationToken));

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(It.IsAny<ReadWorldQuery>(), It.IsAny<WorldModel>(), It.IsAny<CancellationToken>()), Times.Never);
  }

  [Fact(DisplayName = "It should return the world found by ID.")]
  public async Task It_should_return_the_world_found_by_Id()
  {
    ReadWorldQuery query = new(_world1.Id, Slug: "test");
    WorldModel? world = await _handler.Handle(query, _cancellationToken);
    Assert.Same(_world1, world);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(query, _world1, _cancellationToken), Times.Once);
    _permissionService.Verify(x => x.EnsureCanPreviewAsync(query, _world2, _cancellationToken), Times.Never);
  }

  [Fact(DisplayName = "It should return the world found by slug.")]
  public async Task It_should_return_the_world_found_by_slug()
  {
    ReadWorldQuery query = new(Guid.Empty, _world2.Slug);
    WorldModel? world = await _handler.Handle(query, _cancellationToken);
    Assert.Same(_world2, world);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(query, _world1, _cancellationToken), Times.Never);
    _permissionService.Verify(x => x.EnsureCanPreviewAsync(query, _world2, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when many worlds were found.")]
  public async Task It_should_throw_TooManyResultsException_when_many_worlds_were_found()
  {
    ReadWorldQuery query = new(_world1.Id, _world2.Slug);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<WorldModel>>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(query, _world1, _cancellationToken), Times.Once);
    _permissionService.Verify(x => x.EnsureCanPreviewAsync(query, _world2, _cancellationToken), Times.Once);
  }
}
