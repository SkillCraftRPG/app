using Logitar.Portal.Contracts.Search;

namespace SkillCraft.Contracts.Castes;

public record SearchCastesPayload : SearchPayload
{
  public new List<CasteSortOption> Sort { get; set; } = [];
}
