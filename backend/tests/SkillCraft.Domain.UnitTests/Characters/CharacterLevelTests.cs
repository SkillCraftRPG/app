using SkillCraft.Contracts.Characters;
using SkillCraft.Domain.Worlds;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Domain.Characters;

[Trait(Traits.Category, Categories.Unit)]
public class CharacterLevelTests
{
  private readonly WorldMock _world = new();
  private readonly Character _character;

  public CharacterLevelTests()
  {
    _character = new CharacterBuilder(_world).Build();
  }

  [Fact(DisplayName = "CanLevelUp: it should return false when the character cannot level-up yet.")]
  public void CanLevelUp_it_should_return_false_when_the_character_cannot_level_up_yet()
  {
    Assert.False(_character.CanLevelUp);

    _character.GainExperience(50, _world.OwnerId);
    Assert.False(_character.CanLevelUp);
  }

  [Fact(DisplayName = "CanLevelUp: it should return true when the character can level-up.")]
  public void CanLevelUp_it_should_return_true_when_the_character_can_level_up()
  {
    Assert.False(_character.CanLevelUp);

    _character.GainExperience(500, _world.OwnerId);
    Assert.True(_character.CanLevelUp);
  }

  [Fact(DisplayName = "CancelLevelUp: it should not do anything when there is no level-up.")]
  public void CancelLevelUp_it_should_not_do_anything_when_there_is_no_level_up()
  {
    _character.ClearChanges();
    Assert.False(_character.HasChanges);
    Assert.Empty(_character.Changes);

    _character.CancelLevelUp(_world.OwnerId);
    Assert.False(_character.HasChanges);
    Assert.Empty(_character.Changes);
  }

  [Fact(DisplayName = "CancelLevelUp: it should cancel the last level-up.")]
  public void CancelLevelUp_it_should_cancel_the_last_level_up()
  {
    _character.GainExperience(ExperienceTable.GetTotalExperience(_character.Level + 1), _world.OwnerId);
    Assert.True(_character.CanLevelUp);

    _character.LevelUp(Attribute.Agility, _world.OwnerId);
    _character.CancelLevelUp(_world.OwnerId);
    Assert.Empty(_character.LevelUps);
    Assert.Equal(0, _character.Level);

    Assert.Equal(16, _character.Attributes.Agility.Score);
  }

  [Fact(DisplayName = "LevelUp: it should level-up the character.")]
  public void LevelUp_it_should_level_up_the_character()
  {
    _character.GainExperience(ExperienceTable.GetTotalExperience(_character.Level + 1), _world.OwnerId);
    Assert.True(_character.CanLevelUp);

    _character.LevelUp(Attribute.Agility, _world.OwnerId);
    Assert.Equal(1, _character.Level);

    LevelUp levelUp = Assert.Single(_character.LevelUps);
    Assert.Equal(Attribute.Agility, levelUp.Attribute);
    Assert.Equal(7, levelUp.Constitution);
    Assert.Equal(0.2, levelUp.Initiative);
    Assert.Equal(1, levelUp.Learning);
    Assert.Equal(0.15, levelUp.Power);
    Assert.Equal(0.25, levelUp.Precision);
    Assert.Equal(0.5, levelUp.Reputation);
    Assert.Equal(0.425, levelUp.Strength);

    Assert.Equal(17, _character.Attributes.Agility.Score);

    Assert.Equal(42, _character.Statistics.Constitution.Value);
    Assert.Equal(-1, _character.Statistics.Initiative.Value);
    Assert.Equal(6, _character.Statistics.Learning.Value);
    Assert.Equal(-2, _character.Statistics.Power.Value);
    Assert.Equal(0, _character.Statistics.Precision.Value);
    Assert.Equal(0, _character.Statistics.Reputation.Value);
    Assert.Equal(3, _character.Statistics.Strength.Value);
  }

  [Fact(DisplayName = "LevelUp: it should throw ArgumentOutOfRangeException when the attribute is not defined.")]
  public void LevelUp_it_should_throw_ArgumentOutOfRangeException_when_the_attribute_is_not_defined()
  {
    _character.GainExperience(ExperienceTable.GetTotalExperience(_character.Level + 1), _world.OwnerId);
    Assert.True(_character.CanLevelUp);

    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _character.LevelUp((Attribute)(-1), _world.OwnerId));
    Assert.Equal("attribute", exception.ParamName);
  }

  [Fact(DisplayName = "LevelUp: it should throw CharacterCannotLevelUpYetException when the character cannot level-up.")]
  public void LevelUp_it_should_throw_CharacterCannotLevelUpYetException_when_the_character_cannot_level_up()
  {
    _character.GainExperience(75, _world.OwnerId);
    Assert.True(_character.Experience < ExperienceTable.GetTotalExperience(_character.Level + 1));

    var exception = Assert.Throws<CharacterCannotLevelUpYetException>(() => _character.LevelUp(Attribute.Agility, _world.OwnerId));
    Assert.Equal(_character.WorldId.ToGuid(), exception.WorldId);
    Assert.Equal(_character.EntityId, exception.CharacterId);
    Assert.Equal(_character.Experience, exception.CurrentExperience);
    Assert.Equal(_character.Level, exception.CurrentLevel);
    Assert.Equal(ExperienceTable.GetTotalExperience(_character.Level + 1), exception.RequiredExperience);
  }

  [Fact(DisplayName = "LevelUp: it should throw AttributeMaximumScoreReachedException when the attribute score is already superior or equal to 20.")]
  public void LevelUp_it_should_throw_AttributeMaximumScoreReachedException_when_the_attribute_score_is_already_superior_or_equal_to_20()
  {
    _character.AddBonus(new Bonus(BonusCategory.Attribute, Attribute.Agility.ToString(), value: 4), _world.OwnerId);
    Assert.Equal(20, _character.Attributes.Agility.Score);

    _character.GainExperience(ExperienceTable.GetTotalExperience(_character.Level + 1), _world.OwnerId);
    Assert.True(_character.CanLevelUp);

    var exception = Assert.Throws<AttributeMaximumScoreReachedException>(() => _character.LevelUp(Attribute.Agility, _world.OwnerId));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(_character.EntityId, exception.CharacterId);
    Assert.Equal(Attribute.Agility, exception.Attribute);
    Assert.Equal("Attribute", exception.PropertyName);
  }
}
