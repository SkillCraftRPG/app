using SkillCraft.Domain.Talents;

namespace SkillCraft.Domain.Characters;

public record CharacterTalent(TalentId TalentId, int Cost, Name? Precision, Description? Notes);
