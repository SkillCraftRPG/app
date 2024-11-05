using SkillCraft.Contracts.Aspects;

namespace SkillCraft.Tools.Seeding.Backend;

internal record AspectPayload : CreateOrReplaceAspectPayload
{
  public Guid Id { get; set; }
}
