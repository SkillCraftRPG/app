using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Aspects;
using SkillCraft.Domain;
using SkillCraft.Domain.Aspects;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Application.Aspects.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class ReplaceAspectCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IAspectQuerier> _aspectQuerier = new();
  private readonly Mock<IAspectRepository> _aspectRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();

  private readonly ReplaceAspectCommandHandler _handler;

  private readonly WorldMock _world = new();

  public ReplaceAspectCommandHandlerTests()
  {
    _handler = new(_aspectQuerier.Object, _aspectRepository.Object, _permissionService.Object, _sender.Object);
  }

  [Fact(DisplayName = "It should replace an existing aspect.")]
  public async Task It_should_replace_an_existing_aspect()
  {
    Aspect aspect = new(_world.Id, new Name("oeil-de-lynx"), _world.OwnerId);
    _aspectRepository.Setup(x => x.LoadAsync(aspect.Id, _cancellationToken)).ReturnsAsync(aspect);

    ReplaceAspectPayload payload = new(" Œil-de-lynx ")
    {
      Description = "  Doté d’une excellente vision…  ",
      Attributes = new AttributeSelectionModel
      {
        Mandatory1 = Attribute.Coordination,
        Mandatory2 = Attribute.Sensitivity,
        Optional1 = Attribute.Intellect,
        Optional2 = Attribute.Presence
      },
      Skills = new SkillsModel
      {
        Discounted1 = Skill.Orientation,
        Discounted2 = Skill.Perception
      }
    };
    ReplaceAspectCommand command = new(aspect.EntityId, payload, Version: null);
    command.Contextualize(_world);

    AspectModel model = new();
    _aspectQuerier.Setup(x => x.ReadAsync(aspect, _cancellationToken)).ReturnsAsync(model);

    AspectModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Key.Type == EntityType.Aspect && y.Key.Id == aspect.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveAspectCommand>(y => y.Aspect.Equals(aspect)
      && y.Aspect.Name.Value == payload.Name.Trim()
      && y.Aspect.Description != null && y.Aspect.Description.Value == payload.Description.Trim()
      && y.Aspect.Attributes.Mandatory1 == payload.Attributes.Mandatory1
      && y.Aspect.Attributes.Mandatory2 == payload.Attributes.Mandatory2
      && y.Aspect.Attributes.Optional1 == payload.Attributes.Optional1
      && y.Aspect.Attributes.Optional2 == payload.Attributes.Optional2
      && y.Aspect.Skills.Discounted1 == payload.Skills.Discounted1
      && y.Aspect.Skills.Discounted2 == payload.Skills.Discounted2), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return null when the aspect could not be found.")]
  public async Task It_should_return_null_when_the_aspect_could_not_be_found()
  {
    ReplaceAspectPayload payload = new("Œil-de-lynx");
    ReplaceAspectCommand command = new(Guid.Empty, payload, Version: null);
    command.Contextualize(_world);

    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    ReplaceAspectPayload payload = new("Œil-de-lynx")
    {
      Attributes = new AttributeSelectionModel
      {
        Mandatory1 = Attribute.Coordination,
        Mandatory2 = Attribute.Sensitivity,
        Optional1 = Attribute.Coordination,
        Optional2 = Attribute.Presence
      }
    };
    ReplaceAspectCommand command = new(Guid.Empty, payload, Version: null);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "AttributesValidator" && e.PropertyName == "Mandatory1" && e.AttemptedValue.Equals(Attribute.Coordination));
    Assert.Contains(exception.Errors, e => e.ErrorCode == "AttributesValidator" && e.PropertyName == "Optional1" && e.AttemptedValue.Equals(Attribute.Coordination));
  }

  [Fact(DisplayName = "It should update an existing aspect from a reference.")]
  public async Task It_should_update_an_existing_aspect_from_a_reference()
  {
    Aspect reference = new(_world.Id, new Name("OEil-de-lynx"), _world.OwnerId);
    long version = reference.Version;
    _aspectRepository.Setup(x => x.LoadAsync(reference.Id, version, _cancellationToken)).ReturnsAsync(reference);

    Aspect aspect = new(_world.Id, reference.Name, _world.OwnerId, reference.EntityId);
    _aspectRepository.Setup(x => x.LoadAsync(aspect.Id, _cancellationToken)).ReturnsAsync(aspect);

    Description description = new("Doté d’une excellente vision…");
    aspect.Description = description;
    aspect.Update(_world.OwnerId);

    ReplaceAspectPayload payload = new(" Œil-de-lynx ")
    {
      Attributes = new AttributeSelectionModel
      {
        Mandatory1 = Attribute.Coordination,
        Mandatory2 = Attribute.Sensitivity,
        Optional1 = Attribute.Intellect,
        Optional2 = Attribute.Presence
      },
      Skills = new SkillsModel
      {
        Discounted1 = Skill.Orientation,
        Discounted2 = Skill.Perception
      }
    };
    ReplaceAspectCommand command = new(aspect.EntityId, payload, version);
    command.Contextualize(_world);

    AspectModel model = new();
    _aspectQuerier.Setup(x => x.ReadAsync(aspect, _cancellationToken)).ReturnsAsync(model);

    AspectModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Key.Type == EntityType.Aspect && y.Key.Id == aspect.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveAspectCommand>(y => y.Aspect.Equals(aspect)
      && y.Aspect.Name.Value == payload.Name.Trim()
      && y.Aspect.Description != null && y.Aspect.Description == description
      && y.Aspect.Attributes.Mandatory1 == payload.Attributes.Mandatory1
      && y.Aspect.Attributes.Mandatory2 == payload.Attributes.Mandatory2
      && y.Aspect.Attributes.Optional1 == payload.Attributes.Optional1
      && y.Aspect.Attributes.Optional2 == payload.Attributes.Optional2
      && y.Aspect.Skills.Discounted1 == payload.Skills.Discounted1
      && y.Aspect.Skills.Discounted2 == payload.Skills.Discounted2), _cancellationToken), Times.Once);
  }
}
