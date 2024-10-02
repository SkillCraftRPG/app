using FluentValidation.Results;
using Logitar.Security.Cryptography;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Storages;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Domain;
using SkillCraft.Domain.Customizations;

namespace SkillCraft.Application.Customizations.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class UpdateCustomizationCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICustomizationQuerier> _customizationQuerier = new();
  private readonly Mock<ICustomizationRepository> _customizationRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly UpdateCustomizationCommandHandler _handler;

  private readonly WorldMock _world = new();

  public UpdateCustomizationCommandHandlerTests()
  {
    _handler = new(_customizationQuerier.Object, _customizationRepository.Object, _permissionService.Object, _storageService.Object);
  }

  [Fact(DisplayName = "It should return null when the customization could not be found.")]
  public async Task It_should_return_null_when_the_customization_could_not_be_found()
  {
    UpdateCustomizationPayload payload = new();
    UpdateCustomizationCommand command = new(Guid.Empty, payload);
    command.Contextualize(_world);

    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    UpdateCustomizationPayload payload = new()
    {
      Name = RandomStringGenerator.GetString(Name.MaximumLength + 1)
    };
    UpdateCustomizationCommand command = new(Guid.Empty, payload);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("MaximumLengthValidator", error.ErrorCode);
    Assert.Equal("Name", error.PropertyName);
  }

  [Fact(DisplayName = "It should update an existing customization.")]
  public async Task It_should_update_an_existing_customization()
  {
    Customization customization = new(_world.Id, CustomizationType.Gift, new Name("aigrefin"), _world.OwnerId)
    {
      Description = new Description("Lorsque le personnage n’est pas surpris en situation de combat, alors il peut utiliser sa réaction afin d’effectuer l’activité **Objet**. Également, il peut effectuer l’activité **Objet** en action libre une fois par tour.")
    };
    customization.Update(_world.OwnerId);
    _customizationRepository.Setup(x => x.LoadAsync(customization.Id, _cancellationToken)).ReturnsAsync(customization);

    UpdateCustomizationPayload payload = new()
    {
      Name = " Aigrefin ",
      Description = new Change<string>("    ")
    };
    UpdateCustomizationCommand command = new(customization.EntityId, payload);
    command.Contextualize(_world);

    CustomizationModel model = new();
    _customizationQuerier.Setup(x => x.ReadAsync(customization, _cancellationToken)).ReturnsAsync(model);

    CustomizationModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Customization && y.Id == customization.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);

    _customizationRepository.Verify(x => x.SaveAsync(
      It.Is<Customization>(y => y.Equals(customization) && y.Name.Value == payload.Name.Trim() && y.Description == null),
      _cancellationToken), Times.Once);

    _storageService.Verify(x => x.EnsureAvailableAsync(
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Customization && y.Id == customization.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);
    _storageService.Verify(x => x.UpdateAsync(
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Customization && y.Id == customization.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);
  }
}
