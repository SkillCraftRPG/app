using Bogus;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Users;
using Moq;
using SkillCraft.Application.Aspects.Commands;
using SkillCraft.Application.Castes.Commands;
using SkillCraft.Application.Customizations.Commands;
using SkillCraft.Application.Educations.Commands;
using SkillCraft.Application.Languages.Commands;
using SkillCraft.Application.Lineages.Queries;
using SkillCraft.Application.Personalities.Commands;
using SkillCraft.Application.Settings;
using SkillCraft.Application.Worlds;
using SkillCraft.Application.Worlds.Commands;
using SkillCraft.Application.Worlds.Queries;
using SkillCraft.Contracts.Aspects;
using SkillCraft.Contracts.Castes;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Contracts.Educations;
using SkillCraft.Contracts.Languages;
using SkillCraft.Contracts.Personalities;
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
  private readonly Mock<IPermissionQuerier> _permissionQuerier = new();
  private readonly Mock<IWorldQuerier> _worldQuerier = new();

  private readonly PermissionService _service;

  private readonly User _otherUser;
  private readonly UserMock _user;
  private readonly WorldModel _world;

  private WorldId WorldId => new(_world.Id);

  public PermissionServiceTests()
  {
    _service = new(_accountSettings, _permissionQuerier.Object, _worldQuerier.Object);

    _otherUser = new(_faker.Internet.UserName())
    {
      Id = Guid.NewGuid(),
      Email = new Email(_faker.Internet.Email()),
      FirstName = _faker.Name.FirstName(),
      LastName = _faker.Name.LastName(),
      Picture = _faker.Internet.Avatar()
    };
    _user = new(_faker);
    _world = new(new Actor(_user), "ungar")
    {
      Id = Guid.NewGuid()
    };
  }

  #region EnsureCanAsync(Entity)
  [Fact(DisplayName = "EnsureCanAsync(Entity): it should succeed when the user has the permission.")]
  public async Task EnsureCanAsyncEntity_it_should_succeed_when_the_user_has_the_permission()
  {
    UpdateCustomizationCommand command = new(Guid.NewGuid(), new UpdateCustomizationPayload());
    command.Contextualize(_otherUser, _world);

    EntityMetadata entity = new(WorldId, new EntityKey(EntityType.Customization, command.Id), size: 9);
    _permissionQuerier.Setup(x => x.HasAsync(_otherUser, _world, Action.Update, entity.Type, entity.Id, _cancellationToken)).ReturnsAsync(true);

    await _service.EnsureCanUpdateAsync(command, entity, _cancellationToken);

    _permissionQuerier.Verify(x => x.HasAsync(_otherUser, _world, Action.Update, entity.Type, entity.Id, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "EnsureCanAsync(Entity): it should succeed when the user is the world owner.")]
  public async Task EnsureCanAsyncEntity_it_should_succeed_when_the_user_is_the_world_owner()
  {
    UpdateAspectCommand command = new(Guid.NewGuid(), new UpdateAspectPayload());
    command.Contextualize(_user, _world);

    EntityMetadata entity = new(WorldId, new EntityKey(EntityType.Aspect, command.Id), size: 7);
    await _service.EnsureCanUpdateAsync(command, entity, _cancellationToken);

    EnsurePermissionQuerierHasAsyncHasNeverBeenCalled();
  }

  [Fact(DisplayName = "EnsureCanAsync(Entity): it should throw ArgumentException when the entity type is 'World'.")]
  public async Task EnsureCanAsyncEntity_it_should_throw_ArgumentException_when_the_entity_type_is_World()
  {
    UpdateWorldCommand command = new(Guid.NewGuid(), new UpdateWorldPayload());
    command.Contextualize(_user, _world);

    EntityMetadata entity = new(WorldId, new EntityKey(EntityType.World, _world.Id), size: 5);
    var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await _service.EnsureCanUpdateAsync(command, entity, _cancellationToken));
    Assert.StartsWith("The entity type must not be 'World'.", exception.Message);
    Assert.Equal("entity", exception.ParamName);
  }

  [Fact(DisplayName = "EnsureCanAsync(Entity): it should throw PermissionDeniedException when the entity does not reside in the world.")]
  public async Task EnsureCanAsyncEntity_it_should_throw_PermissionDeniedException_when_the_entity_does_not_reside_in_the_world()
  {
    UpdatePersonalityCommand command = new(Guid.NewGuid(), new UpdatePersonalityPayload());
    command.Contextualize(_user, _world);

    EntityMetadata entity = new(WorldId.NewId(), new EntityKey(EntityType.Personality, command.Id), size: 7);
    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _service.EnsureCanUpdateAsync(command, entity, _cancellationToken));
    Assert.Equal(Action.Update, exception.Action);
    Assert.Equal(entity.Type, exception.EntityType);
    Assert.Equal(_user.Id, exception.UserId);
    Assert.Equal(_world.Id, exception.WorldId);
    Assert.Equal(entity.Id, exception.EntityId);

    EnsurePermissionQuerierHasAsyncHasNeverBeenCalled();
  }

  [Fact(DisplayName = "EnsureCanAsync(Entity): it should throw PermissionDeniedException when the user does not have the permission.")]
  public async Task EnsureCanAsyncEntity_it_should_throw_PermissionDeniedException_when_the_user_does_not_have_the_permission()
  {
    UpdateLanguageCommand command = new(Guid.NewGuid(), new UpdateLanguagePayload());
    command.Contextualize(_otherUser, _world);

    EntityMetadata entity = new(WorldId, new EntityKey(EntityType.Language, command.Id), size: 9);
    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _service.EnsureCanUpdateAsync(command, entity, _cancellationToken));
    Assert.Equal(Action.Update, exception.Action);
    Assert.Equal(entity.Type, exception.EntityType);
    Assert.Equal(_otherUser.Id, exception.UserId);
    Assert.Equal(_world.Id, exception.WorldId);
    Assert.Equal(entity.Id, exception.EntityId);

    _permissionQuerier.Verify(x => x.HasAsync(_otherUser, _world, Action.Update, entity.Type, entity.Id, _cancellationToken), Times.Once);
  }
  #endregion

  #region EnsureCanAsync(World)
  [Fact(DisplayName = "EnsureCanAsync(World): it should succeed when the user has the permission.")]
  public async Task EnsureCanAsyncWorld_it_should_succeed_when_the_user_has_the_permission()
  {
    World world = new(new Slug(_world.Slug), new UserId(_world.Owner.Id), new WorldId(_world.Id));

    ReplaceWorldCommand command = new(_world.Id, new ReplaceWorldPayload(_world.Slug), Version: null);
    command.Contextualize(_otherUser);

    _permissionQuerier.Setup(x => x.HasAsync(
      _otherUser,
      It.Is<WorldModel>(y => y.Id == _world.Id && y.Owner.Id == _world.Owner.Id),
      Action.Update,
      EntityType.World,
      _world.Id,
      _cancellationToken)).ReturnsAsync(true);

    await _service.EnsureCanUpdateAsync(command, world, _cancellationToken);

    _permissionQuerier.Verify(x => x.HasAsync(
      _otherUser,
      It.Is<WorldModel>(y => y.Id == _world.Id && y.Owner.Id == _world.Owner.Id),
      Action.Update,
      EntityType.World,
      _world.Id,
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "EnsureCanAsync(World): it should succeed when the user is the world owner.")]
  public async Task EnsureCanAsyncWorld_it_should_succeed_when_the_user_is_the_world_owner()
  {
    World world = new(new Slug(_world.Slug), new UserId(_world.Owner.Id), new WorldId(_world.Id));

    ReplaceWorldCommand command = new(_world.Id, new ReplaceWorldPayload(_world.Slug), Version: null);
    command.Contextualize(_user, _world);

    await _service.EnsureCanUpdateAsync(command, world, _cancellationToken);

    EnsurePermissionQuerierHasAsyncHasNeverBeenCalled();
  }

  [Fact(DisplayName = "EnsureCanAsync(World): it should throw PermissionDeniedException when the user does not have the permission.")]
  public async Task EnsureCanAsyncWorld_it_should_throw_PermissionDeniedException_when_the_user_does_not_have_the_permission()
  {
    World world = new(new Slug(_world.Slug), new UserId(_world.Owner.Id), new WorldId(_world.Id));

    UpdateWorldCommand command = new(_world.Id, new UpdateWorldPayload());
    command.Contextualize(_otherUser);

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _service.EnsureCanUpdateAsync(command, world, _cancellationToken));
    Assert.Equal(Action.Update, exception.Action);
    Assert.Equal(EntityType.World, exception.EntityType);
    Assert.Equal(_otherUser.Id, exception.UserId);
    Assert.Equal(_world.Id, exception.WorldId);
    Assert.Equal(_world.Id, exception.EntityId);

    _permissionQuerier.Verify(x => x.HasAsync(
      _otherUser,
      It.Is<WorldModel>(y => y.Id == _world.Id && y.Owner.Id == _world.Owner.Id),
      Action.Update,
      EntityType.World,
      _world.Id,
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "EnsureCanAsync(World): it should throw PermissionDeniedException when the worlds are not the same.")]
  public async Task EnsureCanAsyncWorld_it_should_throw_PermissionDeniedException_when_the_worlds_are_not_the_same()
  {
    World world = new(new Slug("hyrule"), UserId.NewId());

    UpdateWorldCommand command = new(world.Id.ToGuid(), new UpdateWorldPayload());
    command.Contextualize(_user, _world);

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _service.EnsureCanUpdateAsync(command, world, _cancellationToken));
    Assert.Equal(Action.Update, exception.Action);
    Assert.Equal(EntityType.World, exception.EntityType);
    Assert.Equal(_user.Id, exception.UserId);
    Assert.Equal(_world.Id, exception.WorldId);
    Assert.Equal(command.Id, exception.EntityId);

    EnsurePermissionQuerierHasAsyncHasNeverBeenCalled();
  }
  #endregion

  #region EnsureCanCreateAsync
  [Fact(DisplayName = "EnsureCanCreateAsync: it should succeed when the user has the permission.")]
  public async Task EnsureCanCreateAsync_it_should_succeed_when_the_user_has_the_permission()
  {
    CreateEducationCommand command = new(new CreateEducationPayload("Classique"));
    command.Contextualize(_otherUser, _world);

    _permissionQuerier.Setup(x => x.HasAsync(_otherUser, _world, Action.Create, EntityType.Education, null, _cancellationToken)).ReturnsAsync(true);

    await _service.EnsureCanCreateAsync(command, EntityType.Education, _cancellationToken);

    _permissionQuerier.Verify(x => x.HasAsync(_otherUser, _world, Action.Create, EntityType.Education, null, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "EnsureCanCreateAsync: it should succeed when the user is the world owner.")]
  public async Task EnsureCanCreateAsync_it_should_succeed_when_the_user_is_the_world_owner()
  {
    CreateCasteCommand command = new(new CreateCastePayload("Artisan"));
    command.Contextualize(_user, _world);

    await _service.EnsureCanCreateAsync(command, EntityType.Caste, _cancellationToken);

    EnsurePermissionQuerierHasAsyncHasNeverBeenCalled();
  }

  [Fact(DisplayName = "EnsureCanCreateAsync: it should succeed when the world limit has not been reached.")]
  public async Task EnsureCanCreateAsync_it_should_succeed_when_the_world_limit_has_not_been_reached()
  {
    CreateWorldCommand command = new(new CreateWorldPayload("ungar"));
    command.Contextualize(_user);

    _worldQuerier.Setup(x => x.CountOwnedAsync(command.GetUserId(), _cancellationToken)).ReturnsAsync(_accountSettings.WorldLimit - 1);

    await _service.EnsureCanCreateAsync(command, EntityType.World, _cancellationToken);
  }

  [Fact(DisplayName = "EnsureCanCreateAsync: it should throw PermissionDeniedException when the user does not have the permission.")]
  public async Task EnsureCanCreateAsync_it_should_throw_PermissionDeniedException_when_the_user_does_not_have_the_permission()
  {
    CreateCasteCommand command = new(new CreateCastePayload("Artisan"));
    command.Contextualize(_otherUser, _world);

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _service.EnsureCanCreateAsync(command, EntityType.Caste, _cancellationToken));
    Assert.Equal(Action.Create, exception.Action);
    Assert.Equal(EntityType.Caste, exception.EntityType);
    Assert.Equal(_otherUser.Id, exception.UserId);
    Assert.Equal(_world.Id, exception.WorldId);
    Assert.Null(exception.EntityId);

    _permissionQuerier.Verify(x => x.HasAsync(_otherUser, _world, Action.Create, EntityType.Caste, null, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "EnsureCanCreateAsync: it should throw PermissionDeniedException when the world limit has been reached.")]
  public async Task EnsureCanCreateAsync_it_should_throw_PermissionDeniedException_when_the_world_limit_has_been_reached()
  {
    CreateWorldCommand command = new(new CreateWorldPayload("ungar"));
    command.Contextualize(_user);

    _worldQuerier.Setup(x => x.CountOwnedAsync(command.GetUserId(), _cancellationToken)).ReturnsAsync(_accountSettings.WorldLimit);

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _service.EnsureCanCreateAsync(command, EntityType.World, _cancellationToken));
    Assert.Equal(Action.Create, exception.Action);
    Assert.Equal(EntityType.World, exception.EntityType);
    Assert.Equal(_user.Id, exception.UserId);
    Assert.Null(exception.WorldId);
    Assert.Null(exception.EntityId);
  }
  #endregion

  #region EnsureCanPreviewAsync(EntityMetadata)
  #endregion

  #region EnsureCanPreviewAsync(EntityType)
  [Fact(DisplayName = "EnsureCanPreviewAsync(EntityType): it should succeed when the user has the permission.")]
  public async Task EnsureCanPreviewAsyncEntityType_it_should_succeed_when_the_user_has_the_permission()
  {
    ReadLineageQuery query = new(Guid.NewGuid());
    query.Contextualize(_otherUser, _world);

    _permissionQuerier.Setup(x => x.HasAsync(_otherUser, _world, Action.Preview, EntityType.Lineage, null, _cancellationToken)).ReturnsAsync(true);

    await _service.EnsureCanPreviewAsync(query, EntityType.Lineage, _cancellationToken);

    _permissionQuerier.Verify(x => x.HasAsync(_otherUser, _world, Action.Preview, EntityType.Lineage, null, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "EnsureCanPreviewAsync(EntityType): it should succeed when the user is the world owner.")]
  public async Task EnsureCanPreviewAsyncEntityType_it_should_succeed_when_the_user_is_the_world_owner()
  {
    ReadLineageQuery query = new(Guid.NewGuid());
    query.Contextualize(_user, _world);

    await _service.EnsureCanPreviewAsync(query, EntityType.Lineage, _cancellationToken);

    EnsurePermissionQuerierHasAsyncHasNeverBeenCalled();
  }

  [Fact(DisplayName = "EnsureCanPreviewAsync(EntityType): it should throw ArgumentOutOfRangeException when the type is world.")]
  public async Task EnsureCanPreviewAsyncEntityType_it_should_throw_ArgumentOutOfRangeException_when_the_type_is_world()
  {
    ReadWorldQuery query = new(_world.Id, _world.Slug);
    query.Contextualize(_user);

    var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await _service.EnsureCanPreviewAsync(query, EntityType.World, _cancellationToken));
    Assert.Equal("entityType", exception.ParamName);
  }

  [Fact(DisplayName = "EnsureCanPreviewAsync(EntityType): it should throw PermissionDeniedException when the user does not have the permission.")]
  public async Task EnsureCanPreviewAsyncEntityType_it_should_throw_PermissionDeniedException_when_the_user_does_not_have_the_permission()
  {
    ReadLineageQuery query = new(Guid.NewGuid());
    query.Contextualize(_otherUser, _world);

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _service.EnsureCanPreviewAsync(query, EntityType.Lineage, _cancellationToken));
    Assert.Equal(Action.Preview, exception.Action);
    Assert.Equal(EntityType.Lineage, exception.EntityType);
    Assert.Equal(_otherUser.Id, exception.UserId);
    Assert.Equal(_world.Id, exception.WorldId);
    Assert.Null(exception.EntityId);

    _permissionQuerier.Verify(x => x.HasAsync(_otherUser, _world, Action.Preview, EntityType.Lineage, null, _cancellationToken), Times.Once);
  }
  #endregion

  #region EnsureCanPreviewAsync(World)
  [Fact(DisplayName = "EnsureCanPreviewAsync(World): it should succeed when the user is the world owner.")]
  public async Task EnsureCanPreviewAsyncWorld_it_should_succeed_when_the_user_is_the_world_owner()
  {
    ReadWorldQuery query = new(_world.Id, _world.Slug);
    query.Contextualize(_user, _world);

    await _service.EnsureCanPreviewAsync(query, _world, _cancellationToken);

    EnsurePermissionQuerierHasAsyncHasNeverBeenCalled();
  }

  [Fact(DisplayName = "EnsureCanPreviewAsync(World): it should throw PermissionDeniedException when the user is not the world owner.")]
  public async Task EnsureCanPreviewAsyncWorld_it_should_throw_PermissionDeniedException_when_the_user_is_not_the_world_owner()
  {
    ReadWorldQuery query = new(_world.Id, _world.Slug);
    query.Contextualize(_otherUser);

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _service.EnsureCanPreviewAsync(query, _world, _cancellationToken));
    Assert.Equal(Action.Preview, exception.Action);
    Assert.Equal(EntityType.World, exception.EntityType);
    Assert.Equal(_otherUser.Id, exception.UserId);
    Assert.Null(exception.WorldId);
    Assert.Equal(_world.Id, exception.EntityId);

    EnsurePermissionQuerierHasAsyncHasNeverBeenCalled();
  }

  [Fact(DisplayName = "EnsureCanPreviewAsync(World): it should throw PermissionDeniedException when the worlds are not the same.")]
  public async Task EnsureCanPreviewAsyncWorld_it_should_throw_PermissionDeniedException_when_the_worlds_are_not_the_same()
  {
    WorldModel world = new(new Actor(_otherUser), "hyrule")
    {
      Id = Guid.NewGuid()
    };

    ReadWorldQuery query = new(world.Id, world.Slug);
    query.Contextualize(_otherUser, _world);

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _service.EnsureCanPreviewAsync(query, world, _cancellationToken));
    Assert.Equal(Action.Preview, exception.Action);
    Assert.Equal(EntityType.World, exception.EntityType);
    Assert.Equal(_otherUser.Id, exception.UserId);
    Assert.Equal(_world.Id, exception.WorldId);
    Assert.Equal(world.Id, exception.EntityId);

    EnsurePermissionQuerierHasAsyncHasNeverBeenCalled();
  }
  #endregion

  private void EnsurePermissionQuerierHasAsyncHasNeverBeenCalled()
  {
    _permissionQuerier.Verify(x => x.HasAsync(It.IsAny<User>(), It.IsAny<WorldModel>(), It.IsAny<Action>(), It.IsAny<EntityType>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
  }
}
