using Bogus;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Users;
using Moq;
using SkillCraft.Application.Educations.Commands;
using SkillCraft.Application.Settings;
using SkillCraft.Application.Worlds;
using SkillCraft.Application.Worlds.Commands;
using SkillCraft.Application.Worlds.Queries;
using SkillCraft.Contracts.Educations;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain;
using SkillCraft.Domain.Worlds;

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

  private readonly User _user;
  private readonly World _userWorld;
  private readonly World _otherWorld;

  public PermissionServiceTests()
  {
    _service = new(_accountSettings, _worldQuerier.Object);

    _user = new UserMock();
    _userWorld = new(new Slug("ungar"), new UserId(_user.Id));
    _otherWorld = new(new Slug("hyrule"), UserId.NewId());
  }

  [Fact(DisplayName = "EnsureCanCreateAsync: it should succeed when the user owns the world.")]
  public async Task EnsureCanCreateAsync_it_should_succeed_when_the_user_owns_the_world()
  {
    CreateEducationCommand command = new(new CreateEducationPayload());
    command.Contextualize(_user, _userWorld);

    await _service.EnsureCanCreateAsync(command, EntityType.Education, _cancellationToken);
  }

  [Fact(DisplayName = "EnsureCanCreateAsync: it should succeed when the world limit has not been reached.")]
  public async Task EnsureCanCreateAsync_it_should_succeed_when_the_world_limit_has_not_been_reached()
  {
    CreateWorldCommand command = new(new CreateWorldPayload());
    command.Contextualize(_user);

    int count = _faker.Random.Number(_accountSettings.WorldLimit - 1);
    _worldQuerier.Setup(x => x.CountOwnedAsync(new UserId(_user.Id), _cancellationToken)).ReturnsAsync(count);

    await _service.EnsureCanCreateAsync(command, EntityType.World, _cancellationToken);
  }

  [Fact(DisplayName = "EnsureCanCreateAsync: it should throw PermissionDeniedException when the user does not own the world.")]
  public async Task EnsureCanCreateAsync_it_should_throw_PermissionDeniedException_when_the_user_does_not_own_the_world()
  {
    CreateEducationCommand command = new(new CreateEducationPayload());
    command.Contextualize(_user, _otherWorld);

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _service.EnsureCanCreateAsync(command, EntityType.Education, _cancellationToken));
    Assert.Equal(Action.Create, exception.Action);
    Assert.Equal(EntityType.Education, exception.EntityType);
    Assert.Equal(_user.Id, exception.UserId);
    Assert.Equal(_otherWorld.Id.ToGuid(), exception.WorldId);
    Assert.Null(exception.EntityId);
  }

  [Fact(DisplayName = "EnsureCanCreateAsync: it should throw PermissionDeniedException when the world limit has been reached.")]
  public async Task EnsureCanCreateAsync_it_should_throw_PermissionDeniedException_when_the_world_limit_has_been_reached()
  {
    CreateWorldCommand command = new(new CreateWorldPayload());
    command.Contextualize(_user);

    _worldQuerier.Setup(x => x.CountOwnedAsync(new UserId(_user.Id), _cancellationToken)).ReturnsAsync(_accountSettings.WorldLimit);

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _service.EnsureCanCreateAsync(command, EntityType.World, _cancellationToken));
    Assert.Equal(Action.Create, exception.Action);
    Assert.Equal(EntityType.World, exception.EntityType);
    Assert.Equal(_user.Id, exception.UserId);
    Assert.Null(exception.WorldId);
    Assert.Null(exception.EntityId);
  }

  [Fact(DisplayName = "EnsureCanPreviewAsync: it should succeed when the user can preview the world.")]
  public async Task EnsureCanPreviewAsync_it_should_succeed_when_the_user_can_preview_the_world()
  {
    ReadWorldQuery query = new(_userWorld.Id.ToGuid(), _userWorld.Slug.Value);
    query.Contextualize(_user);

    WorldModel world = new(new Actor(_user), _userWorld.Slug.Value)
    {
      Id = _userWorld.Id.ToGuid()
    };

    await _service.EnsureCanPreviewAsync(query, world, _cancellationToken);
  }

  [Fact(DisplayName = "EnsureCanPreviewAsync: it should throw PermissionDeniedException when the user does not own the world.")]
  public async Task EnsureCanPreviewAsync_it_should_throw_PermissionDeniedException_when_the_user_does_not_own_the_world()
  {
    ReadWorldQuery query = new(_otherWorld.Id.ToGuid(), _otherWorld.Slug.Value);
    query.Contextualize(_user, _otherWorld);

    WorldModel world = new(Actor.System, _otherWorld.Slug.Value)
    {
      Id = _otherWorld.Id.ToGuid()
    };

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _service.EnsureCanPreviewAsync(query, world, _cancellationToken));
    Assert.Equal(Action.Preview, exception.Action);
    Assert.Equal(EntityType.World, exception.EntityType);
    Assert.Equal(_user.Id, exception.UserId);
    Assert.Equal(_otherWorld.Id.ToGuid(), exception.WorldId);
    Assert.Equal(world.Id, exception.EntityId);
  }

  [Fact(DisplayName = "EnsureCanPreviewAsync: it should throw PermissionDeniedException when the world is not the same as the context.")]
  public async Task EnsureCanPreviewAsync_it_should_throw_PermissionDeniedException_when_the_world_is_not_the_same_as_the_context()
  {
    ReadWorldQuery query = new(_userWorld.Id.ToGuid(), _userWorld.Slug.Value);
    query.Contextualize(_user, _otherWorld);

    WorldModel world = new(new Actor(_user), _userWorld.Slug.Value)
    {
      Id = _userWorld.Id.ToGuid()
    };

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _service.EnsureCanPreviewAsync(query, world, _cancellationToken));
    Assert.Equal(Action.Preview, exception.Action);
    Assert.Equal(EntityType.World, exception.EntityType);
    Assert.Equal(_user.Id, exception.UserId);
    Assert.Equal(_otherWorld.Id.ToGuid(), exception.WorldId);
    Assert.Equal(world.Id, exception.EntityId);
  }

  [Fact(DisplayName = "EnsureCanUpdateAsync: it should succeed when the user can update the world.")]
  public async Task EnsureCanUpdateAsync_it_should_succeed_when_the_user_can_update_the_world()
  {
    UpdateWorldCommand command = new(_userWorld.Id.ToGuid(), new UpdateWorldPayload());
    command.Contextualize(_user);

    await _service.EnsureCanUpdateAsync(command, _userWorld, _cancellationToken);
  }

  [Fact(DisplayName = "EnsureCanUpdateAsync: it should throw PermissionDeniedException when the user does not own the world.")]
  public async Task EnsureCanUpdateAsync_it_should_throw_PermissionDeniedException_when_the_user_does_not_own_the_world()
  {
    UpdateWorldCommand command = new(_otherWorld.Id.ToGuid(), new UpdateWorldPayload());
    command.Contextualize(_user, _otherWorld);

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _service.EnsureCanUpdateAsync(command, _otherWorld, _cancellationToken));
    Assert.Equal(Action.Update, exception.Action);
    Assert.Equal(EntityType.World, exception.EntityType);
    Assert.Equal(_user.Id, exception.UserId);
    Assert.Equal(_otherWorld.Id.ToGuid(), exception.WorldId);
    Assert.Equal(_otherWorld.Id.ToGuid(), exception.EntityId);
  }

  [Fact(DisplayName = "EnsureCanUpdateAsync: it should throw PermissionDeniedException when the world is not the same as the context.")]
  public async Task EnsureCanUpdateAsync_it_should_throw_PermissionDeniedException_when_the_world_is_not_the_same_as_the_context()
  {
    UpdateWorldCommand command = new(_userWorld.Id.ToGuid(), new UpdateWorldPayload());
    command.Contextualize(_user, _otherWorld);

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _service.EnsureCanUpdateAsync(command, _userWorld, _cancellationToken));
    Assert.Equal(Action.Update, exception.Action);
    Assert.Equal(EntityType.World, exception.EntityType);
    Assert.Equal(_user.Id, exception.UserId);
    Assert.Equal(_otherWorld.Id.ToGuid(), exception.WorldId);
    Assert.Equal(_userWorld.Id.ToGuid(), exception.EntityId);
  }
}
