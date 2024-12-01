using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;

namespace SkillCraft.Domain.Talents;

public class TalentCannotBePurchasedMultipleTimesException : DomainException
{
  private const string ErrorMessage = "The specified talent cannot be purchased multiple times.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
  public Guid TalentId
  {
    get => (Guid)Data[nameof(TalentId)]!;
    private set => Data[nameof(TalentId)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error => new PropertyError(this.GetErrorCode(), ErrorMessage, TalentId, PropertyName);

  public TalentCannotBePurchasedMultipleTimesException(Talent talent, string propertyName)
    : base(BuildMessage(talent, propertyName))
  {
    WorldId = talent.WorldId.ToGuid();
    TalentId = talent.EntityId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Talent talent, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), talent.WorldId.ToGuid())
    .AddData(nameof(TalentId), talent.EntityId)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
