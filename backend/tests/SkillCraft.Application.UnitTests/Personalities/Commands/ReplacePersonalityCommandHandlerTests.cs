using FluentValidation.Results;
using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Contracts.Personalities;
using SkillCraft.Domain;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Personalities;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Application.Personalities.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class ReplacePersonalityCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICustomizationRepository> _customizationRepository = new();
  private readonly Mock<IPersonalityQuerier> _personalityQuerier = new();
  private readonly Mock<IPersonalityRepository> _personalityRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();

  private readonly ReplacePersonalityCommandHandler _handler;

  private readonly WorldMock _world = new();
  private readonly Customization _reflexes;

  public ReplacePersonalityCommandHandlerTests()
  {
    _handler = new(_customizationRepository.Object, _permissionService.Object, _personalityQuerier.Object, _personalityRepository.Object, _sender.Object);

    _reflexes = new(_world.Id, CustomizationType.Gift, new Name("Réflexes"), _world.OwnerId);
    _customizationRepository.Setup(x => x.LoadAsync(_reflexes.Id, _cancellationToken)).ReturnsAsync(_reflexes);
  }

  [Fact(DisplayName = "It should replace an existing personality without gift.")]
  public async Task It_should_replace_an_existing_personality_without_gift()
  {
    Personality personality = new(_world.Id, new Name("agile"), _world.OwnerId);
    personality.SetGift(_reflexes);
    personality.Update(_world.OwnerId);
    _personalityRepository.Setup(x => x.LoadAsync(personality.Id, _cancellationToken)).ReturnsAsync(personality);

    ReplacePersonalityPayload payload = new("Agile");
    ReplacePersonalityCommand command = new(personality.Id.ToGuid(), payload, Version: null);
    command.Contextualize();

    PersonalityModel model = new();
    _personalityQuerier.Setup(x => x.ReadAsync(personality, _cancellationToken)).ReturnsAsync(model);

    PersonalityModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Key.Type == EntityType.Personality && y.Key.Id == personality.Id.ToGuid() && y.Size > 0),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SavePersonalityCommand>(y => y.Personality.GiftId == null), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should replace an existing personality.")]
  public async Task It_should_replace_an_existing_personality()
  {
    Personality personality = new(_world.Id, new Name("agile"), _world.OwnerId);
    _personalityRepository.Setup(x => x.LoadAsync(personality.Id, _cancellationToken)).ReturnsAsync(personality);

    ReplacePersonalityPayload payload = new(" Agile ")
    {
      Description = "  Les mouvements du personnage sont effectués avec aisance et promptitude, souplesse et alerte.  ",
      Attribute = Attribute.Agility,
      GiftId = _reflexes.Id.ToGuid()
    };
    ReplacePersonalityCommand command = new(personality.Id.ToGuid(), payload, Version: null);
    command.Contextualize();

    PersonalityModel model = new();
    _personalityQuerier.Setup(x => x.ReadAsync(personality, _cancellationToken)).ReturnsAsync(model);

    PersonalityModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Key.Type == EntityType.Personality && y.Key.Id == personality.Id.ToGuid() && y.Size > 0),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SavePersonalityCommand>(y => y.Personality.Equals(personality)
      && y.Personality.Name.Value == payload.Name.Trim()
      && y.Personality.Description != null && y.Personality.Description.Value == payload.Description.Trim()
      && y.Personality.Attribute == payload.Attribute
      && y.Personality.GiftId == _reflexes.Id), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return null when the personality could not be found.")]
  public async Task It_should_return_null_when_the_personality_could_not_be_found()
  {
    ReplacePersonalityPayload payload = new("Agile");
    ReplacePersonalityCommand command = new(Guid.Empty, payload, Version: null);

    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw AggregateNotFoundException when the gift could not be found.")]
  public async Task It_should_throw_AggregateNotFoundException_when_the_gift_could_not_be_found()
  {
    Personality personality = new(_world.Id, new Name("agile"), _world.OwnerId);
    _personalityRepository.Setup(x => x.LoadAsync(personality.Id, _cancellationToken)).ReturnsAsync(personality);

    ReplacePersonalityPayload payload = new("Agile")
    {
      GiftId = Guid.NewGuid()
    };
    ReplacePersonalityCommand command = new(personality.Id.ToGuid(), payload, Version: null);

    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<Customization>>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(new CustomizationId(payload.GiftId.Value).Value, exception.Id);
    Assert.Equal("GiftId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    ReplacePersonalityPayload payload = new("Agile")
    {
      GiftId = Guid.Empty
    };
    ReplacePersonalityCommand command = new(Guid.Empty, payload, Version: null);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("NotEmptyValidator", error.ErrorCode);
    Assert.Equal("GiftId.Value", error.PropertyName);
    Assert.Equal(payload.GiftId, error.AttemptedValue);
  }

  [Fact(DisplayName = "It should update an existing personality from a reference.")]
  public async Task It_should_update_an_existing_personality_from_a_reference()
  {
    Personality reference = new(_world.Id, new Name("agile"), _world.OwnerId);
    long version = reference.Version;
    _personalityRepository.Setup(x => x.LoadAsync(reference.Id, version, _cancellationToken)).ReturnsAsync(reference);

    Personality personality = new(_world.Id, reference.Name, _world.OwnerId, reference.Id);
    _personalityRepository.Setup(x => x.LoadAsync(personality.Id, _cancellationToken)).ReturnsAsync(personality);

    Description description = new("  Les mouvements du personnage sont effectués avec aisance et promptitude, souplesse et alerte.  ");
    personality.Description = description;
    personality.Update(_world.OwnerId);

    ReplacePersonalityPayload payload = new(" Agile ")
    {
      Description = "    ",
      Attribute = Attribute.Agility,
      GiftId = _reflexes.Id.ToGuid()
    };
    ReplacePersonalityCommand command = new(personality.Id.ToGuid(), payload, version);
    command.Contextualize();

    PersonalityModel model = new();
    _personalityQuerier.Setup(x => x.ReadAsync(personality, _cancellationToken)).ReturnsAsync(model);

    PersonalityModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Key.Type == EntityType.Personality && y.Key.Id == personality.Id.ToGuid() && y.Size > 0),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SavePersonalityCommand>(y => y.Personality.Equals(personality)
      && y.Personality.Name.Value == payload.Name.Trim()
      && y.Personality.Description == description
      && y.Personality.Attribute == payload.Attribute
      && y.Personality.GiftId == _reflexes.Id), _cancellationToken), Times.Once);
  }
}
