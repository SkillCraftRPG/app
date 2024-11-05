using SkillCraft.Contracts.Educations;

namespace SkillCraft.Tools.Seeding.Backend;

internal record EducationPayload : CreateOrReplaceEducationPayload
{
  public Guid Id { get; set; }
}
