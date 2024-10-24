using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;
using SkillCraft.Domain.Worlds;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Application.Characters;

internal class InvalidAspectAttributeSelectionException : BadRequestException
{
  private const string ErrorMessage = "The specified attribute was not in the aspects attribute selection.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
  public IEnumerable<Guid> AspectIds
  {
    get => (IEnumerable<Guid>)Data[nameof(AspectIds)]!;
    private set => Data[nameof(AspectIds)] = value;
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

  public InvalidAspectAttributeSelectionException(WorldId worldId, IEnumerable<Guid> aspectIds, Attribute attribute, string propertyName)
    : base(BuildMessage(worldId, aspectIds, attribute, propertyName))
  {
    WorldId = worldId.ToGuid();
    AspectIds = aspectIds;
    Attribute = attribute;
    PropertyName = propertyName;
  }

  private static string BuildMessage(WorldId worldId, IEnumerable<Guid> aspectIds, Attribute attribute, string propertyName)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append(nameof(WorldId)).Append(": ").Append(worldId.ToGuid()).AppendLine();
    message.Append(nameof(Attribute)).Append(": ").Append(attribute).AppendLine();
    message.Append(nameof(PropertyName)).Append(": ").Append(propertyName).AppendLine();
    message.Append(nameof(AspectIds)).Append(':').AppendLine();
    foreach (Guid aspectId in aspectIds)
    {
      message.Append(" - ").Append(aspectId).AppendLine();
    }

    return message.ToString();
  }
}
