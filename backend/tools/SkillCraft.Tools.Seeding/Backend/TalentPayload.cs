using SkillCraft.Contracts.Talents;

namespace SkillCraft.Tools.Seeding.Backend;

internal record TalentPayload : CreateOrReplaceTalentPayload
{
  public Guid Id { get; set; }
}
