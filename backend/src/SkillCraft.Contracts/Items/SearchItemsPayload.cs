using Logitar.Portal.Contracts.Search;

namespace SkillCraft.Contracts.Items;

public record SearchItemsPayload : SearchPayload
{
  public ItemCategory? Category { get; set; }

  public new List<ItemSortOption> Sort { get; set; } = [];
}
