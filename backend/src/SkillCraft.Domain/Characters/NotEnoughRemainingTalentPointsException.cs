using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;
using SkillCraft.Domain.Talents;

namespace SkillCraft.Domain.Characters;

public class NotEnoughRemainingTalentPointsException : DomainException
{
  private const string ErrorMessage = "The specified talent cannot be purchased since its cost is greater than the character remaining talent points.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
  public Guid CharacterId
  {
    get => (Guid)Data[nameof(CharacterId)]!;
    private set => Data[nameof(CharacterId)] = value;
  }
  public int RemainingTalentPoints
  {
    get => (int)Data[nameof(RemainingTalentPoints)]!;
    private set => Data[nameof(RemainingTalentPoints)] = value;
  }
  public Guid TalentId
  {
    get => (Guid)Data[nameof(TalentId)]!;
    private set => Data[nameof(TalentId)] = value;
  }
  public int Cost
  {
    get => (int)Data[nameof(Cost)]!;
    private set => Data[nameof(Cost)] = value;
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
      PropertyError error = new(this.GetErrorCode(), ErrorMessage, TalentId, PropertyName);
      error.AddData(nameof(RemainingTalentPoints), RemainingTalentPoints.ToString());
      error.AddData(nameof(Cost), Cost.ToString());
      return error;
    }
  }

  public NotEnoughRemainingTalentPointsException(Character character, Talent talent, int cost, string propertyName)
    : base(BuildMessage(character, talent, cost, propertyName))
  {
    WorldId = character.WorldId.ToGuid();
    CharacterId = character.EntityId;
    RemainingTalentPoints = character.RemainingTalentPoints;
    TalentId = talent.EntityId;
    Cost = cost;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Character character, Talent talent, int cost, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), character.WorldId.ToGuid())
    .AddData(nameof(CharacterId), character.EntityId)
    .AddData(nameof(RemainingTalentPoints), character.RemainingTalentPoints)
    .AddData(nameof(TalentId), talent.EntityId)
    .AddData(nameof(Cost), cost)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
