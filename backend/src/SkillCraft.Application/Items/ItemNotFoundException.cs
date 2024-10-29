using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;
using SkillCraft.Domain.Items;

namespace SkillCraft.Application.Items;

internal class ItemNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified item could not be found.";

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
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error => new PropertyError(this.GetErrorCode(), ErrorMessage, ItemId, PropertyName);

  public ItemNotFoundException(ItemId itemId, string propertyName)
    : base(BuildMessage(itemId, propertyName))
  {
    WorldId = itemId.WorldId.ToGuid();
    ItemId = itemId.EntityId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(ItemId itemId, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), itemId.WorldId.ToGuid())
    .AddData(nameof(ItemId), itemId.EntityId)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
