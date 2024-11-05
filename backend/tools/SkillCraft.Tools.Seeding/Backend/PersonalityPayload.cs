using SkillCraft.Contracts.Personalities;

namespace SkillCraft.Tools.Seeding.Backend;

internal record PersonalityPayload : CreateOrReplacePersonalityPayload
{
  public Guid Id { get; set; }
}
