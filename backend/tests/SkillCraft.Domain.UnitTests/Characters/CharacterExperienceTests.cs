using SkillCraft.Domain.Worlds;

namespace SkillCraft.Domain.Characters;

[Trait(Traits.Category, Categories.Unit)]
public class CharacterExperienceTests
{
  private readonly WorldMock _world = new();
  private readonly Character _character;

  public CharacterExperienceTests()
  {
    _character = new CharacterBuilder().Build();
  }

  [Theory(DisplayName = "GainExperience: it should increase the character experience by a positive number.")]
  [InlineData(30)]
  public void GainExperience_it_should_increase_the_character_experience_by_a_positive_number(int experience)
  {
    int previousExperience = _character.Experience;

    _character.GainExperience(experience, _world.OwnerId);
    Assert.Equal(previousExperience + experience, _character.Experience);
    Assert.Contains(_character.Changes, change => change is Character.ExperienceGainedEvent e && e.Experience == experience);
  }

  [Theory(DisplayName = "GainExperience: it should throw ArgumentOutOfRangeException when experience gain was zero or negative.")]
  [InlineData(0)]
  [InlineData(-25)]
  public void GainExperience_it_should_throw_ArgumentOutOfRangeException_when_experience_gain_was_zero_or_negative(int experience)
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _character.GainExperience(experience, _world.OwnerId));
    Assert.Equal("experience", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentOutOfRangeException when setting the experience below current level total experience.")]
  public void It_should_throw_ArgumentOutOfRangeException_when_setting_the_experience_below_current_level_total_experience()
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _character.Experience = -100);
    Assert.Equal("Experience", exception.ParamName);
  }
}
