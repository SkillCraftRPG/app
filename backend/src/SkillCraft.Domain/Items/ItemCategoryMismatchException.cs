using Logitar;
using SkillCraft.Contracts.Items;

namespace SkillCraft.Domain.Items;

internal class ItemCategoryMismatchException : Exception
{
  public const string ErrorMessage = "The specified item category was not expected.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
  public Guid ItemId
  {
    get => (Guid)Data[nameof(ItemId)]!;
    private set => Data[nameof(ItemId)] = value;
  }
  public ItemCategory ExpectedCategory
  {
    get => (ItemCategory)Data[nameof(ExpectedCategory)]!;
    private set => Data[nameof(ExpectedCategory)] = value;
  }
  public ItemCategory ActualCategory
  {
    get => (ItemCategory)Data[nameof(ActualCategory)]!;
    private set => Data[nameof(ActualCategory)] = value;
  }

  public ItemCategoryMismatchException(Item item, ItemCategory attemptedCategory)
    : base(BuildMessage(item, attemptedCategory))
  {
    WorldId = item.WorldId.ToGuid();
    ItemId = item.EntityId;
    ExpectedCategory = item.Category;
    ActualCategory = attemptedCategory;
  }

  private static string BuildMessage(Item item, ItemCategory attemptedCategory) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), item.WorldId.ToGuid())
    .AddData(nameof(ItemId), item.EntityId)
    .AddData(nameof(ExpectedCategory), item.Category)
    .AddData(nameof(ActualCategory), attemptedCategory)
    .Build();
}
