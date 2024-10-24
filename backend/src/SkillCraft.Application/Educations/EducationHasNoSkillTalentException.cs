using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Errors;
using SkillCraft.Domain.Educations;

namespace SkillCraft.Application.Educations;

internal class EducationHasNoSkillTalentException : NotFoundException
{
  private const string ErrorMessage = "The specified education does not have an associated skill talent.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
  public Guid EducationId
  {
    get => (Guid)Data[nameof(EducationId)]!;
    private set => Data[nameof(EducationId)] = value;
  }
  public Skill? Skill
  {
    get => (Skill?)Data[nameof(Skill)];
    private set => Data[nameof(Skill)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error => new PropertyError(this.GetErrorCode(), ErrorMessage, EducationId, PropertyName);

  public EducationHasNoSkillTalentException(Education education, string propertyName)
    : base(BuildMessage(education, propertyName))
  {
    WorldId = education.WorldId.ToGuid();
    EducationId = education.EntityId;
    Skill = education.Skill;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Education education, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), education.WorldId.ToGuid())
    .AddData(nameof(EducationId), education.EntityId)
    .AddData(nameof(Skill), education.Skill, "<null>")
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
