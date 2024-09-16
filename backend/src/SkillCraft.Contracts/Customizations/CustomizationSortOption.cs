using Logitar.Portal.Contracts.Search;

namespace SkillCraft.Contracts.Customizations;

public record CustomizationSortOption : SortOption
{
  public new CustomizationSort Field
  {
    get => Enum.Parse<CustomizationSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public CustomizationSortOption(CustomizationSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
