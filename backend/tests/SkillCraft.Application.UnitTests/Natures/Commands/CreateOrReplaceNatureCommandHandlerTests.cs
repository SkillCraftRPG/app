﻿using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using SkillCraft.Application.Customizations;
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
public class CreateOrReplaceNatureCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICustomizationRepository> _customizationRepository = new();
  private readonly Mock<INatureQuerier> _natureQuerier = new();
  private readonly Mock<INatureRepository> _natureRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();

  private readonly CreateOrReplaceNatureCommandHandler _handler;

  private readonly WorldMock _world = new();
  private readonly Customization _gift;
  private readonly Nature _nature;
  private readonly NatureModel _model = new();

  public CreateOrReplaceNatureCommandHandlerTests()
  {
    _handler = new(_customizationRepository.Object, _natureQuerier.Object, _natureRepository.Object, _permissionService.Object, _sender.Object);

    _gift = new(_world.Id, CustomizationType.Gift, new Name("Féroce"), _world.OwnerId);
    _customizationRepository.Setup(x => x.LoadAsync(_gift.Id, _cancellationToken)).ReturnsAsync(_gift);

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
      GiftId = _gift.EntityId
    };

    bool parsed = Guid.TryParse(idValue, out Guid id);
    CreateOrReplaceNatureCommand command = new(parsed ? id : null, payload, Version: null);
    command.Contextualize(_world);

    CreateOrReplaceNatureResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_model, result.Nature);
    Assert.True(result.Created);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(command, EntityType.Customization, _cancellationToken), Times.Once);
    _permissionService.Verify(x => x.EnsureCanCreateAsync(command, EntityType.Nature, _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(
      It.Is<SaveNatureCommand>(y => (!parsed || y.Nature.EntityId == id)
        && y.Nature.Name.Value == payload.Name.Trim()
        && y.Nature.Description != null && y.Nature.Description.Value == payload.Description.Trim()
        && y.Nature.Attribute == payload.Attribute
        && y.Nature.GiftId == _gift.Id),
      _cancellationToken), Times.Once);
  }

  [Theory(DisplayName = "It should nullify the gift.")]
  [InlineData(null)]
  [InlineData("de7ee9b0-6feb-4b62-856f-848de57c3c10")]
  public async Task It_should_nullify_the_gift(string? idValue)
  {
    Nature? nature = null;
    if (idValue != null)
    {
      nature = new(_world.Id, new Name("Courroucé"), _world.OwnerId, Guid.Parse(idValue));
      _natureRepository.Setup(x => x.LoadAsync(nature.Id, _cancellationToken)).ReturnsAsync(nature);

      Customization gift = new(_world.Id, CustomizationType.Gift, new Name("Féroce"), _world.OwnerId);
      nature.SetGift(gift);
      nature.Update(_world.OwnerId);
    }

    CreateOrReplaceNaturePayload payload = new("Courroucé");
    CreateOrReplaceNatureCommand command = new(nature?.EntityId, payload, Version: null);
    command.Contextualize(_world);

    CreateOrReplaceNatureResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_model, result.Nature);
    Assert.Equal(idValue == null, result.Created);

    if (nature == null)
    {
      _sender.Verify(x => x.Send(It.Is<SaveNatureCommand>(y => y.Nature.GiftId == null), _cancellationToken), Times.Once);
    }
    else
    {
      Assert.Null(nature.GiftId);
    }
  }

  [Fact(DisplayName = "It should replace an existing nature.")]
  public async Task It_should_replace_an_existing_nature()
  {
    CreateOrReplaceNaturePayload payload = new(" Courroucé ")
    {
      Description = "  Les émotions du personnage sont vives et ses mouvements sont brusques.  ",
      Attribute = Attribute.Agility,
      GiftId = _gift.EntityId
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

    _sender.Verify(x => x.Send(
      It.Is<SaveNatureCommand>(y => y.Nature.Equals(_nature)
        && y.Nature.Name.Value == payload.Name.Trim()
        && y.Nature.Description != null && y.Nature.Description.Value == payload.Description.Trim()
        && y.Nature.Attribute == payload.Attribute
        && y.Nature.GiftId == _gift.Id),
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

  [Theory(DisplayName = "It should throw CustomizationNotFoundException when the customization could not be found.")]
  [InlineData(null)]
  [InlineData("bfd82453-f40f-430f-8feb-0aad4eed8ded")]
  public async Task It_should_throw_CustomizationNotFoundException_when_the_customization_could_not_be_found(string? idValue)
  {
    Nature? nature = null;
    if (idValue != null)
    {
      nature = new(_world.Id, new Name("Courroucé"), _world.OwnerId, Guid.Parse(idValue));
      _natureRepository.Setup(x => x.LoadAsync(nature.Id, _cancellationToken)).ReturnsAsync(nature);
    }

    CreateOrReplaceNaturePayload payload = new("Courroucé")
    {
      GiftId = Guid.NewGuid()
    };
    CreateOrReplaceNatureCommand command = new(nature?.EntityId, payload, Version: null);
    command.Contextualize(_world);

    var exception = await Assert.ThrowsAsync<CustomizationNotFoundException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(payload.GiftId, exception.CustomizationId);
    Assert.Equal("GiftId", exception.PropertyName);
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
      GiftId = _gift.EntityId
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

    _sender.Verify(x => x.Send(
      It.Is<SaveNatureCommand>(y => y.Nature.Equals(_nature)
        && y.Nature.Name.Value == payload.Name.Trim()
        && y.Nature.Description == description
        && y.Nature.Attribute == payload.Attribute
        && y.Nature.GiftId == _gift.Id),
      _cancellationToken), Times.Once);
  }
}
