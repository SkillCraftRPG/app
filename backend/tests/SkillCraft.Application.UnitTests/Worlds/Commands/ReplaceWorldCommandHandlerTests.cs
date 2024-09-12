using FluentValidation.Results;
using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Worlds.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class ReplaceWorldCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();
  private readonly Mock<IWorldQuerier> _worldQuerier = new();
  private readonly Mock<IWorldRepository> _worldRepository = new();

  private readonly ReplaceWorldCommandHandler _handler;

  public ReplaceWorldCommandHandlerTests()
  {
    _handler = new(_permissionService.Object, _sender.Object, _worldQuerier.Object, _worldRepository.Object);
  }

  [Fact(DisplayName = "It should replace an existing world.")]
  public async Task It_should_replace_an_existing_world()
  {
    World reference = new(new Slug("new-world"), UserId.NewId());
    long version = reference.Version;
    _worldRepository.Setup(x => x.LoadAsync(reference.Id, version, _cancellationToken)).ReturnsAsync(reference);

    World world = new(reference.Slug, reference.OwnerId, reference.Id);
    _worldRepository.Setup(x => x.LoadAsync(world.Id, _cancellationToken)).ReturnsAsync(world);

    Description description = new("This is the new world.");
    world.Description = description;
    world.Update(world.OwnerId);

    ReplaceWorldPayload payload = new("new-world")
    {
      Name = " The New World ",
      Description = "    "
    };
    ReplaceWorldCommand command = new(world.Id.ToGuid(), payload, version);
    ActivityContextMock.Contextualize(command);

    WorldModel model = new();
    _worldQuerier.Setup(x => x.ReadAsync(world, _cancellationToken)).ReturnsAsync(model);

    WorldModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(command, world, _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveWorldCommand>(y => y.World.Equals(world)
      && y.World.Name != null && y.World.Name.Value == payload.Name.Trim()
      && y.World.Description == description), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return null when the world could not be found.")]
  public async Task It_should_return_null_when_the_world_could_not_be_found()
  {
    ReplaceWorldPayload payload = new("new-slug");
    ReplaceWorldCommand command = new(Guid.Empty, payload, Version: null);

    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    ReplaceWorldPayload payload = new("new-world!");
    ReplaceWorldCommand command = new(Guid.Empty, payload, Version: null);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("SlugValidator", error.ErrorCode);
    Assert.Equal("Slug", error.PropertyName);
    Assert.Equal(payload.Slug, error.AttemptedValue);
  }
}
