using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain.Characters;

namespace SkillCraft.Application.Characters.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class RemoveCharacterBonusCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICharacterQuerier> _characterQuerier = new();
  private readonly Mock<ICharacterRepository> _characterRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();

  private readonly RemoveCharacterBonusCommandHandler _handler;

  private readonly WorldMock _world = new();

  public RemoveCharacterBonusCommandHandlerTests()
  {
    _handler = new(_characterQuerier.Object, _characterRepository.Object, _permissionService.Object, _sender.Object);
  }

  [Fact(DisplayName = "It should not do anything when the bonus could not be found.")]
  public async Task It_should_not_do_anything_when_the_bonus_could_not_be_found()
  {
    Character character = new CharacterBuilder(_world).Build();
    _characterRepository.Setup(x => x.LoadAsync(character.Id, _cancellationToken)).ReturnsAsync(character);

    RemoveCharacterBonusCommand command = new(character.EntityId, Guid.Empty);
    command.Contextualize(_world);

    CharacterModel model = new();
    _characterQuerier.Setup(x => x.ReadAsync(character, _cancellationToken)).ReturnsAsync(model);

    CharacterModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(command, character.GetMetadata(), _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveCharacterCommand>(y => y.Character.Equals(character)), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should remove an existing character bonus.")]
  public async Task It_should_remove_an_existing_character_bonus()
  {
    Character character = new CharacterBuilder(_world).Build();
    _characterRepository.Setup(x => x.LoadAsync(character.Id, _cancellationToken)).ReturnsAsync(character);

    Assert.Empty(character.Bonuses);
    Guid bonusId = Guid.NewGuid();
    Bonus bonus = new(BonusCategory.Skill, Skill.Melee.ToString(), value: +2);
    character.SetBonus(bonusId, bonus, _world.OwnerId);
    Assert.NotEmpty(character.Bonuses);

    RemoveCharacterBonusCommand command = new(character.EntityId, bonusId);
    command.Contextualize(_world);

    CharacterModel model = new();
    _characterQuerier.Setup(x => x.ReadAsync(character, _cancellationToken)).ReturnsAsync(model);

    CharacterModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    Assert.Empty(character.Bonuses);
    Assert.Contains(character.Changes, change => change is Character.BonusRemovedEvent e && e.BonusId == bonusId);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(command, character.GetMetadata(), _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveCharacterCommand>(y => y.Character.Equals(character)), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return null when the character could not be found.")]
  public async Task It_should_return_null_when_the_character_could_not_be_found()
  {
    RemoveCharacterBonusCommand command = new(Guid.Empty, Guid.Empty);
    command.Contextualize(_world);

    CharacterModel? character = await _handler.Handle(command, _cancellationToken);
    Assert.Null(character);
  }
}
