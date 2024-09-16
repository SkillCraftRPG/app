using FluentValidation.Results;
using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Domain;
using SkillCraft.Domain.Customizations;

namespace SkillCraft.Application.Customizations.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class ReplaceCustomizationCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICustomizationQuerier> _customizationQuerier = new();
  private readonly Mock<ICustomizationRepository> _customizationRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();

  private readonly ReplaceCustomizationCommandHandler _handler;

  private readonly WorldMock _world = new();

  public ReplaceCustomizationCommandHandlerTests()
  {
    _handler = new(_customizationQuerier.Object, _customizationRepository.Object, _permissionService.Object, _sender.Object);
  }

  [Fact(DisplayName = "It should replace an existing customization.")]
  public async Task It_should_replace_an_existing_customization()
  {
    Customization customization = new(_world.Id, CustomizationType.Gift, new Name("aigrefin"), _world.OwnerId);
    _customizationRepository.Setup(x => x.LoadAsync(customization.Id, _cancellationToken)).ReturnsAsync(customization);

    Guid subjectId = Guid.NewGuid();
    ReplaceCustomizationPayload payload = new(" Aigrefin ")
    {
      Description = "  Lorsque le personnage n’est pas surpris en situation de combat, alors il peut utiliser sa réaction afin d’effectuer l’activité **Objet**. Également, il peut effectuer l’activité **Objet** en action libre une fois par tour.  "
    };
    ReplaceCustomizationCommand command = new(customization.Id.ToGuid(), payload, Version: null);
    command.Contextualize();

    CustomizationModel model = new();
    _customizationQuerier.Setup(x => x.ReadAsync(customization, _cancellationToken)).ReturnsAsync(model);

    CustomizationModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Key.Type == EntityType.Customization && y.Key.Id == customization.Id.ToGuid() && y.Size > 0),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveCustomizationCommand>(y => y.Customization.Equals(customization)
      && y.Customization.Name.Value == payload.Name.Trim()
      && y.Customization.Description != null && y.Customization.Description.Value == payload.Description.Trim()
      ), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return null when the customization could not be found.")]
  public async Task It_should_return_null_when_the_customization_could_not_be_found()
  {
    ReplaceCustomizationPayload payload = new("Aigrefin");
    ReplaceCustomizationCommand command = new(Guid.Empty, payload, Version: null);

    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    ReplaceCustomizationPayload payload = new();
    ReplaceCustomizationCommand command = new(Guid.Empty, payload, Version: null);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("NotEmptyValidator", error.ErrorCode);
    Assert.Equal("Name", error.PropertyName);
    Assert.Equal(payload.Name, error.AttemptedValue);
  }

  [Fact(DisplayName = "It should update an existing customization from a reference.")]
  public async Task It_should_update_an_existing_customization_from_a_reference()
  {
    Customization reference = new(_world.Id, CustomizationType.Gift, new Name("aigrefin"), _world.OwnerId);
    long version = reference.Version;
    _customizationRepository.Setup(x => x.LoadAsync(reference.Id, version, _cancellationToken)).ReturnsAsync(reference);

    Customization customization = new(_world.Id, reference.Type, reference.Name, _world.OwnerId, reference.Id);
    _customizationRepository.Setup(x => x.LoadAsync(customization.Id, _cancellationToken)).ReturnsAsync(customization);

    Description description = new("  Lorsque le personnage n’est pas surpris en situation de combat, alors il peut utiliser sa réaction afin d’effectuer l’activité **Objet**. Également, il peut effectuer l’activité **Objet** en action libre une fois par tour.  ");
    customization.Description = description;
    customization.Update(_world.OwnerId);

    ReplaceCustomizationPayload payload = new(" Aigrefin ")
    {
      Description = "    "
    };
    ReplaceCustomizationCommand command = new(customization.Id.ToGuid(), payload, version);
    command.Contextualize();

    CustomizationModel model = new();
    _customizationQuerier.Setup(x => x.ReadAsync(customization, _cancellationToken)).ReturnsAsync(model);

    CustomizationModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Key.Type == EntityType.Customization && y.Key.Id == customization.Id.ToGuid() && y.Size > 0),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveCustomizationCommand>(y => y.Customization.Equals(customization)
      && y.Customization.Name.Value == payload.Name.Trim()
      && y.Customization.Description != null && y.Customization.Description == description
      ), _cancellationToken), Times.Once);
  }
}
