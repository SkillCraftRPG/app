namespace SkillCraft.Contracts.Characters;

public interface IBonus
{
  BonusCategory Category { get; }
  string Target { get; }
  int Value { get; }
}
