using Logitar.Portal.Contracts.Search;

namespace SkillCraft.Contracts.Talents;

public record SearchTalentsPayload : SearchPayload
{
  public bool? AllowMultiplePurchases { get; set; }
  public bool? HasSkill { get; set; }
  public Guid? RequiredTalentId { get; set; }
  public TierFilter? Tier { get; set; }

  public new List<TalentSortOption> Sort { get; set; } = [];
}
