using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Tools.Seeding.Backend;

internal record WorldPayload : CreateOrReplaceWorldPayload
{
  public Guid Id { get; set; }
}
