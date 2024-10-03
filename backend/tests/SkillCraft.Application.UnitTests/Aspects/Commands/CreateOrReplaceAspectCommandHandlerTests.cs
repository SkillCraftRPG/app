using FluentValidation;
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
public class CreateOrReplaceAspectCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IAspectQuerier> _aspectQuerier = new();
  private readonly Mock<IAspectRepository> _aspectRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();

  private readonly CreateOrReplaceAspectCommandHandler _handler;

  private readonly WorldMock _world = new();
  private readonly Aspect _aspect;
  private readonly AspectModel _model = new();

  public CreateOrReplaceAspectCommandHandlerTests()
  {
    _handler = new(_aspectQuerier.Object, _aspectRepository.Object, _permissionService.Object, _sender.Object);

    _aspect = new(_world.Id, new Name("oeil-de-lynx"), _world.OwnerId);
    _aspectRepository.Setup(x => x.LoadAsync(_aspect.Id, _cancellationToken)).ReturnsAsync(_aspect);

    _aspectQuerier.Setup(x => x.ReadAsync(It.IsAny<Aspect>(), _cancellationToken)).ReturnsAsync(_model);
  }

  [Theory(DisplayName = "It should create a new aspect.")]
  [InlineData(null)]
  [InlineData("5a4172dc-9ad7-43cf-9980-2538b7d1742f")]
  public async Task It_should_create_a_new_aspect(string? idValue)
  {
    CreateOrReplaceAspectPayload payload = new(" Œil-de-lynx ")
    {
      Description = "    ",
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

    bool parsed = Guid.TryParse(idValue, out Guid id);
    CreateOrReplaceAspectCommand command = new(parsed ? id : null, payload, Version: null);
    command.Contextualize(_world);

    CreateOrReplaceAspectResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_model, result.Aspect);
    Assert.True(result.Created);

    _permissionService.Verify(x => x.EnsureCanCreateAsync(command, EntityType.Aspect, _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(
      It.Is<SaveAspectCommand>(y => (!parsed || y.Aspect.EntityId == id)
        && y.Aspect.Name.Value == payload.Name.Trim()
        && y.Aspect.Description == null
        && y.Aspect.Attributes.Mandatory1 == payload.Attributes.Mandatory1
        && y.Aspect.Attributes.Mandatory2 == payload.Attributes.Mandatory2
        && y.Aspect.Attributes.Optional1 == payload.Attributes.Optional1
        && y.Aspect.Attributes.Optional2 == payload.Attributes.Optional2
        && y.Aspect.Skills.Discounted1 == payload.Skills.Discounted1
        && y.Aspect.Skills.Discounted2 == payload.Skills.Discounted2),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should replace an existing aspect.")]
  public async Task It_should_replace_an_existing_aspect()
  {
    CreateOrReplaceAspectPayload payload = new(" Œil-de-lynx ")
    {
      Description = "    ",
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

    CreateOrReplaceAspectCommand command = new(_aspect.EntityId, payload, Version: null);
    command.Contextualize(_world);

    CreateOrReplaceAspectResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_model, result.Aspect);
    Assert.False(result.Created);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Aspect && y.Id == _aspect.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(
      It.Is<SaveAspectCommand>(y => y.Aspect.Equals(_aspect)
        && y.Aspect.Name.Value == payload.Name.Trim()
        && y.Aspect.Description == null
        && y.Aspect.Attributes.Mandatory1 == payload.Attributes.Mandatory1
        && y.Aspect.Attributes.Mandatory2 == payload.Attributes.Mandatory2
        && y.Aspect.Attributes.Optional1 == payload.Attributes.Optional1
        && y.Aspect.Attributes.Optional2 == payload.Attributes.Optional2
        && y.Aspect.Skills.Discounted1 == payload.Skills.Discounted1
        && y.Aspect.Skills.Discounted2 == payload.Skills.Discounted2),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return null when updating an aspect that does not exist.")]
  public async Task It_should_return_null_when_updating_an_aspect_that_does_not_exist()
  {
    CreateOrReplaceAspectCommand command = new(Guid.Empty, new CreateOrReplaceAspectPayload("Œil-de-lynx"), Version: 0);
    command.Contextualize(_world);

    CreateOrReplaceAspectResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Null(result.Aspect);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateOrReplaceAspectPayload payload = new();
    payload.Attributes.Mandatory1 = (Attribute)(-1);

    CreateOrReplaceAspectCommand command = new(Id: null, payload, Version: null);
    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Name");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "EnumValidator" && e.PropertyName == "Attributes.Mandatory1.Value");
  }

  [Fact(DisplayName = "It should update an existing aspect.")]
  public async Task It_should_update_an_existing_aspect()
  {
    Aspect reference = new(_aspect.WorldId, _aspect.Name, _world.OwnerId, _aspect.EntityId)
    {
      Description = _aspect.Description,
      Attributes = _aspect.Attributes,
      Skills = _aspect.Skills
    };
    reference.Update(_world.OwnerId);
    _aspectRepository.Setup(x => x.LoadAsync(reference.Id, reference.Version, _cancellationToken)).ReturnsAsync(reference);

    Description description = new("Personne dotée d’une excellente vue.");
    _aspect.Description = description;
    _aspect.Update(_world.OwnerId);

    CreateOrReplaceAspectPayload payload = new(" Œil-de-lynx ")
    {
      Description = "    ",
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

    CreateOrReplaceAspectCommand command = new(_aspect.EntityId, payload, reference.Version);
    command.Contextualize(_world);

    CreateOrReplaceAspectResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_model, result.Aspect);
    Assert.False(result.Created);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Aspect && y.Id == _aspect.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(
      It.Is<SaveAspectCommand>(y => y.Aspect.Equals(_aspect)
        && y.Aspect.Name.Value == payload.Name.Trim()
        && y.Aspect.Description == description
        && y.Aspect.Attributes.Mandatory1 == payload.Attributes.Mandatory1
        && y.Aspect.Attributes.Mandatory2 == payload.Attributes.Mandatory2
        && y.Aspect.Attributes.Optional1 == payload.Attributes.Optional1
        && y.Aspect.Attributes.Optional2 == payload.Attributes.Optional2
        && y.Aspect.Skills.Discounted1 == payload.Skills.Discounted1
        && y.Aspect.Skills.Discounted2 == payload.Skills.Discounted2),
      _cancellationToken), Times.Once);
  }
}
