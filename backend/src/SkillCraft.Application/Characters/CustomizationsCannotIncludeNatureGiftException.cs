using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;
using SkillCraft.Domain.Natures;

namespace SkillCraft.Application.Characters;

internal class CustomizationsCannotIncludeNatureGiftException : BadRequestException
{
  private const string ErrorMessage = "The nature's gift should not be included in character customizations.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
  public Guid NatureId
  {
    get => (Guid)Data[nameof(NatureId)]!;
    private set => Data[nameof(NatureId)] = value;
  }
  public Guid GiftId
  {
    get => (Guid)Data[nameof(GiftId)]!;
    private set => Data[nameof(GiftId)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error => new PropertyError(this.GetErrorCode(), ErrorMessage, GiftId, PropertyName);

  public CustomizationsCannotIncludeNatureGiftException(Nature nature, string propertyName)
    : base(BuildMessage(nature, propertyName))
  {
    if (!nature.GiftId.HasValue)
    {
      throw new ArgumentException($"The '{nameof(nature.GiftId)}' is required.", nameof(nature));
    }

    WorldId = nature.WorldId.ToGuid();
    NatureId = nature.EntityId;
    GiftId = nature.GiftId.Value.EntityId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Nature nature, string propertyName)
  {
    if (!nature.GiftId.HasValue)
    {
      throw new ArgumentException($"The '{nameof(nature.GiftId)}' is required.", nameof(nature));
    }

    return new ErrorMessageBuilder(ErrorMessage)
      .AddData(nameof(WorldId), nature.WorldId.ToGuid())
      .AddData(nameof(NatureId), nature.EntityId)
      .AddData(nameof(GiftId), nature.GiftId.Value.EntityId)
      .AddData(nameof(PropertyName), propertyName)
      .Build();
  }
}
