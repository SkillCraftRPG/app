﻿using FluentValidation.Results;
using Logitar.Portal.Contracts.Users;
using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Contracts.Personalities;
using SkillCraft.Domain;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Personalities;
using SkillCraft.Domain.Worlds;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Application.Personalities.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreatePersonalityCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICustomizationRepository> _customizationRepository = new();
  private readonly Mock<IPersonalityQuerier> _personalityQuerier = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();

  private readonly CreatePersonalityCommandHandler _handler;

  private readonly User _user;
  private readonly World _world;
  private readonly Customization _reflexes;

  public CreatePersonalityCommandHandlerTests()
  {
    _handler = new(_customizationRepository.Object, _permissionService.Object, _personalityQuerier.Object, _sender.Object);

    _user = new UserMock();
    _world = new(new Slug("ungar"), new UserId(_user.Id));

    _reflexes = new(_world.Id, CustomizationType.Gift, new Name("Réflexes"), _world.OwnerId);
    _customizationRepository.Setup(x => x.LoadAsync(_reflexes.Id, _cancellationToken)).ReturnsAsync(_reflexes);
  }

  [Fact(DisplayName = "It should create a new personality without gift.")]
  public async Task It_should_create_a_new_personality_githout_gift()
  {
    CreatePersonalityPayload payload = new(" Agile ");
    CreatePersonalityCommand command = new(payload);
    command.Contextualize(_user, _world);

    PersonalityModel model = new();
    _personalityQuerier.Setup(x => x.ReadAsync(It.IsAny<Personality>(), _cancellationToken)).ReturnsAsync(model);

    PersonalityModel result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(result, model);

    _permissionService.Verify(x => x.EnsureCanCreateAsync(command, EntityType.Personality, _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SavePersonalityCommand>(y => y.Personality.GiftId == null), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should create a new personality.")]
  public async Task It_should_create_a_new_personality()
  {
    CreatePersonalityPayload payload = new(" Agile ")
    {
      Description = "    ",
      Attribute = Attribute.Agility,
      GiftId = _reflexes.Id.ToGuid()
    };
    CreatePersonalityCommand command = new(payload);
    command.Contextualize(_user, _world);

    PersonalityModel model = new();
    _personalityQuerier.Setup(x => x.ReadAsync(It.IsAny<Personality>(), _cancellationToken)).ReturnsAsync(model);

    PersonalityModel result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(result, model);

    _permissionService.Verify(x => x.EnsureCanCreateAsync(command, EntityType.Personality, _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SavePersonalityCommand>(y => y.Personality.WorldId == _world.Id
      && y.Personality.Name.Value == payload.Name.Trim()
      && y.Personality.Description == null
      && y.Personality.Attribute == payload.Attribute
      && y.Personality.GiftId == _reflexes.Id), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw AggregateNotFoundException when the gift could not be found.")]
  public async Task It_should_throw_AggregateNotFoundException_when_the_gift_could_not_be_found()
  {
    CreatePersonalityPayload payload = new(" Agile ")
    {
      GiftId = Guid.NewGuid()
    };
    CreatePersonalityCommand command = new(payload);
    command.Contextualize(_user, _world);

    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<Customization>>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(new CustomizationId(payload.GiftId.Value).Value, exception.Id);
    Assert.Equal("GiftId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreatePersonalityPayload payload = new("Agile")
    {
      Attribute = (Attribute)(-1)
    };
    CreatePersonalityCommand command = new(payload);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("EnumValidator", error.ErrorCode);
    Assert.Equal("Attribute.Value", error.PropertyName);
    Assert.Equal(payload.Attribute, error.AttemptedValue);
  }
}
