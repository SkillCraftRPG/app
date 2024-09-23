using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Errors;
using SkillCraft.Domain.Talents;

namespace SkillCraft.Application.Talents;

internal class TalentSkillAlreadyExistingException : ConflictException
{
  private const string ErrorMessage = "The specified skill is already associated to another talent.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
  public IEnumerable<Guid> Ids
  {
    get => (IEnumerable<Guid>)Data[nameof(Ids)]!;
    private set => Data[nameof(Ids)] = value;
  }
  public Skill Skill
  {
    get => (Skill)Data[nameof(Skill)]!;
    private set => Data[nameof(Skill)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error => new PropertyError(this.GetErrorCode(), ErrorMessage, Skill, PropertyName);

  public TalentSkillAlreadyExistingException(Talent talent, TalentId conflictId, string? propertyName = null)
    : base(BuildMessage(talent, conflictId, propertyName))
  {
    if (talent.Skill == null)
    {
      throw new ArgumentException($"The {nameof(talent.Skill)} is required.", nameof(talent));
    }

    WorldId = talent.WorldId.ToGuid();
    Ids = [talent.Id.ToGuid(), conflictId.ToGuid()];
    Skill = talent.Skill.Value;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Talent talent, TalentId conflictId, string? propertyName)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append(nameof(WorldId)).Append(": ").Append(talent.WorldId.ToGuid()).AppendLine();
    message.Append(nameof(Skill)).Append(": ").Append(talent.Skill).AppendLine();
    message.Append(nameof(Ids)).Append(':').AppendLine();
    message.Append(" - ").Append(talent.Id.ToGuid()).AppendLine();
    message.Append(" - ").Append(conflictId.ToGuid()).AppendLine();

    return message.ToString();
  }
}
