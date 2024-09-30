﻿using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Storages;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Aspects;
using SkillCraft.Domain;
using SkillCraft.Domain.Aspects;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Application.Aspects.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class UpdateAspectCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IAspectQuerier> _aspectQuerier = new();
  private readonly Mock<IAspectRepository> _aspectRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly UpdateAspectCommandHandler _handler;

  private readonly WorldMock _world = new();

  public UpdateAspectCommandHandlerTests()
  {
    _handler = new(_aspectQuerier.Object, _aspectRepository.Object, _permissionService.Object, _storageService.Object);
  }

  [Fact(DisplayName = "It should return null when the aspect could not be found.")]
  public async Task It_should_return_null_when_the_aspect_could_not_be_found()
  {
    UpdateAspectPayload payload = new();
    UpdateAspectCommand command = new(Guid.Empty, payload);
    command.Contextualize(_world);

    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    UpdateAspectPayload payload = new()
    {
      Skills = new SkillsModel
      {
        Discounted1 = Skill.Perception,
        Discounted2 = Skill.Perception
      }
    };
    UpdateAspectCommand command = new(Guid.Empty, payload);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "SkillsValidator" && e.PropertyName == "Discounted1" && e.AttemptedValue.Equals(payload.Skills.Discounted1));
    Assert.Contains(exception.Errors, e => e.ErrorCode == "SkillsValidator" && e.PropertyName == "Discounted1" && e.AttemptedValue.Equals(payload.Skills.Discounted1));
  }

  [Fact(DisplayName = "It should update an existing aspect.")]
  public async Task It_should_update_an_existing_aspect()
  {
    Aspect aspect = new(_world.Id, new Name("oeil-de-lynx"), _world.OwnerId)
    {
      Description = new Description("Doté d’une excellente vision…")
    };
    aspect.Update(_world.OwnerId);
    _aspectRepository.Setup(x => x.LoadAsync(aspect.Id, _cancellationToken)).ReturnsAsync(aspect);

    UpdateAspectPayload payload = new()
    {
      Name = " Œil-de-lynx ",
      Description = new Change<string>("    "),
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
    UpdateAspectCommand command = new(aspect.EntityId, payload);
    command.Contextualize(_world);

    AspectModel model = new();
    _aspectQuerier.Setup(x => x.ReadAsync(aspect, _cancellationToken)).ReturnsAsync(model);

    AspectModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Aspect && y.Id == aspect.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);

    _aspectRepository.Verify(x => x.SaveAsync(
      It.Is<Aspect>(y => y.Equals(aspect) && y.Name.Value == payload.Name.Trim() && y.Description == null
        && y.Attributes.Mandatory1 == payload.Attributes.Mandatory1 && y.Attributes.Mandatory2 == payload.Attributes.Mandatory2
        && y.Attributes.Optional1 == payload.Attributes.Optional1 && y.Attributes.Optional2 == payload.Attributes.Optional2
        && y.Skills.Discounted1 == payload.Skills.Discounted1 && y.Skills.Discounted2 == payload.Skills.Discounted2),
      _cancellationToken), Times.Once);

    _storageService.Verify(x => x.EnsureAvailableAsync(
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Aspect && y.Id == aspect.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);
    _storageService.Verify(x => x.UpdateAsync(
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Aspect && y.Id == aspect.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);
  }
}
