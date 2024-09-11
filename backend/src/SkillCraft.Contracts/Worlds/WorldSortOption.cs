using Logitar.Portal.Contracts.Search;

namespace SkillCraft.Contracts.Worlds;

public record WorldSortOption : SortOption
{
  public new WorldSort Field
  {
    get => Enum.Parse<WorldSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public WorldSortOption(WorldSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
