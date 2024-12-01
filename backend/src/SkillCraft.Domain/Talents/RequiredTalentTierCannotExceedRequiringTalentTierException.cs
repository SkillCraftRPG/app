using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;

namespace SkillCraft.Domain.Talents;

public class RequiredTalentTierCannotExceedRequiringTalentTierException : DomainException
{
  private const string ErrorMessage = "The tier of the required talent cannot exceed the tier of the requiring talent.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
  public Guid RequiredTalentId
  {
    get => (Guid)Data[nameof(RequiredTalentId)]!;
    private set => Data[nameof(RequiredTalentId)] = value;
  }
  public int RequiredTalentTier
  {
    get => (int)Data[nameof(RequiredTalentTier)]!;
    private set => Data[nameof(RequiredTalentTier)] = value;
  }
  public Guid RequiringTalentId
  {
    get => (Guid)Data[nameof(RequiringTalentId)]!;
    private set => Data[nameof(RequiringTalentId)] = value;
  }
  public int RequiringTalentTier
  {
    get => (int)Data[nameof(RequiringTalentTier)]!;
    private set => Data[nameof(RequiringTalentTier)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error
  {
    get
    {
      PropertyError error = new(this.GetErrorCode(), ErrorMessage, RequiredTalentId, PropertyName);
      error.AddData(nameof(RequiredTalentTier), RequiredTalentTier.ToString());
      return error;
    }
  }

  public RequiredTalentTierCannotExceedRequiringTalentTierException(Talent requiredTalent, Talent requiringTalent, string propertyName)
    : base(BuildMessage(requiredTalent, requiringTalent, propertyName))
  {
    WorldId = GetWorldId(requiredTalent, requiringTalent);
    RequiredTalentId = requiredTalent.EntityId;
    RequiredTalentTier = requiredTalent.Tier;
    RequiringTalentId = requiringTalent.EntityId;
    RequiringTalentTier = requiringTalent.Tier;
    PropertyName = propertyName;
  }

  private static Guid GetWorldId(params Talent[] talents) => talents.Select(talent => talent.WorldId).Distinct().Single().ToGuid();

  private static string BuildMessage(Talent requiredTalent, Talent requiringTalent, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), GetWorldId(requiredTalent, requiringTalent))
    .AddData(nameof(RequiredTalentId), requiredTalent.EntityId)
    .AddData(nameof(RequiredTalentTier), requiredTalent.Tier)
    .AddData(nameof(RequiringTalentId), requiringTalent.EntityId)
    .AddData(nameof(RequiringTalentTier), requiringTalent.Tier)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
