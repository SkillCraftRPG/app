using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Domain.Characters;

[Trait(Traits.Category, Categories.Unit)]
public class CharacterBonusTests
{
  private readonly WorldMock _world = new();
  private readonly Character _character;

  public CharacterBonusTests()
  {
    _character = new CharacterBuilder().Build();
  }

  [Fact(DisplayName = "AddBonus: it should add a new bonus.")]
  public void AddBonus_it_should_add_a_new_bonus()
  {
    Assert.Empty(_character.Bonuses);

    Bonus bonus = new(BonusCategory.Miscellaneous, MiscellaneousBonusTarget.Stamina.ToString(), value: 5, precision: new Name("Talent : Discipline"));
    _character.AddBonus(bonus, _world.OwnerId);

    KeyValuePair<Guid, Bonus> pair = Assert.Single(_character.Bonuses);
    Assert.NotEqual(default, pair.Key);
    Assert.Same(bonus, pair.Value);

    Assert.Contains(_character.Changes, c => c is Character.BonusUpdatedEvent e && e.BonusId != default && e.Bonus.Equals(bonus));
  }

  [Fact(DisplayName = "RemoveBonus: it should not do anything when the bonus was not found.")]
  public void RemoveBonus_it_should_not_do_anything_when_the_bonus_was_not_found()
  {
    Bonus bonus = new(BonusCategory.Speed, SpeedKind.Swim.ToString(), value: 6);
    _character.AddBonus(bonus, _world.OwnerId);

    _character.ClearChanges();
    Assert.False(_character.HasChanges);
    Assert.Empty(_character.Changes);

    _character.RemoveBonus(Guid.NewGuid(), _world.OwnerId);

    _character.ClearChanges();
    Assert.False(_character.HasChanges);
    Assert.Empty(_character.Changes);

    Assert.Same(bonus, Assert.Single(_character.Bonuses).Value);
  }

  [Fact(DisplayName = "RemoveBonus: it should remove an bonus language.")]
  public void RemoveBonus_it_should_remove_an_existing_bonus()
  {
    Guid bonusId = Guid.NewGuid();
    Bonus bonus = new(BonusCategory.Miscellaneous, MiscellaneousBonusTarget.Dodge.ToString(), value: +2);
    _character.SetBonus(bonusId, bonus, _world.OwnerId);
    Assert.NotEmpty(_character.Bonuses);

    _character.RemoveBonus(bonusId, _world.OwnerId);
    Assert.Empty(_character.Bonuses);

    Assert.Contains(_character.Changes, change => change is Character.BonusRemovedEvent e && e.BonusId == bonusId);
  }

  [Fact(DisplayName = "SetBonus: it should not do anything when the bonus has not changed.")]
  public void SetBonus_it_should_not_do_anything_when_the_bonus_has_not_changed()
  {
    Assert.Empty(_character.Bonuses);

    Guid bonusId = Guid.NewGuid();
    Bonus bonus = new(BonusCategory.Statistic, Statistic.Learning.ToString(), value: +4);
    _character.SetBonus(bonusId, bonus, _world.OwnerId);

    _character.ClearChanges();
    Assert.False(_character.HasChanges);
    Assert.Empty(_character.Changes);

    _character.SetBonus(bonusId, bonus, _world.OwnerId);
    Assert.False(_character.HasChanges);
    Assert.Empty(_character.Changes);
  }

  [Fact(DisplayName = "SetBonus: it should replace an existing bonus.")]
  public void SetBonus_it_should_replace_an_existing_bonus()
  {
    Assert.Empty(_character.Bonuses);

    Guid bonusId = Guid.NewGuid();

    Bonus oldBonus = new(BonusCategory.Skill, Skill.Performance.ToString(), value: +1);
    _character.SetBonus(bonusId, oldBonus, _world.OwnerId);
    Assert.Contains(_character.Changes, c => c is Character.BonusUpdatedEvent e && e.BonusId == bonusId && e.Bonus.Equals(oldBonus));

    Bonus newBonus = new(BonusCategory.Skill, Skill.Perception.ToString(), value: +2, precision: new Name("Hat of Performance"));
    _character.SetBonus(bonusId, newBonus, _world.OwnerId);
    Assert.Contains(_character.Changes, c => c is Character.BonusUpdatedEvent e && e.BonusId == bonusId && e.Bonus.Equals(newBonus));

    KeyValuePair<Guid, Bonus> pair = Assert.Single(_character.Bonuses);
    Assert.Equal(bonusId, pair.Key);
    Assert.Same(newBonus, pair.Value);
  }

  [Fact(DisplayName = "SetBonus: it should set a new bonus.")]
  public void SetBonus_it_should_set_a_new_bonus()
  {
    Assert.Empty(_character.Bonuses);

    Guid bonusId = Guid.NewGuid();
    Bonus bonus = new(BonusCategory.Miscellaneous, MiscellaneousBonusTarget.Stamina.ToString(), value: 5, precision: new Name("Talent : Discipline"));
    _character.SetBonus(bonusId, bonus, _world.OwnerId);

    KeyValuePair<Guid, Bonus> pair = Assert.Single(_character.Bonuses);
    Assert.Equal(bonusId, pair.Key);
    Assert.Same(bonus, pair.Value);

    Assert.Contains(_character.Changes, c => c is Character.BonusUpdatedEvent e && e.BonusId == bonusId && e.Bonus.Equals(bonus));
  }
}
