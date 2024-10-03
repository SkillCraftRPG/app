using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Worlds.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateOrReplaceWorldCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();
  private readonly Mock<IWorldQuerier> _worldQuerier = new();
  private readonly Mock<IWorldRepository> _worldRepository = new();

  private readonly CreateOrReplaceWorldCommandHandler _handler;

  private readonly WorldMock _world = new();
  private readonly WorldModel _model = new();

  public CreateOrReplaceWorldCommandHandlerTests()
  {
    _handler = new(_permissionService.Object, _sender.Object, _worldQuerier.Object, _worldRepository.Object);

    _worldRepository.Setup(x => x.LoadAsync(_world.Id, _cancellationToken)).ReturnsAsync(_world);
    _worldQuerier.Setup(x => x.ReadAsync(It.IsAny<World>(), _cancellationToken)).ReturnsAsync(_model);
  }

  [Theory(DisplayName = "It should create a new world.")]
  [InlineData(null)]
  [InlineData("d6e1208a-a761-4acc-a670-e6bbaeb7a32d")]
  public async Task It_should_create_a_new_world(string? idValue)
  {
    CreateOrReplaceWorldPayload payload = new("ungar")
    {
      Name = " Ungar ",
      Description = "    "
    };

    bool parsed = Guid.TryParse(idValue, out Guid id);
    CreateOrReplaceWorldCommand command = new(parsed ? id : null, payload, Version: null);
    command.Contextualize();

    CreateOrReplaceWorldResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_model, result.World);
    Assert.True(result.Created);

    _permissionService.Verify(x => x.EnsureCanCreateAsync(command, EntityType.World, _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(
      It.Is<SaveWorldCommand>(y => (!parsed || y.World.EntityId == id)
        && y.World.Slug.Value == payload.Slug.Trim()
        && y.World.Name != null && y.World.Name.Value == payload.Name.Trim()
        && y.World.Description == null),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should replace an existing world.")]
  public async Task It_should_replace_an_existing_world()
  {
    CreateOrReplaceWorldPayload payload = new("ungar")
    {
      Name = " Ungar ",
      Description = "    "
    };

    CreateOrReplaceWorldCommand command = new(_world.EntityId, payload, Version: null);
    command.Contextualize();

    CreateOrReplaceWorldResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_model, result.World);
    Assert.False(result.Created);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(command, _world, _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(
      It.Is<SaveWorldCommand>(y => y.World.Equals(_world)
        && y.World.Slug.Value == payload.Slug
        && y.World.Name != null && y.World.Name.Value == payload.Name.Trim()
        && y.World.Description == null),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return null when updating an world that does not exist.")]
  public async Task It_should_return_null_when_updating_an_world_that_does_not_exist()
  {
    CreateOrReplaceWorldCommand command = new(Guid.Empty, new CreateOrReplaceWorldPayload("ungar"), Version: 0);
    command.Contextualize(_world);

    CreateOrReplaceWorldResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Null(result.World);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateOrReplaceWorldPayload payload = new("ungar!");

    CreateOrReplaceWorldCommand command = new(Id: null, payload, Version: null);
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

    CreateOrReplaceWorldPayload payload = new("new-world")
    {
      Name = " Ungar ",
      Description = "    "
    };

    CreateOrReplaceWorldCommand command = new(_world.EntityId, payload, reference.Version);
    command.Contextualize(_world);

    CreateOrReplaceWorldResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_model, result.World);
    Assert.False(result.Created);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(command, _world, _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(
      It.Is<SaveWorldCommand>(y => y.World.Equals(_world)
        && y.World.Slug.Value == payload.Slug
        && y.World.Name != null && y.World.Name.Value == payload.Name.Trim()
        && y.World.Description == description),
      _cancellationToken), Times.Once);
  }
}
