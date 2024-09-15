using Logitar.Portal.Contracts.Search;

namespace SkillCraft.Contracts.Aspects;

public record SearchAspectsPayload : SearchPayload
{
  public new List<AspectSortOption> Sort { get; set; } = [];
}
