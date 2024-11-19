using SkillCraft.Contracts.Lineages;

namespace SkillCraft.Tools.Seeding.Backend;

internal record LineagePayload : CreateOrReplaceLineagePayload
{
  public Guid Id { get; set; }
}
