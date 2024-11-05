using SkillCraft.Contracts.Items;

namespace SkillCraft.Tools.Seeding.Backend;

internal record ItemPayload : CreateOrReplaceItemPayload
{
  public Guid Id { get; set; }
}
