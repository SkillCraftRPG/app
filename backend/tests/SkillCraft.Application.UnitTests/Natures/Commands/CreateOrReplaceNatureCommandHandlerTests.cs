using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Natures;
using SkillCraft.Domain;
using SkillCraft.Domain.Natures;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Application.Natures.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateOrReplaceNatureCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<INatureQuerier> _natureQuerier = new();
  private readonly Mock<INatureRepository> _natureRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();

  private readonly CreateOrReplaceNatureCommandHandler _handler;

  private readonly WorldMock _world = new();
  private readonly Nature _nature;
  private readonly NatureModel _model = new();

  public CreateOrReplaceNatureCommandHandlerTests()
  {
    _handler = new(_natureQuerier.Object, _natureRepository.Object, _permissionService.Object, _sender.Object);

    _nature = new(_world.Id, new Name("courrouce"), _world.OwnerId);
    _natureRepository.Setup(x => x.LoadAsync(_nature.Id, _cancellationToken)).ReturnsAsync(_nature);

    _natureQuerier.Setup(x => x.ReadAsync(It.IsAny<Nature>(), _cancellationToken)).ReturnsAsync(_model);
  }

  [Theory(DisplayName = "It should create a new nature.")]
  [InlineData(null)]
  [InlineData("fdf958c0-06be-4b68-a405-b4fd7b21ee23")]
  public async Task It_should_create_a_new_nature(string? idValue)
  {
    CreateOrReplaceNaturePayload payload = new(" Courroucé ")
    {
      Description = "  Les émotions du personnage sont vives et ses mouvements sont brusques.  ",
      Attribute = Attribute.Agility,
      GiftId = Guid.NewGuid()
    };

    bool parsed = Guid.TryParse(idValue, out Guid id);
    CreateOrReplaceNatureCommand command = new(parsed ? id : null, payload, Version: null);
    command.Contextualize(_world);

    CreateOrReplaceNatureResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_model, result.Nature);
    Assert.True(result.Created);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(command, EntityType.Customization, _cancellationToken), Times.Once);
    _permissionService.Verify(x => x.EnsureCanCreateAsync(command, EntityType.Nature, _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SetGiftCommand>(y => y.Id == payload.GiftId), _cancellationToken), Times.Once);
    _sender.Verify(x => x.Send(
      It.Is<SaveNatureCommand>(y => (!parsed || y.Nature.EntityId == id)
        && y.Nature.Name.Value == payload.Name.Trim()
        && y.Nature.Description != null && y.Nature.Description.Value == payload.Description.Trim()
        && y.Nature.Attribute == payload.Attribute),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should replace an existing nature.")]
  public async Task It_should_replace_an_existing_nature()
  {
    CreateOrReplaceNaturePayload payload = new(" Courroucé ")
    {
      Description = "  Les émotions du personnage sont vives et ses mouvements sont brusques.  ",
      Attribute = Attribute.Agility,
      GiftId = Guid.NewGuid()
    };

    CreateOrReplaceNatureCommand command = new(_nature.EntityId, payload, Version: null);
    command.Contextualize(_world);

    CreateOrReplaceNatureResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_model, result.Nature);
    Assert.False(result.Created);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(command, EntityType.Customization, _cancellationToken), Times.Once);
    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Nature && y.Id == _nature.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SetGiftCommand>(y => y.Nature.Equals(_nature) && y.Id == payload.GiftId), _cancellationToken), Times.Once);
    _sender.Verify(x => x.Send(
      It.Is<SaveNatureCommand>(y => y.Nature.Equals(_nature)
        && y.Nature.Name.Value == payload.Name.Trim()
        && y.Nature.Description != null && y.Nature.Description.Value == payload.Description.Trim()
        && y.Nature.Attribute == payload.Attribute),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return null when updating a nature that does not exist.")]
  public async Task It_should_return_null_when_updating_an_nature_that_does_not_exist()
  {
    CreateOrReplaceNatureCommand command = new(Guid.Empty, new CreateOrReplaceNaturePayload("Courroucé"), Version: 0);
    command.Contextualize(_world);

    CreateOrReplaceNatureResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Null(result.Nature);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateOrReplaceNaturePayload payload = new();

    CreateOrReplaceNatureCommand command = new(Id: null, payload, Version: null);
    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("NotEmptyValidator", error.ErrorCode);
    Assert.Equal("Name", error.PropertyName);
  }

  [Fact(DisplayName = "It should update an existing nature.")]
  public async Task It_should_update_an_existing_nature()
  {
    Nature reference = new(_nature.WorldId, _nature.Name, _world.OwnerId, _nature.EntityId)
    {
      Description = _nature.Description
    };
    reference.Update(_world.OwnerId);
    _natureRepository.Setup(x => x.LoadAsync(reference.Id, reference.Version, _cancellationToken)).ReturnsAsync(reference);

    Description description = new("Les émotions du personnage sont vives et ses mouvements sont brusques.");
    _nature.Description = description;
    _nature.Update(_world.OwnerId);

    CreateOrReplaceNaturePayload payload = new(" Courroucé ")
    {
      Description = "    ",
      Attribute = Attribute.Agility,
      GiftId = Guid.NewGuid()
    };

    CreateOrReplaceNatureCommand command = new(_nature.EntityId, payload, reference.Version);
    command.Contextualize(_world);

    CreateOrReplaceNatureResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_model, result.Nature);
    Assert.False(result.Created);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(command, EntityType.Customization, _cancellationToken), Times.Once);
    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Nature && y.Id == _nature.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SetGiftCommand>(y => y.Nature.Equals(_nature) && y.Id == payload.GiftId), _cancellationToken), Times.Once);
    _sender.Verify(x => x.Send(
      It.Is<SaveNatureCommand>(y => y.Nature.Equals(_nature)
        && y.Nature.Name.Value == payload.Name.Trim()
        && y.Nature.Description == description
        && y.Nature.Attribute == payload.Attribute),
      _cancellationToken), Times.Once);
  }
}
