using FluentValidation.Results;
using Logitar.Security.Cryptography;
using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Contracts.Natures;
using SkillCraft.Domain;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Natures;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Application.Natures.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class UpdateNatureCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICustomizationRepository> _customizationRepository = new();
  private readonly Mock<INatureQuerier> _natureQuerier = new();
  private readonly Mock<INatureRepository> _natureRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();

  private readonly UpdateNatureCommandHandler _handler;

  private readonly WorldMock _world = new();

  public UpdateNatureCommandHandlerTests()
  {
    _handler = new(_customizationRepository.Object, _natureQuerier.Object, _natureRepository.Object, _permissionService.Object, _sender.Object);
  }

  [Fact(DisplayName = "It should return null when the nature could not be found.")]
  public async Task It_should_return_null_when_the_nature_could_not_be_found()
  {
    UpdateNaturePayload payload = new();
    UpdateNatureCommand command = new(Guid.Empty, payload);
    command.Contextualize(_world);

    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    UpdateNaturePayload payload = new()
    {
      Name = RandomStringGenerator.GetString(Name.MaximumLength + 1)
    };
    UpdateNatureCommand command = new(Guid.Empty, payload);
    command.Contextualize(_world);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("MaximumLengthValidator", error.ErrorCode);
    Assert.Equal("Name", error.PropertyName);
  }

  [Fact(DisplayName = "It should update an existing nature.")]
  public async Task It_should_update_an_existing_nature()
  {
    Customization gift = new(_world.Id, CustomizationType.Gift, new Name("Mysticisme"), _world.OwnerId);
    _customizationRepository.Setup(x => x.LoadAsync(gift.Id, _cancellationToken)).ReturnsAsync(gift);

    Nature nature = new(_world.Id, new Name("mysterieux"), _world.OwnerId)
    {
      Description = new Description("Le personnage s’entoure de secrets et de mystères, il laisse peu paraître ses idées et émotions."),
      Attribute = Attribute.Spirit
    };
    nature.Update(_world.OwnerId);
    _natureRepository.Setup(x => x.LoadAsync(nature.Id, _cancellationToken)).ReturnsAsync(nature);

    UpdateNaturePayload payload = new()
    {
      Name = " Mystérieux ",
      Description = new Change<string>("    "),
      GiftId = new Change<Guid?>(gift.EntityId)
    };
    UpdateNatureCommand command = new(nature.EntityId, payload);
    command.Contextualize(_world);

    NatureModel model = new();
    _natureQuerier.Setup(x => x.ReadAsync(nature, _cancellationToken)).ReturnsAsync(model);

    NatureModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(command, EntityType.Customization, _cancellationToken), Times.Once);
    _permissionService.Verify(x => x.EnsureCanUpdateAsync(command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Nature && y.Id == nature.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(
      It.Is<SaveNatureCommand>(y => y.Nature.Equals(nature)
        && y.Nature.Name.Value == payload.Name.Trim()
        && y.Nature.Description == null
        && y.Nature.Attribute == Attribute.Spirit
        && y.Nature.GiftId == gift.Id),
      _cancellationToken), Times.Once);
  }

  // TODO(fpion): set null GiftId?
  // TODO(fpion): CustomizationNotFoundException
}
