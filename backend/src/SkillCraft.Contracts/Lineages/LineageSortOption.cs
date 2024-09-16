using Logitar.Portal.Contracts.Search;

namespace SkillCraft.Contracts.Lineages;

public record LineageSortOption : SortOption
{
  public new LineageSort Field
  {
    get => Enum.Parse<LineageSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public LineageSortOption(LineageSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
