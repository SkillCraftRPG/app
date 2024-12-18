﻿using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain.Characters;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Application.Characters.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class LevelUpCharacterCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICharacterQuerier> _characterQuerier = new();
  private readonly Mock<ICharacterRepository> _characterRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();

  private readonly LevelUpCharacterCommandHandler _handler;

  private readonly WorldMock _world = new();

  public LevelUpCharacterCommandHandlerTests()
  {
    _handler = new(_characterQuerier.Object, _characterRepository.Object, _permissionService.Object, _sender.Object);
  }

  [Fact(DisplayName = "It should level-up the character.")]
  public async Task It_should_level_up_the_character()
  {
    Character character = new CharacterBuilder(_world).CanLevelUpTo(1).Build();
    _characterRepository.Setup(x => x.LoadAsync(character.Id, _cancellationToken)).ReturnsAsync(character);

    Assert.Equal(0, character.Level);
    Assert.Empty(character.LevelUps);

    CharacterModel model = new();
    _characterQuerier.Setup(x => x.ReadAsync(character, _cancellationToken)).ReturnsAsync(model);

    LevelUpCharacterPayload payload = new(Attribute.Vigor);
    LevelUpCharacterCommand command = new(character.EntityId, payload);
    command.Contextualize(_world);

    CharacterModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    Assert.Equal(1, character.Level);
    Assert.Equal(payload.Attribute, Assert.Single(character.LevelUps).Attribute);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Character && y.Id == character.EntityId),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveCharacterCommand>(y => y.Character.Equals(character)), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return null when the character could not be found.")]
  public async Task It_should_return_null_when_the_character_could_not_be_found()
  {
    LevelUpCharacterPayload payload = new(Attribute.Vigor);
    LevelUpCharacterCommand command = new(Guid.NewGuid(), payload);
    command.Contextualize(_world);

    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    LevelUpCharacterPayload payload = new((Attribute)(-1));
    LevelUpCharacterCommand command = new(Guid.Empty, payload);
    command.Contextualize(_world);

    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("EnumValidator", error.ErrorCode);
    Assert.Equal("Attribute", error.PropertyName);
  }
}
