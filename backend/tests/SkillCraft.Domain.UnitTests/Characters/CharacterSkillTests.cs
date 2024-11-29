using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Domain.Characters;

[Trait(Traits.Category, Categories.Unit)]
public class CharacterSkillTests
{
  private readonly WorldMock _world = new();
  private readonly Character _character;

  public CharacterSkillTests()
  {
    _character = new CharacterBuilder(_world).Build();
  }

  [Fact(DisplayName = "IncreaseSkillRank: it should increase the rank of the specified skill.")]
  public void IncreaseSkillRank_it_should_increase_the_rank_of_the_specified_skill()
  {
    _character.IncreaseSkillRank(Skill.Athletics, _world.OwnerId);
    Assert.Equal(1, _character.SkillRanks[Skill.Athletics]);
  }

  [Fact(DisplayName = "IncreaseSkillRank: it should throw ArgumentOutOfRangeException when the skill is not defined.")]
  public void IncreaseSkillRank_it_should_throw_ArgumentOutOfRangeException_when_the_skill_is_not_defined()
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _character.IncreaseSkillRank((Skill)(-1), _world.OwnerId));
    Assert.Equal("skill", exception.ParamName);
  }

  [Fact(DisplayName = "IncreaseSkillRank: it should throw NotEnoughRemainingSkillPointsException when the character has no remaining skill point.")]
  public void IncreaseSkillRank_it_should_throw_NotEnoughRemainingSkillPointsException_when_the_character_has_no_remaining_skill_point()
  {
    _character.IncreaseSkillRank(Skill.Acrobatics, _world.OwnerId);
    _character.IncreaseSkillRank(Skill.Acrobatics, _world.OwnerId);
    _character.IncreaseSkillRank(Skill.Athletics, _world.OwnerId);
    _character.IncreaseSkillRank(Skill.Athletics, _world.OwnerId);
    _character.IncreaseSkillRank(Skill.Resistance, _world.OwnerId);

    var exception = Assert.Throws<NotEnoughRemainingSkillPointsException>(() => _character.IncreaseSkillRank(Skill.Resistance, _world.OwnerId));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(_character.EntityId, exception.CharacterId);
  }

  [Fact(DisplayName = "IncreaseSkillRank: it should throw SkillMaximumRankReachedException when the skill maximum rank has been reached.")]
  public void IncreaseSkillRank_it_should_throw_SkillMaximumRankReachedException_when_the_skill_maximum_rank_has_been_reached()
  {
    _character.IncreaseSkillRank(Skill.Athletics, _world.OwnerId);
    _character.IncreaseSkillRank(Skill.Athletics, _world.OwnerId);

    var exception = Assert.Throws<SkillMaximumRankReachedException>(() => _character.IncreaseSkillRank(Skill.Athletics, _world.OwnerId));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(_character.EntityId, exception.CharacterId);
    Assert.Equal(_character.MaximumSkillRank, exception.MaximumSkillRank);
    Assert.Equal(Skill.Athletics, exception.Skill);
    Assert.Equal("Skill", exception.PropertyName);
  }

  [Fact(DisplayName = "It should account for correct skill points.")]
  public void It_should_account_for_correct_skill_points()
  {
    _character.AddBonus(new Bonus(BonusCategory.Statistic, Statistic.Learning.ToString(), value: +4, notes: new Description("Apprentissage accéléré")), _world.OwnerId);

    Assert.Equal(9, _character.AvailableSkillPoints);
    Assert.Equal(0, _character.SpentSkillPoints);
    Assert.Equal(9, _character.RemainingSkillPoints);

    _character.IncreaseSkillRank(Skill.Acrobatics, _world.OwnerId);
    _character.IncreaseSkillRank(Skill.Acrobatics, _world.OwnerId);
    _character.IncreaseSkillRank(Skill.Athletics, _world.OwnerId);
    _character.IncreaseSkillRank(Skill.Athletics, _world.OwnerId);
    _character.IncreaseSkillRank(Skill.Melee, _world.OwnerId);
    _character.IncreaseSkillRank(Skill.Melee, _world.OwnerId);
    _character.IncreaseSkillRank(Skill.Survival, _world.OwnerId);
    _character.IncreaseSkillRank(Skill.Survival, _world.OwnerId);

    Assert.Equal(9, _character.AvailableSkillPoints);
    Assert.Equal(8, _character.SpentSkillPoints);
    Assert.Equal(1, _character.RemainingSkillPoints);
  }

  [Theory(DisplayName = "MaximumSkillRank: it should return the correct maximum rank.")]
  [InlineData(0, 2)]
  public void MaximumSkillRank_it_should_return_the_correct_maximum_rank(int tier, int maximumRank)
  {
    Assert.Equal(0, tier); // NOTE(fpion): reserved for future use.

    Assert.Equal(maximumRank, _character.MaximumSkillRank);
  }

  [Theory(DisplayName = "SetSkillRank: it should set a character skill rank.")]
  [InlineData(Skill.Discipline, 0)]
  [InlineData(Skill.Discipline, 2)]
  public void SetSkillRank_it_should_set_a_character_skill_rank(Skill skill, int rank)
  {
    _character.SetSkillRank(skill, rank);
    _character.Update(_world.OwnerId);

    if (rank == 0)
    {
      Assert.Empty(_character.SkillRanks);
    }
    else
    {
      KeyValuePair<Skill, int> skillRank = Assert.Single(_character.SkillRanks);
      Assert.Equal(skill, skillRank.Key);
      Assert.Equal(rank, skillRank.Value);
    }
  }

  [Fact(DisplayName = "SetSkillRank: it should throw ArgumentOutOfRangeException when the skill is negative.")]
  public void SetSkillRank_it_should_throw_ArgumentOutOfRangeException_when_the_skill_is_negative()
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _character.SetSkillRank(Skill.Performance, rank: -2));
    Assert.Equal("rank", exception.ParamName);
  }

  [Fact(DisplayName = "SetSkillRank: it should throw ArgumentOutOfRangeException when the skill is not defined.")]
  public void SetSkillRank_it_should_throw_ArgumentOutOfRangeException_when_the_skill_is_not_defined()
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _character.SetSkillRank((Skill)(-1), rank: 0));
    Assert.Equal("skill", exception.ParamName);
  }

  [Fact(DisplayName = "SetSkillRank: it should throw NotEnoughRemainingSkillPointsException when the character does not have any remaining skill point.")]
  public void SetSkillRank_it_should_throw_NotEnoughRemainingSkillPointsException_when_the_character_does_not_have_any_remaining_skill_point()
  {
    _character.IncreaseSkillRank(Skill.Acrobatics, _world.OwnerId);
    _character.IncreaseSkillRank(Skill.Acrobatics, _world.OwnerId);
    _character.IncreaseSkillRank(Skill.Athletics, _world.OwnerId);
    _character.IncreaseSkillRank(Skill.Athletics, _world.OwnerId);
    _character.IncreaseSkillRank(Skill.Resistance, _world.OwnerId);

    var exception = Assert.Throws<NotEnoughRemainingSkillPointsException>(() => _character.SetSkillRank(Skill.Resistance, rank: 2));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(_character.EntityId, exception.CharacterId);
  }

  [Fact(DisplayName = "SetSkillRank: it should throw SkillMaximumRankReachedException when the skill exceeds the character maximum rank.")]
  public void SetSkillRank_it_should_throw_SkillMaximumRankReachedException_when_the_skill_exceeds_the_character_maximum_rank()
  {
    var exception = Assert.Throws<SkillMaximumRankReachedException>(() => _character.SetSkillRank(Skill.Resistance, rank: 3));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(_character.EntityId, exception.CharacterId);
    Assert.Equal(_character.MaximumSkillRank, exception.MaximumSkillRank);
    Assert.Equal(Skill.Resistance, exception.Skill);
    Assert.Equal("Skill", exception.PropertyName);
  }
}
