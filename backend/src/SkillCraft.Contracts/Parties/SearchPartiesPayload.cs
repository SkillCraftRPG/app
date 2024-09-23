using Logitar.Portal.Contracts.Search;

namespace SkillCraft.Contracts.Parties;

public record SearchPartiesPayload : SearchPayload
{
  public new List<PartySortOption> Sort { get; set; } = [];
}
