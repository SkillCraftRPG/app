using Moq;
using SkillCraft.Application.Settings;
using SkillCraft.Application.Worlds;

namespace SkillCraft.Application.Permissions;

[Trait(Traits.Category, Categories.Unit)]
public class PermissionServiceTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly AccountSettings _accountSettings = new()
  {
    WorldLimit = 3
  };
  private readonly Mock<IWorldQuerier> _worldQuerier = new();

  private readonly PermissionService _service;

  public PermissionServiceTests()
  {
    _service = new(_accountSettings, _worldQuerier.Object);
  }

  [Fact(DisplayName = "EnsureCanCreateWorldAsync: it should succeed when the world limit has not been reached.")]
  public async Task EnsureCanCreateWorldAsync_it_should_succeed_when_the_world_limit_has_not_been_reached()
  {
    UserMock user = new();
    _worldQuerier.Setup(x => x.CountOwnedAsync(user, _cancellationToken)).ReturnsAsync(_accountSettings.WorldLimit - 1);

    await _service.EnsureCanCreateWorldAsync(user, _cancellationToken);
  }

  [Fact(DisplayName = "EnsureCanCreateWorldAsync: it should throw PermissionDeniedException when the user has reached the world limit.")]
  public async Task EnsureCanCreateWorldAsync_it_should_throw_PermissionDeniedException_when_the_user_has_reached_the_world_limit()
  {
    UserMock user = new();
    _worldQuerier.Setup(x => x.CountOwnedAsync(user, _cancellationToken)).ReturnsAsync(_accountSettings.WorldLimit);

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _service.EnsureCanCreateWorldAsync(user, _cancellationToken));
    Assert.Equal(Action.Create, exception.Action);
    Assert.Equal(EntityType.World, exception.EntityType);
    Assert.Equal(user.Id, exception.UserId);
    Assert.Null(exception.WorldId);
    Assert.Null(exception.EntityId);
  }
}
