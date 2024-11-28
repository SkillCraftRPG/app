namespace SkillCraft.Domain.Characters;

public record CharacterStatistic
{
  public int Value { get; }
  public double Increment { get; }

  public CharacterStatistic(int @base, double increment, double sum, int bonuses)
  {
    Value = @base + (int)Math.Floor(sum) + bonuses;
    Increment = increment;
  }
}
