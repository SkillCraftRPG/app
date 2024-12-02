using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain.Characters;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Application.Characters.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SaveCharacterBonusCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICharacterQuerier> _characterQuerier = new();
  private readonly Mock<ICharacterRepository> _characterRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();

  private readonly SaveCharacterBonusCommandHandler _handler;

  private readonly WorldMock _world = new();

  public SaveCharacterBonusCommandHandlerTests()
  {
    _handler = new(_characterQuerier.Object, _characterRepository.Object, _permissionService.Object, _sender.Object);
  }

  [Theory(DisplayName = "It should set a new character bonus.")]
  [InlineData(null)]
  [InlineData("dfe56839-dd36-454f-afa6-642447802276")]
  public async Task It_should_set_a_new_character_bonus(string? bonusIdValue)
  {
    Guid? bonusId = bonusIdValue == null ? null : Guid.Parse(bonusIdValue);

    Character character = new CharacterBuilder(_world).Build();
    _characterRepository.Setup(x => x.LoadAsync(character.Id, _cancellationToken)).ReturnsAsync(character);

    BonusPayload payload = new(BonusCategory.Skill, Skill.Melee.ToString(), value: +2)
    {
      IsTemporary = true,
      Precision = " Item: Belt of Stone Giant Strength ",
      Notes = "  Granted by the item \"Belt of Stone Giant Strength\"  "
    };
    SaveCharacterBonusCommand command = new(character.EntityId, bonusId, payload);
    command.Contextualize(_world);

    CharacterModel model = new();
    _characterQuerier.Setup(x => x.ReadAsync(character, _cancellationToken)).ReturnsAsync(model);

    CharacterModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    KeyValuePair<Guid, Bonus> bonus = Assert.Single(character.Bonuses);
    if (bonusId.HasValue)
    {
      Assert.Equal(bonusId.Value, bonus.Key);
    }
    else
    {
      Assert.NotEqual(default, bonus.Key);
    }
    Assert.Equal(payload.Category, bonus.Value.Category);
    Assert.Equal(payload.Target, bonus.Value.Target);
    Assert.Equal(payload.Value, bonus.Value.Value);
    Assert.Equal(payload.IsTemporary, bonus.Value.IsTemporary);
    Assert.Equal(payload.Precision.Trim(), bonus.Value.Precision?.Value);
    Assert.Equal(payload.Notes.Trim(), bonus.Value.Notes?.Value);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(command, character.GetMetadata(), _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveCharacterCommand>(y => y.Character.Equals(character)), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should replace an existing character bonus.")]
  public async Task It_should_replace_an_existing_character_bonus()
  {
    Character character = new CharacterBuilder(_world).Build();
    _characterRepository.Setup(x => x.LoadAsync(character.Id, _cancellationToken)).ReturnsAsync(character);

    Guid bonusId = Guid.NewGuid();
    Bonus bonus = new(BonusCategory.Skill, Skill.Melee.ToString(), value: +3);
    character.SetBonus(bonusId, bonus, _world.OwnerId);

    BonusPayload payload = new(BonusCategory.Attribute, Attribute.Agility.ToString(), value: +2)
    {
      IsTemporary = true,
      Precision = " Item: Belt of Stone Giant Strength ",
      Notes = "  Granted by the item \"Belt of Stone Giant Strength\"  "
    };
    SaveCharacterBonusCommand command = new(character.EntityId, bonusId, payload);
    command.Contextualize(_world);

    CharacterModel model = new();
    _characterQuerier.Setup(x => x.ReadAsync(character, _cancellationToken)).ReturnsAsync(model);

    CharacterModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    KeyValuePair<Guid, Bonus> pair = Assert.Single(character.Bonuses);
    Assert.Equal(bonusId, pair.Key);
    Assert.Equal(bonus.Category, pair.Value.Category);
    Assert.Equal(bonus.Target, pair.Value.Target);
    Assert.Equal(payload.Value, pair.Value.Value);
    Assert.Equal(payload.IsTemporary, pair.Value.IsTemporary);
    Assert.Equal(payload.Precision.Trim(), pair.Value.Precision?.Value);
    Assert.Equal(payload.Notes.Trim(), pair.Value.Notes?.Value);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(command, character.GetMetadata(), _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveCharacterCommand>(y => y.Character.Equals(character)), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return null when the character could not be found.")]
  public async Task It_should_return_null_when_the_character_could_not_be_found()
  {
    BonusPayload payload = new(BonusCategory.Skill, Skill.Melee.ToString(), value: +2);
    SaveCharacterBonusCommand command = new(Guid.Empty, Guid.Empty, payload);
    command.Contextualize(_world);

    CharacterModel? character = await _handler.Handle(command, _cancellationToken);
    Assert.Null(character);
  }
}
