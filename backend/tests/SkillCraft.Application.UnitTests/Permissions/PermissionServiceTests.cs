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

  [Fact(DisplayName = "EnsureCanCreateWorldAsync: it should success when the world limit has not been reached.")]
  public async Task EnsureCanCreateWorldAsync_it_should_success_when_the_world_limit_has_not_been_reached()
  {
    UserMock user = new();
    _worldQuerier.Setup(x => x.CountOwnedAsync(user, _cancellationToken)).ReturnsAsync(_accountSettings.WorldLimit - 1);

    await _service.EnsureCanCreateWorldAsync(user, _cancellationToken);
  }
}
