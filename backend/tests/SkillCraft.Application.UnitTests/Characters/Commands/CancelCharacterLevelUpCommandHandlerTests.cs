﻿using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain.Characters;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Application.Characters.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CancelCharacterLevelUpCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICharacterQuerier> _characterQuerier = new();
  private readonly Mock<ICharacterRepository> _characterRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();

  private readonly CancelCharacterLevelUpCommandHandler _handler;

  private readonly WorldMock _world = new();

  public CancelCharacterLevelUpCommandHandlerTests()
  {
    _handler = new(_characterQuerier.Object, _characterRepository.Object, _permissionService.Object, _sender.Object);
  }

  [Fact(DisplayName = "It should cancel the latest character level-up.")]
  public async Task It_should_cancel_the_latest_character_level_up()
  {
    Character character = new CharacterBuilder(_world).CanLevelUpTo(2).Build();
    _characterRepository.Setup(x => x.LoadAsync(character.Id, _cancellationToken)).ReturnsAsync(character);

    character.LevelUp(Attribute.Agility, _world.OwnerId);
    character.LevelUp(Attribute.Vigor, _world.OwnerId);
    Assert.Equal(2, character.Level);

    CharacterModel model = new();
    _characterQuerier.Setup(x => x.ReadAsync(character, _cancellationToken)).ReturnsAsync(model);

    CancelCharacterLevelUpCommand command = new(character.EntityId);
    command.Contextualize(_world);

    CharacterModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    Assert.Equal(1, character.Level);
    Assert.Equal(Attribute.Agility, Assert.Single(character.LevelUps).Attribute);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Character && y.Id == character.EntityId),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveCharacterCommand>(y => y.Character.Equals(character)), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return null when the character could not be found.")]
  public async Task It_should_return_null_when_the_character_could_not_be_found()
  {
    CancelCharacterLevelUpCommand command = new(Guid.NewGuid());
    command.Contextualize(_world);

    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }
}