using Logitar.Portal.Contracts.Search;

namespace SkillCraft.Contracts.Items;

public record ItemSortOption : SortOption
{
  public new ItemSort Field
  {
    get => Enum.Parse<ItemSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public ItemSortOption(ItemSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
