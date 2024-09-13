using Logitar.Portal.Contracts.Search;

namespace SkillCraft.Contracts.Educations;

public record SearchEducationsPayload : SearchPayload
{
  public new List<EducationSortOption> Sort { get; set; } = [];
}
