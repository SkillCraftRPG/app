using FluentValidation;
using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Domain;
using SkillCraft.Domain.Customizations;

namespace SkillCraft.Application.Customizations.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateOrReplaceCustomizationCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICustomizationQuerier> _customizationQuerier = new();
  private readonly Mock<ICustomizationRepository> _customizationRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();

  private readonly CreateOrReplaceCustomizationCommandHandler _handler;

  private readonly WorldMock _world = new();
  private readonly Customization _customization;
  private readonly CustomizationModel _model = new();

  public CreateOrReplaceCustomizationCommandHandlerTests()
  {
    _handler = new(_customizationQuerier.Object, _customizationRepository.Object, _permissionService.Object, _sender.Object);

    _customization = new(_world.Id, CustomizationType.Gift, new Name("aigrefin"), _world.OwnerId);
    _customizationRepository.Setup(x => x.LoadAsync(_customization.Id, _cancellationToken)).ReturnsAsync(_customization);

    _customizationQuerier.Setup(x => x.ReadAsync(It.IsAny<Customization>(), _cancellationToken)).ReturnsAsync(_model);
  }

  [Theory(DisplayName = "It should create a new customization.")]
  [InlineData(null)]
  [InlineData("85fff4b6-561e-4f45-ab11-9047a285cd4a")]
  public async Task It_should_create_a_new_customization(string? idValue)
  {
    CreateOrReplaceCustomizationPayload payload = new(" Aigrefin ")
    {
      Description = "    "
    };

    bool parsed = Guid.TryParse(idValue, out Guid id);
    CreateOrReplaceCustomizationCommand command = new(parsed ? id : null, payload, Version: null);
    command.Contextualize(_world);

    CreateOrReplaceCustomizationResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_model, result.Customization);
    Assert.True(result.Created);

    _permissionService.Verify(x => x.EnsureCanCreateAsync(command, EntityType.Customization, _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(
      It.Is<SaveCustomizationCommand>(y => (!parsed || y.Customization.EntityId == id)
        && y.Customization.Name.Value == payload.Name.Trim()
        && y.Customization.Description == null),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should replace an existing customization.")]
  public async Task It_should_replace_an_existing_customization()
  {
    CreateOrReplaceCustomizationPayload payload = new(" Aigrefin ")
    {
      Description = "  Lorsque le personnage n’est pas surpris en situation de combat, alors il peut utiliser sa réaction afin d’effectuer l’activité **Objet**. Également, il peut effectuer l’activité **Objet** en action libre une fois par tour.  "
    };

    CreateOrReplaceCustomizationCommand command = new(_customization.EntityId, payload, Version: null);
    command.Contextualize(_world);

    CreateOrReplaceCustomizationResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_model, result.Customization);
    Assert.False(result.Created);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Customization && y.Id == _customization.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(
      It.Is<SaveCustomizationCommand>(y => y.Customization.Equals(_customization)
        && y.Customization.Name.Value == payload.Name.Trim()
        && y.Customization.Description != null && y.Customization.Description.Value == payload.Description.Trim()),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return null when updating an customization that does not exist.")]
  public async Task It_should_return_null_when_updating_an_customization_that_does_not_exist()
  {
    CreateOrReplaceCustomizationCommand command = new(Guid.Empty, new CreateOrReplaceCustomizationPayload("Aigrefin"), Version: 0);
    command.Contextualize(_world);

    CreateOrReplaceCustomizationResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Null(result.Customization);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateOrReplaceCustomizationPayload payload = new()
    {
      Type = (CustomizationType)(-1)
    };

    CreateOrReplaceCustomizationCommand command = new(Id: null, payload, Version: null);
    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "EnumValidator" && e.PropertyName == "Type");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Name");
  }

  [Fact(DisplayName = "It should update an existing customization.")]
  public async Task It_should_update_an_existing_customization()
  {
    Customization reference = new(_customization.WorldId, _customization.Type, _customization.Name, _world.OwnerId, _customization.EntityId)
    {
      Description = _customization.Description
    };
    reference.Update(_world.OwnerId);
    _customizationRepository.Setup(x => x.LoadAsync(reference.Id, reference.Version, _cancellationToken)).ReturnsAsync(reference);

    Description description = new("PLorsque le personnage n’est pas surpris en situation de combat, alors il peut utiliser sa réaction afin d’effectuer l’activité **Objet**. Également, il peut effectuer l’activité **Objet** en action libre une fois par tour.");
    _customization.Description = description;
    _customization.Update(_world.OwnerId);

    CreateOrReplaceCustomizationPayload payload = new(" Aigrefin ")
    {
      Description = "    "
    };

    CreateOrReplaceCustomizationCommand command = new(_customization.EntityId, payload, reference.Version);
    command.Contextualize(_world);

    CreateOrReplaceCustomizationResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_model, result.Customization);
    Assert.False(result.Created);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Customization && y.Id == _customization.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(
      It.Is<SaveCustomizationCommand>(y => y.Customization.Equals(_customization)
        && y.Customization.Name.Value == payload.Name.Trim()
        && y.Customization.Description == description),
      _cancellationToken), Times.Once);
  }
}
