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
public class UpdateWorldCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();
  private readonly Mock<IWorldQuerier> _worldQuerier = new();
  private readonly Mock<IWorldRepository> _worldRepository = new();

  private readonly UpdateWorldCommandHandler _handler;

  public UpdateWorldCommandHandlerTests()
  {
    _handler = new(_permissionService.Object, _sender.Object, _worldQuerier.Object, _worldRepository.Object);
  }

  [Fact(DisplayName = "It should return null when the world could not be found.")]
  public async Task It_should_return_null_when_the_world_could_not_be_found()
  {
    UpdateWorldPayload payload = new();
    UpdateWorldCommand command = new(Guid.Empty, payload);

    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    UpdateWorldPayload payload = new()
    {
      Slug = "new-world!"
    };
    UpdateWorldCommand command = new(Guid.Empty, payload);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("SlugValidator", error.ErrorCode);
    Assert.Equal("Slug", error.PropertyName);
    Assert.Equal(payload.Slug, error.AttemptedValue);
  }

  [Fact(DisplayName = "It should update an existing world.")]
  public async Task It_should_update_an_existing_world()
  {
    World world = new(new Slug("new-world"), UserId.NewId())
    {
      Description = new Description("This is the new world.")
    };
    world.Update(world.OwnerId);
    _worldRepository.Setup(x => x.LoadAsync(world.Id, _cancellationToken)).ReturnsAsync(world);

    UpdateWorldPayload payload = new()
    {
      Name = new Change<string>(" The New World "),
      Description = new Change<string>("    ")
    };
    UpdateWorldCommand command = new(world.Id.ToGuid(), payload);
    command.Contextualize();

    WorldModel model = new();
    _worldQuerier.Setup(x => x.ReadAsync(world, _cancellationToken)).ReturnsAsync(model);

    WorldModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(command, world, _cancellationToken), Times.Once);

    Assert.NotNull(payload.Name.Value);
    _sender.Verify(x => x.Send(
      It.Is<SaveWorldCommand>(y => y.World.Equals(world)
        && y.World.Slug == world.Slug
        && y.World.Name != null && y.World.Name.Value == payload.Name.Value.Trim()
        && y.World.Description == null),
      _cancellationToken), Times.Once);
  }
}
