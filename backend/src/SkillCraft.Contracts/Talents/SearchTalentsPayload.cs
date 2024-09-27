using Logitar.Portal.Contracts.Search;

namespace SkillCraft.Contracts.Talents;

public record SearchTalentsPayload : SearchPayload
{
  // TODO(fpion): Tier
  public bool? AllowMultiplePurchases { get; set; }
  public bool? HasSkill { get; set; }
  // TODO(fpion): RequiredTalent/RequiringTalents

  public new List<TalentSortOption> Sort { get; set; } = [];
}
