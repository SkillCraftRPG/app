using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;
using SkillCraft.Domain.Talents;

namespace SkillCraft.Application.Talents;

internal class InvalidRequiredTalentTierException : BadRequestException
{
  private const string ErrorMessage = "The required talent tier is higher than the requiring talent tier.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
  public Guid RequiringTalentId
  {
    get => (Guid)Data[nameof(RequiringTalentId)]!;
    private set => Data[nameof(RequiringTalentId)] = value;
  }
  public Guid RequiredTalentId
  {
    get => (Guid)Data[nameof(RequiredTalentId)]!;
    private set => Data[nameof(RequiredTalentId)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error => new PropertyError(this.GetErrorCode(), ErrorMessage, RequiredTalentId, PropertyName);

  public InvalidRequiredTalentTierException(Talent requiringTalent, Talent requiredTalent, string propertyName)
    : base(BuildMessage(requiringTalent, requiredTalent, propertyName))
  {
    WorldId = requiringTalent.WorldId.ToGuid();
    RequiringTalentId = requiringTalent.EntityId;
    RequiredTalentId = requiredTalent.EntityId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Talent requiringTalent, Talent requiredTalent, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), requiringTalent.WorldId.ToGuid())
    .AddData(nameof(RequiringTalentId), requiringTalent.EntityId)
    .AddData(nameof(RequiredTalentId), requiredTalent.EntityId)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
