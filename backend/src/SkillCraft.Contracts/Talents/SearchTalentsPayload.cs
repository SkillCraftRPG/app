using Logitar.Portal.Contracts.Search;

namespace SkillCraft.Contracts.Talents;

public record SearchTalentsPayload : SearchPayload
{
  public new List<TalentSortOption> Sort { get; set; } = [];
}
