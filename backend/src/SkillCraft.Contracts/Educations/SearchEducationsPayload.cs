using Logitar.Portal.Contracts.Search;

namespace SkillCraft.Contracts.Educations;

public record SearchEducationsPayload : SearchPayload
{
  public Skill? Skill { get; set; }

  public new List<EducationSortOption> Sort { get; set; } = [];
}
