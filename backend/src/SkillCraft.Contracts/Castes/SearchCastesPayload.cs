using Logitar.Portal.Contracts.Search;

namespace SkillCraft.Contracts.Castes;

public record SearchCastesPayload : SearchPayload
{
  public Skill? Skill { get; set; }

  public new List<CasteSortOption> Sort { get; set; } = [];
}
