using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Errors;
using SkillCraft.Domain.Talents;

namespace SkillCraft.Application.Talents;

internal class TalentSkillAlreadyExistingException : ConflictException
{
  private const string ErrorMessage = "A talent associated to the specified skill already exists.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
  public Skill Skill
  {
    get => (Skill)Data[nameof(Skill)]!;
    private set => Data[nameof(Skill)] = value;
  }
  public Guid TalentId
  {
    get => (Guid)Data[nameof(TalentId)]!;
    private set => Data[nameof(TalentId)] = value;
  }
  public Guid ConflictId
  {
    get => (Guid)Data[nameof(ConflictId)]!;
    private set => Data[nameof(ConflictId)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error => new PropertyError(this.GetErrorCode(), ErrorMessage, attemptedValue: null, PropertyName);
  // TODO(fpion): WorldId, Skill, TalentId

  public TalentSkillAlreadyExistingException(Talent talent, Talent conflict, string? propertyName = null)
  {
    if (talent.Skill == null)
    {
      throw new ArgumentException($"The {nameof(talent.Skill)} is required.", nameof(talent));
    }

    WorldId = talent.WorldId.ToGuid();
    Skill = talent.Skill.Value;
    TalentId = talent.Id.ToGuid();
    ConflictId = conflict.Id.ToGuid();
    PropertyName = propertyName;
  }
}
