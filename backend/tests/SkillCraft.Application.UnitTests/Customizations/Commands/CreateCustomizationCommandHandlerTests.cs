using FluentValidation.Results;
using Logitar.Portal.Contracts.Users;
using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Domain;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Customizations.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateCustomizationCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICustomizationQuerier> _customizationQuerier = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();

  private readonly CreateCustomizationCommandHandler _handler;

  private readonly User _user;
  private readonly World _world;

  public CreateCustomizationCommandHandlerTests()
  {
    _handler = new(_customizationQuerier.Object, _permissionService.Object, _sender.Object);

    _user = new UserMock();
    _world = new(new Slug("ungar"), new UserId(_user.Id));
  }

  [Fact(DisplayName = "It should create a new customization.")]
  public async Task It_should_create_a_new_customization()
  {
    CreateCustomizationPayload payload = new(" Adresse légendaire ")
    {
      Type = CustomizationType.Gift,
      Description = "    "
    };
    CreateCustomizationCommand command = new(payload);
    command.Contextualize(_user, _world);

    CustomizationModel model = new();
    _customizationQuerier.Setup(x => x.ReadAsync(It.IsAny<Customization>(), _cancellationToken)).ReturnsAsync(model);

    CustomizationModel result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(result, model);

    _permissionService.Verify(x => x.EnsureCanCreateAsync(command, EntityType.Customization, _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveCustomizationCommand>(y => y.Customization.WorldId == _world.Id
      && y.Customization.Type == payload.Type
      && y.Customization.Name.Value == payload.Name.Trim()
      && y.Customization.Description == null), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateCustomizationPayload payload = new("Abruti")
    {
      Type = (CustomizationType)(-1)
    };
    CreateCustomizationCommand command = new(payload);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("EnumValidator", error.ErrorCode);
    Assert.Equal("Type", error.PropertyName);
    Assert.Equal(payload.Type, error.AttemptedValue);
  }
}
