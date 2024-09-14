using FluentValidation.Results;
using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Worlds.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateWorldCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();
  private readonly Mock<IWorldQuerier> _worldQuerier = new();

  private readonly CreateWorldCommandHandler _handler;

  public CreateWorldCommandHandlerTests()
  {
    _handler = new(_permissionService.Object, _sender.Object, _worldQuerier.Object);
  }

  [Fact(DisplayName = "It should create a new world.")]
  public async Task It_should_create_a_new_world()
  {
    CreateWorldPayload payload = new("new-world")
    {
      Name = " New World ",
      Description = "    "
    };
    CreateWorldCommand command = new(payload);
    command.Contextualize();

    WorldModel model = new();
    _worldQuerier.Setup(x => x.ReadAsync(It.IsAny<World>(), _cancellationToken)).ReturnsAsync(model);

    WorldModel result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(result, model);

    _permissionService.Verify(x => x.EnsureCanCreateAsync(command, EntityType.World, _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveWorldCommand>(y => y.World.Slug.Value == payload.Slug
      && y.World.Name != null && y.World.Name.Value == payload.Name.Trim()
      && y.World.Description == null
      && y.World.OwnerId == command.GetUserId()), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateWorldPayload payload = new("new-world!");
    CreateWorldCommand command = new(payload);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("SlugValidator", error.ErrorCode);
    Assert.Equal("Slug", error.PropertyName);
    Assert.Equal(payload.Slug, error.AttemptedValue);
  }
}
