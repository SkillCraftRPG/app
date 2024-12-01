namespace SkillCraft.Contracts.Characters;

public record CharacterStatisticModel
{
  public int Value { get; set; }
  public int Base { get; set; }
  public double Increment { get; set; }

  public CharacterStatisticModel()
  {
  }

  public CharacterStatisticModel(int value, int @base, double increment)
  {
    Value = value;
    Base = @base;
    Increment = increment;
  }
}
