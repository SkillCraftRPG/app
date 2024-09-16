using Logitar.Portal.Contracts.Search;

namespace SkillCraft.Contracts.Lineages;

public record SearchLineagesPayload : SearchPayload
{
  public new List<LineageSortOption> Sort { get; set; } = [];
}
