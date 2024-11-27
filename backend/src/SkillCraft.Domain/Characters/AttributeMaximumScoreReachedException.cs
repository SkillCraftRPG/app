using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Domain.Characters;

internal class AttributeMaximumScoreReachedException : DomainException
{
  private const string ErrorMessage = "The specified character attribute has reached its maximum score.";

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
  public Attribute Attribute
  {
    get => (Attribute)Data[nameof(Attribute)]!;
    private set => Data[nameof(Attribute)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error => new PropertyError(this.GetErrorCode(), ErrorMessage, Attribute, PropertyName);

  public AttributeMaximumScoreReachedException(Character character, Attribute attribute, string propertyName)
    : base(BuildMessage(character, attribute, propertyName))
  {
    WorldId = character.WorldId.ToGuid();
    CharacterId = character.EntityId;
    Attribute = attribute;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Character character, Attribute attribute, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), character.WorldId.ToGuid())
    .AddData(nameof(CharacterId), character.EntityId)
    .AddData(nameof(Attribute), attribute)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
