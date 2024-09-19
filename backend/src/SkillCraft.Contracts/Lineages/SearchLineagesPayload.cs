using Logitar.Portal.Contracts.Search;

namespace SkillCraft.Contracts.Lineages;

public record SearchLineagesPayload : SearchPayload
{
  public Guid? ParentId { get; set; }

  public new List<LineageSortOption> Sort { get; set; } = [];
}
