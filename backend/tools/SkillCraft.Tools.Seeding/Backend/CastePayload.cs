using SkillCraft.Contracts.Castes;

namespace SkillCraft.Tools.Seeding.Backend;

internal record CastePayload : CreateOrReplaceCastePayload
{
  public Guid Id { get; set; }
}
