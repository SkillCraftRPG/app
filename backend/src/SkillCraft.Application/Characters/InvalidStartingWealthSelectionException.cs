using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;
using SkillCraft.Contracts.Items;
using SkillCraft.Domain.Items;

namespace SkillCraft.Application.Characters;

internal class InvalidStartingWealthSelectionException : BadRequestException
{
  private static readonly string ErrorMessage = $"The starting wealth item shall belong to the '{ItemCategory.Money}' category and have a value of 1.";

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
  public ItemCategory ItemCategory
  {
    get => (ItemCategory)Data[nameof(ItemCategory)]!;
    private set => Data[nameof(ItemCategory)] = value;
  }
  public double? ItemValue
  {
    get => (double?)Data[nameof(ItemValue)];
    private set => Data[nameof(ItemValue)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error => new PropertyError(this.GetErrorCode(), ErrorMessage, ItemId, PropertyName);

  public InvalidStartingWealthSelectionException(Item item, string propertyName) : base(BuildMessage(item, propertyName))
  {
    WorldId = item.WorldId.ToGuid();
    ItemId = item.EntityId;
    ItemCategory = item.Category;
    ItemValue = item.Value;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Item item, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), item.WorldId.ToGuid())
    .AddData(nameof(ItemId), item.EntityId)
    .AddData(nameof(ItemCategory), item.Category)
    .AddData(nameof(ItemValue), item.Value, "<null>")
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
