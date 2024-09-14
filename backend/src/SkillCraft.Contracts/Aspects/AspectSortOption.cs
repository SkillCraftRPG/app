using Logitar.Portal.Contracts.Search;

namespace SkillCraft.Contracts.Aspects;

public record AspectSortOption : SortOption
{
  public new AspectSort Field
  {
    get => Enum.Parse<AspectSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public AspectSortOption(AspectSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
