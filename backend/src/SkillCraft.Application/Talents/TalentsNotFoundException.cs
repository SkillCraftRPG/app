using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Talents;

internal class TalentsNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified talents could not be found.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
  public IEnumerable<Guid> TalentIds
  {
    get => (IEnumerable<Guid>)Data[nameof(TalentIds)]!;
    private set => Data[nameof(TalentIds)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error => new PropertyError(this.GetErrorCode(), ErrorMessage, TalentIds, PropertyName);

  public TalentsNotFoundException(WorldId worldId, IEnumerable<Guid> talentIds, string propertyName)
    : base(BuildMessage(worldId, talentIds, propertyName))
  {
    WorldId = worldId.ToGuid();
    TalentIds = talentIds;
    PropertyName = propertyName;
  }

  private static string BuildMessage(WorldId worldId, IEnumerable<Guid> talentIds, string propertyName)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append(nameof(WorldId)).Append(": ").Append(worldId.ToGuid()).AppendLine();
    message.Append(nameof(PropertyName)).Append(": ").AppendLine(propertyName);
    message.Append(nameof(TalentIds)).AppendLine(":");
    foreach (Guid talentId in talentIds)
    {
      message.Append(" - ").Append(talentId).AppendLine();
    }

    return message.ToString();
  }
}
