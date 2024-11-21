using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;

namespace SkillCraft.Domain.Talents;

internal class TalentMaximumCostExceededException : DomainException
{
  private const string ErrorMessage = "The talent maximum cost was exceeded.";

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
  public int Tier
  {
    get => (int)Data[nameof(Tier)]!;
    private set => Data[nameof(Tier)] = value;
  }
  public int MaximumCost
  {
    get => (int)Data[nameof(MaximumCost)]!;
    private set => Data[nameof(MaximumCost)] = value;
  }
  public int AttemptedCost
  {
    get => (int)Data[nameof(AttemptedCost)]!;
    private set => Data[nameof(AttemptedCost)] = value;
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
      PropertyError error = new(this.GetErrorCode(), ErrorMessage, AttemptedCost, PropertyName);
      error.AddData(nameof(Tier), Tier.ToString());
      error.AddData(nameof(AttemptedCost), AttemptedCost.ToString());
      return error;
    }
  }

  public TalentMaximumCostExceededException(Talent talent, int cost, string propertyName)
    : base(BuildMessage(talent, cost, propertyName))
  {
    WorldId = talent.WorldId.ToGuid();
    TalentId = talent.EntityId;
    Tier = talent.Tier;
    MaximumCost = talent.Tier + 2;
    AttemptedCost = cost;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Talent talent, int cost, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), talent.WorldId.ToGuid())
    .AddData(nameof(TalentId), talent.EntityId)
    .AddData(nameof(Tier), talent.Tier)
    .AddData(nameof(MaximumCost), talent.Tier + 2)
    .AddData(nameof(AttemptedCost), cost)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
