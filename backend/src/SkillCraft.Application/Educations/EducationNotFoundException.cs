using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;
using SkillCraft.Domain.Educations;

namespace SkillCraft.Application.Educations;

internal class EducationNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified education could not be found.";

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
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error => new PropertyError(this.GetErrorCode(), ErrorMessage, EducationId, PropertyName);

  public EducationNotFoundException(EducationId educationId, string propertyName)
    : base(BuildMessage(educationId, propertyName))
  {
    WorldId = educationId.WorldId.ToGuid();
    EducationId = educationId.EntityId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(EducationId educationId, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), educationId.WorldId.ToGuid())
    .AddData(nameof(EducationId), educationId.EntityId)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
