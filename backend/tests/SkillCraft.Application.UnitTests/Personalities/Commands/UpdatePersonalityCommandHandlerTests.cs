using FluentValidation.Results;
using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Contracts.Personalities;
using SkillCraft.Domain;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Personalities;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Application.Personalities.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class UpdatePersonalityCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IPersonalityQuerier> _personalityQuerier = new();
  private readonly Mock<IPersonalityRepository> _personalityRepository = new();
  private readonly Mock<ISender> _sender = new();

  private readonly UpdatePersonalityCommandHandler _handler;

  private readonly WorldMock _world = new();

  public UpdatePersonalityCommandHandlerTests()
  {
    _handler = new(_permissionService.Object, _personalityQuerier.Object, _personalityRepository.Object, _sender.Object);
  }

  [Fact(DisplayName = "It should return null when the personality could not be found.")]
  public async Task It_should_return_null_when_the_personality_could_not_be_found()
  {
    UpdatePersonalityPayload payload = new();
    UpdatePersonalityCommand command = new(Guid.Empty, payload);
    command.Contextualize(_world);

    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    UpdatePersonalityPayload payload = new()
    {
      GiftId = new Change<Guid?>(Guid.Empty)
    };
    UpdatePersonalityCommand command = new(Guid.Empty, payload);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("NotEmptyValidator", error.ErrorCode);
    Assert.Equal("GiftId.Value.Value", error.PropertyName);
    Assert.Equal(payload.GiftId.Value, error.AttemptedValue);
  }

  [Fact(DisplayName = "It should update an existing personality without gift.")]
  public async Task It_should_update_an_existing_personality_without_gift()
  {
    Personality personality = new(_world.Id, new Name("agile"), _world.OwnerId);
    personality.SetGift(new Customization(_world.Id, CustomizationType.Gift, new Name("Réflexes"), _world.OwnerId));
    personality.Update(_world.OwnerId);
    _personalityRepository.Setup(x => x.LoadAsync(personality.Id, _cancellationToken)).ReturnsAsync(personality);

    UpdatePersonalityPayload payload = new()
    {
      Name = " Agile ",
      Description = new Change<string>("  Les mouvements du personnage sont effectués avec aisance et promptitude, souplesse et alerte.  "),
      Attribute = new Change<Attribute?>(Attribute.Agility),
      GiftId = new Change<Guid?>(null)
    };
    UpdatePersonalityCommand command = new(personality.EntityId, payload);
    command.Contextualize(_world);

    PersonalityModel model = new();
    _personalityQuerier.Setup(x => x.ReadAsync(personality, _cancellationToken)).ReturnsAsync(model);

    PersonalityModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Key.Type == EntityType.Personality && y.Key.Id == personality.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);

    Assert.NotNull(payload.Description.Value);
    _sender.Verify(x => x.Send(It.Is<SetGiftCommand>(y => y.Activity == command && y.Personality == personality && y.Id == null), _cancellationToken), Times.Once);
    _sender.Verify(x => x.Send(It.Is<SavePersonalityCommand>(y => y.Personality.Name.Value == payload.Name.Trim()
      && y.Personality.Description != null && y.Personality.Description.Value == payload.Description.Value.Trim()
      && y.Personality.Attribute == payload.Attribute.Value), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should update an existing personality.")]
  public async Task It_should_update_an_existing_personality()
  {
    Personality personality = new(_world.Id, new Name("agile"), _world.OwnerId)
    {
      Description = new Description("Les mouvements du personnage sont effectués avec aisance et promptitude, souplesse et alerte.")
    };
    personality.Update(_world.OwnerId);
    _personalityRepository.Setup(x => x.LoadAsync(personality.Id, _cancellationToken)).ReturnsAsync(personality);

    UpdatePersonalityPayload payload = new()
    {
      Name = " Agile ",
      Description = new Change<string>("    "),
      Attribute = new Change<Attribute?>(Attribute.Agility),
      GiftId = new Change<Guid?>(Guid.NewGuid())
    };
    UpdatePersonalityCommand command = new(personality.EntityId, payload);
    command.Contextualize(_world);

    PersonalityModel model = new();
    _personalityQuerier.Setup(x => x.ReadAsync(personality, _cancellationToken)).ReturnsAsync(model);

    PersonalityModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Personality && y.Id == personality.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SetGiftCommand>(y => y.Activity == command && y.Personality == personality && y.Id == payload.GiftId.Value), _cancellationToken), Times.Once);
    _sender.Verify(x => x.Send(It.Is<SavePersonalityCommand>(y => y.Personality.Equals(personality)
      && y.Personality.Name.Value == payload.Name.Trim()
      && y.Personality.Description == null
      && y.Personality.Attribute == payload.Attribute.Value), _cancellationToken), Times.Once);
  }
}
