using Logitar.Portal.Contracts.Search;

namespace SkillCraft.Contracts.Lineages;

public record SearchLineagesPayload : SearchPayload
{
  // TODO(fpion): ParentId

  public new List<LineageSortOption> Sort { get; set; } = [];
}
