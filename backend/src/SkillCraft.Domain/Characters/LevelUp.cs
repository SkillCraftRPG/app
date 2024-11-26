using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Domain.Characters;

public record LevelUp(Attribute Attribute, int Constitution, double Initiative, int Learning, double Power, double Precision, double Reputation, double Strength);
