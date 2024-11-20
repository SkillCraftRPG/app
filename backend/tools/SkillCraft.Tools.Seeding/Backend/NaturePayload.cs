using SkillCraft.Contracts.Natures;

namespace SkillCraft.Tools.Seeding.Backend;

internal record NaturePayload : CreateOrReplaceNaturePayload
{
  public Guid Id { get; set; }
}
