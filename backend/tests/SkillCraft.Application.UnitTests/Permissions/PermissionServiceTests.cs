using Moq;
using SkillCraft.Application.Settings;
using SkillCraft.Application.Worlds;
using SkillCraft.Application.Worlds.Commands;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain;

namespace SkillCraft.Application.Permissions;

[Trait(Traits.Category, Categories.Unit)]
public class PermissionServiceTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Random _random = new();

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

  [Fact(DisplayName = "EnsureCanCreateAsync: it should succeed when the world limit has not been reached.")]
  public async Task EnsureCanCreateAsync_it_should_succeed_when_the_world_limit_has_not_been_reached()
  {
    CreateWorldCommand command = new(new CreateWorldPayload());
    ActivityContextMock.Contextualize(command);

    int count = _random.Next(0, _accountSettings.WorldLimit);
    _worldQuerier.Setup(x => x.CountOwnedAsync(command.GetUserId(), _cancellationToken)).ReturnsAsync(count);

    await _service.EnsureCanCreateAsync(command, EntityType.World, _cancellationToken);
  }

  [Fact(DisplayName = "EnsureCanCreateAsync: it should throw PermissionDeniedException when the world limit has been reached.")]
  public async Task EnsureCanCreateAsync_it_should_throw_PermissionDeniedException_when_the_world_limit_has_been_reached()
  {
    CreateWorldCommand command = new(new CreateWorldPayload());
    ActivityContextMock.Contextualize(command);

    _worldQuerier.Setup(x => x.CountOwnedAsync(command.GetUserId(), _cancellationToken)).ReturnsAsync(_accountSettings.WorldLimit);

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _service.EnsureCanCreateAsync(command, EntityType.World, _cancellationToken));
    Assert.Equal(Action.Create, exception.Action);
    Assert.Equal(EntityType.World, exception.EntityType);
    Assert.Equal(command.GetUser().Id, exception.UserId);
    Assert.Null(exception.WorldId);
    Assert.Null(exception.EntityId);
  }
}
