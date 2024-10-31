using SkillCraft.Domain.Talents;

namespace SkillCraft.Domain.Characters;

public record CharacterTalent(TalentId Id, int Cost, Name? Precision, Description? Notes);
