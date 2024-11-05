using SkillCraft.Contracts.Languages;

namespace SkillCraft.Tools.Seeding.Backend;

internal record LanguagePayload : CreateOrReplaceLanguagePayload
{
  public Guid Id { get; set; }
}
