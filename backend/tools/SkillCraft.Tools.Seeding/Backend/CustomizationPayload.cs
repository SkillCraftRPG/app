using SkillCraft.Contracts.Customizations;

namespace SkillCraft.Tools.Seeding.Backend;

internal record CustomizationPayload : CreateOrReplaceCustomizationPayload
{
  public Guid Id { get; set; }
}
