using FluentValidation.Results;
using MediatR;
using Moq;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Worlds.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateWorldCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ISender> _sender = new();
  private readonly Mock<IWorldQuerier> _worldQuerier = new();

  private readonly CreateWorldCommandHandler _handler;

  public CreateWorldCommandHandlerTests()
  {
    _handler = new(_sender.Object, _worldQuerier.Object);
  }

  [Fact(DisplayName = "It should save a new world.")]
  public async Task It_should_save_a_new_world()
  {
    World world = new();
    _worldQuerier.Setup(x => x.ReadAsync(It.IsAny<WorldAggregate>(), _cancellationToken)).ReturnsAsync(world);

    CreateWorldPayload payload = new("new-world")
    {
      DisplayName = "  New World  ",
      Description = "  This is the new world.  "
    };
    CreateWorldCommand command = new(payload);
    ActivityContextMock context = new();
    Assert.NotNull(context.User);
    command.Contextualize(context);
    World result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(world, result);

    _sender.Verify(x => x.Send(It.Is<SaveWorldCommand>(y => y.World.OwnerId.ToGuid() == context.User.Id
      && y.World.UniqueSlug.Value == payload.UniqueSlug
      && y.World.DisplayName != null && y.World.DisplayName.Value == payload.DisplayName.Trim()
      && y.World.Description != null && y.World.Description.Value == payload.Description.Trim()
    ), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_it_not_valid()
  {
    CreateWorldPayload payload = new("new--world");
    CreateWorldCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("SlugValidator", error.ErrorCode);
    Assert.Equal("UniqueSlug", error.PropertyName);
  }
}
