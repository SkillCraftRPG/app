namespace SkillCraft.Contracts.Characters;

public interface ILevelUp
{
  Attribute Attribute { get; }

  int Constitution { get; }
  double Initiative { get; }
  int Learning { get; }
  double Power { get; }
  double Precision { get; }
  double Reputation { get; }
  double Strength { get; }
}
