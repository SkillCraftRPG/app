namespace SkillCraft.Domain.Characters;

[Trait(Traits.Category, Categories.Unit)]
public class ExperienceTableTests
{
  [Theory(DisplayName = "GetTotalExperience: it should return the correct total experience.")]
  [InlineData(0, 0)]
  [InlineData(1, 100)]
  [InlineData(2, 400)]
  [InlineData(3, 1100)]
  [InlineData(4, 2400)]
  [InlineData(5, 4500)]
  [InlineData(6, 7600)]
  [InlineData(7, 11900)]
  [InlineData(8, 17600)]
  [InlineData(9, 24900)]
  [InlineData(10, 34000)]
  [InlineData(11, 45100)]
  [InlineData(12, 58400)]
  [InlineData(13, 74100)]
  [InlineData(14, 92400)]
  [InlineData(15, 113500)]
  [InlineData(16, 137600)]
  [InlineData(17, 164900)]
  [InlineData(18, 195600)]
  [InlineData(19, 229900)]
  [InlineData(20, 268000)]
  public void GetTotalExperience_it_should_return_the_correct_total_experience(int level, int experience)
  {
    Assert.Equal(experience, ExperienceTable.GetTotalExperience(level));
  }

  [Theory(DisplayName = "GetTotalExperience: it should throw ArgumentOutOfRangeException when the level is out of bounds.")]
  [InlineData(-1)]
  [InlineData(21)]
  public void GetTotalExperience_it_should_throw_ArgumentOutOfRangeException_when_the_level_is_out_of_bounds(int level)
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => ExperienceTable.GetTotalExperience(level));
    Assert.Equal("level", exception.ParamName);
  }
}
