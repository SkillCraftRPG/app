using FluentValidation;
using FluentValidation.Results;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Storages;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Worlds.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SaveWorldCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IWorldQuerier> _worldQuerier = new();
  private readonly Mock<IWorldRepository> _worldRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly SaveWorldCommandHandler _handler;

  private readonly WorldMock _world = new();
  private readonly WorldModel _model = new();

  public SaveWorldCommandHandlerTests()
  {
    _handler = new(_permissionService.Object, _storageService.Object, _worldQuerier.Object, _worldRepository.Object);

    _worldRepository.Setup(x => x.LoadAsync(_world.Id, _cancellationToken)).ReturnsAsync(_world);
    _worldQuerier.Setup(x => x.ReadAsync(It.IsAny<World>(), _cancellationToken)).ReturnsAsync(_model);
  }

  [Theory(DisplayName = "It should create a new world.")]
  [InlineData(null)]
  [InlineData("d6e1208a-a761-4acc-a670-e6bbaeb7a32d")]
  public async Task It_should_create_a_new_world(string? idValue)
  {
    SaveWorldPayload payload = new("ungar")
    {
      Name = " Ungar ",
      Description = "    "
    };

    bool parsed = Guid.TryParse(idValue, out Guid id);
    SaveWorldCommand command = new(parsed ? id : null, payload, Version: null);
    command.Contextualize();

    SaveWorldResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_model, result.World);
    Assert.True(result.Created);

    _permissionService.Verify(x => x.EnsureCanCreateAsync(command, EntityType.World, _cancellationToken), Times.Once);

    _worldRepository.Verify(x => x.SaveAsync(
      It.Is<World>(y => (!parsed || y.EntityId == id)
        && y.Slug.Value == payload.Slug
        && y.Name != null && y.Name.Value == payload.Name.Trim()
        && y.Description == null),
      _cancellationToken), Times.Once);

    _storageService.Verify(x => x.EnsureAvailableAsync(It.Is<World>(y => !parsed || y.EntityId == id), _cancellationToken), Times.Once);
    _storageService.Verify(x => x.UpdateAsync(It.Is<World>(y => !parsed || y.EntityId == id), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should replace an existing world.")]
  public async Task It_should_replace_an_existing_world()
  {
    SaveWorldPayload payload = new("ungar")
    {
      Name = " Ungar ",
      Description = "    "
    };

    SaveWorldCommand command = new(_world.EntityId, payload, Version: null);
    command.Contextualize();

    SaveWorldResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_model, result.World);
    Assert.False(result.Created);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(command, _world, _cancellationToken), Times.Once);

    _worldRepository.Verify(x => x.SaveAsync(
      It.Is<World>(y => y.Equals(_world)
        && y.Slug.Value == payload.Slug
        && y.Name != null && y.Name.Value == payload.Name.Trim()
        && y.Description == null),
      _cancellationToken), Times.Once);

    _storageService.Verify(x => x.EnsureAvailableAsync(_world, _cancellationToken), Times.Once);
    _storageService.Verify(x => x.UpdateAsync(_world, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return null when updating an world that does not exist.")]
  public async Task It_should_return_null_when_updating_an_world_that_does_not_exist()
  {
    SaveWorldCommand command = new(Guid.Empty, new SaveWorldPayload("Œil-de-lynx"), Version: 0);
    command.Contextualize(_world);

    SaveWorldResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Null(result.World);
  }

  [Theory(DisplayName = "It should throw SlugAlreadyUsedException when the slug is already used.")]
  [InlineData(null)]
  [InlineData("add46384-a6f0-4ffc-98aa-17d4821c6b76")]
  public async Task It_should_throw_SlugAlreadyUsedException_when_the_slug_is_already_used(string? idValue)
  {
    _worldQuerier.Setup(x => x.FindIdAsync(_world.Slug, _cancellationToken)).ReturnsAsync(_world.Id);

    World? world = null;
    if (Guid.TryParse(idValue, out Guid id))
    {
      world = new(new Slug("new-world"), _world.OwnerId, new WorldId(id));
      _worldRepository.Setup(x => x.LoadAsync(world.Id, _cancellationToken)).ReturnsAsync(world);
    }

    SaveWorldPayload payload = new(_world.Slug.Value);
    SaveWorldCommand command = new(world == null ? null : id, payload, Version: null);
    command.Contextualize();

    var exception = await Assert.ThrowsAsync<SlugAlreadyUsedException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(2, exception.Ids.Count());
    Assert.Contains(_world.EntityId, exception.Ids);
    if (world != null)
    {
      Assert.Contains(id, exception.Ids);
    }
    Assert.Equal(payload.Slug, exception.Slug);
    Assert.Equal("Slug", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    SaveWorldPayload payload = new("ungar!");

    SaveWorldCommand command = new(Id: null, payload, Version: null);
    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("SlugValidator", error.ErrorCode);
    Assert.Equal("Slug", error.PropertyName);
  }

  [Fact(DisplayName = "It should update an existing world.")]
  public async Task It_should_update_an_existing_world()
  {
    World reference = new(_world.Slug, _world.OwnerId, _world.Id)
    {
      Name = _world.Name,
      Description = _world.Description
    };
    reference.Update(_world.OwnerId);
    _worldRepository.Setup(x => x.LoadAsync(reference.Id, reference.Version, _cancellationToken)).ReturnsAsync(reference);

    Description description = new("Ceci est le monde d’Ungar.");
    _world.Description = description;
    _world.Update(_world.OwnerId);

    SaveWorldPayload payload = new("new-world")
    {
      Name = " Ungar ",
      Description = "    "
    };

    SaveWorldCommand command = new(_world.EntityId, payload, reference.Version);
    command.Contextualize(_world);

    SaveWorldResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_model, result.World);
    Assert.False(result.Created);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(command, _world, _cancellationToken), Times.Once);

    _worldRepository.Verify(x => x.SaveAsync(
      It.Is<World>(y => y.Equals(_world)
        && y.Slug.Value == payload.Slug.Trim()
        && y.Name != null && y.Name.Value == payload.Name.Trim()
        && y.Description == description),
      _cancellationToken), Times.Once);

    _storageService.Verify(x => x.EnsureAvailableAsync(_world, _cancellationToken), Times.Once);
    _storageService.Verify(x => x.UpdateAsync(_world, _cancellationToken), Times.Once);
  }
}
