using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;
using SkillCraft.Domain.Personalities;

namespace SkillCraft.Application.Characters;

internal class CustomizationsCannotIncludePersonalityGiftException : BadRequestException
{
  private const string ErrorMessage = "The personality's gift should not be included in character customizations.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
  public Guid PersonalityId
  {
    get => (Guid)Data[nameof(PersonalityId)]!;
    private set => Data[nameof(PersonalityId)] = value;
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

  public CustomizationsCannotIncludePersonalityGiftException(Personality personality, string propertyName)
    : base(BuildMessage(personality, propertyName))
  {
    if (!personality.GiftId.HasValue)
    {
      throw new ArgumentException($"The '{nameof(personality.GiftId)}' is required.", nameof(personality));
    }

    WorldId = personality.WorldId.ToGuid();
    PersonalityId = personality.EntityId;
    GiftId = personality.GiftId.Value.EntityId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Personality personality, string propertyName)
  {
    if (!personality.GiftId.HasValue)
    {
      throw new ArgumentException($"The '{nameof(personality.GiftId)}' is required.", nameof(personality));
    }

    return new ErrorMessageBuilder(ErrorMessage)
      .AddData(nameof(WorldId), personality.WorldId.ToGuid())
      .AddData(nameof(PersonalityId), personality.EntityId)
      .AddData(nameof(GiftId), personality.GiftId.Value.EntityId)
      .AddData(nameof(PropertyName), propertyName)
      .Build();
  }
}
