using GraphQL.Types;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.GraphQL.Worlds;

internal class WorldSortGraphType : EnumerationGraphType<WorldSort>
{
  public WorldSortGraphType()
  {
    Name = nameof(WorldSort);
    Description = "Represents the available world fields for sorting.";

    AddValue(WorldSort.Name, "The worlds will be sorted by their display name.");
    AddValue(WorldSort.UpdatedOn, "The worlds will be sorted by their latest update date and time.");
  }
  private void AddValue(WorldSort value, string description)
  {
    Add(value.ToString(), value, description);
  }
}
