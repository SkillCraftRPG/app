using Logitar;
using Logitar.Portal.Contracts.Errors;

namespace SkillCraft.Domain.Characters;

public class NotEnoughRemainingSkillPointsException : DomainException
{
  private const string ErrorMessage = "The character has no remaining skill point.";

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

  public override Error Error => new(this.GetErrorCode(), ErrorMessage);

  public NotEnoughRemainingSkillPointsException(Character character)
    : base(BuildMessage(character))
  {
    WorldId = character.WorldId.ToGuid();
    CharacterId = character.EntityId;
  }

  private static string BuildMessage(Character character) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), character.WorldId.ToGuid())
    .AddData(nameof(CharacterId), character.EntityId)
    .Build();
}
