namespace SkillCraft.Domain.Characters;

public record CharacterStatistic
{
  public int Value { get; }
  public int Base { get; }
  public double Increment { get; }

  public CharacterStatistic(int value, int @base, double increment)
  {
    Value = value;
    Base = @base;
    Increment = increment;
  }
}
