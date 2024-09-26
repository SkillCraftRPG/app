using GraphQL.Types;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.GraphQL.Worlds;

internal class WorldSortGraphType : EnumerationGraphType<WorldSort>
{
  public WorldSortGraphType()
  {
    Name = nameof(WorldSort);
    Description = "Represents the available world fields for sorting.";

    AddValue(WorldSort.CreatedOn, "The worlds will be sorted by their creation date and time.");
    AddValue(WorldSort.Name, "The worlds will be sorted by their display name.");
    AddValue(WorldSort.Slug, "The worlds will be sorted by their slug.");
    AddValue(WorldSort.UpdatedOn, "The worlds will be sorted by their latest update date and time.");
  }
  private void AddValue(WorldSort value, string description)
  {
    Add(value.ToString(), value, description);
  }
}
