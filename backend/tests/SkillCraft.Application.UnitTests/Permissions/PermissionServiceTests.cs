using Bogus;
using Logitar.Portal.Contracts.Actors;
using Moq;
using SkillCraft.Application.Settings;
using SkillCraft.Application.Worlds;
using SkillCraft.Application.Worlds.Commands;
using SkillCraft.Application.Worlds.Queries;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain;

namespace SkillCraft.Application.Permissions;

[Trait(Traits.Category, Categories.Unit)]
public class PermissionServiceTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

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

    int count = _faker.Random.Number(_accountSettings.WorldLimit - 1);
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

  [Fact(DisplayName = "EnsureCanPreviewAsync: it should succeed when the user can preview the world.")]
  public async Task EnsureCanPreviewAsync_it_should_succeed_when_the_user_can_preview_the_world()
  {
    UserMock user = new(_faker);
    WorldModel world = new(new Actor(user), "new-world")
    {
      Id = Guid.NewGuid()
    };
    ActivityContext context = new(ApiKey: null, Session: null, user, world);
    ReadWorldQuery query = new(world.Id, world.Slug);
    query.Contextualize(context);

    await _service.EnsureCanPreviewAsync(query, world, _cancellationToken);
  }

  [Fact(DisplayName = "EnsureCanPreviewAsync: it should throw PermissionDeniedException when the user does not own the world.")]
  public async Task EnsureCanPreviewAsync_it_should_throw_PermissionDeniedException_when_the_user_does_not_own_the_world()
  {
    UserMock user = new(_faker);
    WorldModel world = new(Actor.System, "new-world")
    {
      Id = Guid.NewGuid()
    };
    ActivityContext context = new(ApiKey: null, Session: null, user, world);
    ReadWorldQuery query = new(world.Id, world.Slug);
    query.Contextualize(context);

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _service.EnsureCanPreviewAsync(query, world, _cancellationToken));
    Assert.Equal(Action.Preview, exception.Action);
    Assert.Equal(EntityType.World, exception.EntityType);
    Assert.Equal(user.Id, exception.UserId);
    Assert.Equal(world.Id, exception.WorldId);
    Assert.Equal(world.Id, exception.EntityId);
  }

  [Fact(DisplayName = "EnsureCanPreviewAsync: it should throw PermissionDeniedException when the world is not the same as the context.")]
  public async Task EnsureCanPreviewAsync_it_should_throw_PermissionDeniedException_when_the_world_is_not_the_same_as_the_context()
  {
    UserMock user = new(_faker);
    WorldModel world = new(new Actor(user), "new-world")
    {
      Id = Guid.NewGuid()
    };
    ActivityContext context = new(ApiKey: null, Session: null, user, world);
    ReadWorldQuery query = new(world.Id, world.Slug);
    query.Contextualize(context);

    WorldModel otherWorld = new(new Actor(user), world.Slug)
    {
      Id = Guid.NewGuid()
    };

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _service.EnsureCanPreviewAsync(query, otherWorld, _cancellationToken));
    Assert.Equal(Action.Preview, exception.Action);
    Assert.Equal(EntityType.World, exception.EntityType);
    Assert.Equal(user.Id, exception.UserId);
    Assert.Equal(world.Id, exception.WorldId);
    Assert.Equal(otherWorld.Id, exception.EntityId);
  }
}
