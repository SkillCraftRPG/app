using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Aspects;

internal class AspectsNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified aspects could not be found.";

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
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error => new PropertyError(this.GetErrorCode(), ErrorMessage, AspectIds, PropertyName);

  public AspectsNotFoundException(WorldId worldId, IEnumerable<Guid> aspectIds, string propertyName)
    : base(BuildMessage(worldId, aspectIds, propertyName))
  {
    WorldId = worldId.ToGuid();
    AspectIds = aspectIds;
    PropertyName = propertyName;
  }

  private static string BuildMessage(WorldId worldId, IEnumerable<Guid> aspectIds, string propertyName)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append(nameof(WorldId)).Append(": ").Append(worldId.ToGuid()).AppendLine();
    message.Append(nameof(PropertyName)).Append(": ").AppendLine(propertyName);
    message.Append(nameof(AspectIds)).AppendLine(":");
    foreach (Guid aspectId in aspectIds)
    {
      message.Append(" - ").Append(aspectId).AppendLine();
    }

    return message.ToString();
  }
}
