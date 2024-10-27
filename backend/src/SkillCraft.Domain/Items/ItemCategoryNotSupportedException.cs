using Logitar;
using SkillCraft.Contracts.Items;

namespace SkillCraft.Domain.Items;

internal class ItemCategoryNotSupportedException : NotSupportedException
{
  private const string ErrorMessage = "The specified item category is not supported.";

  public ItemCategory ItemCategory
  {
    get => (ItemCategory)Data[nameof(ItemCategory)]!;
    private set => Data[nameof(ItemCategory)] = value;
  }

  public ItemCategoryNotSupportedException(ItemCategory itemCategory) : base(BuildMessage(itemCategory))
  {
    ItemCategory = itemCategory;
  }

  private static string BuildMessage(ItemCategory itemCategory) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(ItemCategory), itemCategory)
    .Build();
}
