using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Domain.Castes;
using SkillCraft.Domain.Educations;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Characters;

internal class InvalidCasteEducationSelectionException : BadRequestException
{
  private const string ErrorMessage = "The specified caste and education are associated to the same skill.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
  public Guid CasteId
  {
    get => (Guid)Data[nameof(CasteId)]!;
    private set => Data[nameof(CasteId)] = value;
  }
  public Guid EducationId
  {
    get => (Guid)Data[nameof(EducationId)]!;
    private set => Data[nameof(EducationId)] = value;
  }

  public override Error Error => new(this.GetErrorCode(), ErrorMessage);

  public InvalidCasteEducationSelectionException(Caste caste, Education education)
    : base(BuildMessage(caste, education))
  {
    WorldId worldId = GetWorldId(caste, education);

    WorldId = worldId.ToGuid();
    CasteId = caste.EntityId;
    EducationId = education.EntityId;
  }

  private static string BuildMessage(Caste caste, Education education)
  {
    WorldId worldId = GetWorldId(caste, education);
    return new ErrorMessageBuilder(ErrorMessage)
      .AddData(nameof(WorldId), worldId.ToGuid())
      .AddData(nameof(CasteId), caste.EntityId)
      .AddData(nameof(EducationId), education.EntityId)
      .Build();
  }

  private static WorldId GetWorldId(Caste caste, Education education) => new WorldId[] { caste.WorldId, education.WorldId }.Distinct().Single();
}
