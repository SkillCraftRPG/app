using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;
using SkillCraft.Domain.Castes;
using SkillCraft.Domain.Educations;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Characters;

internal class InvalidSkillTalentSelectionException : BadRequestException
{
  private const string ErrorMessage = "The specified talents are not associated to a skill, or are associated to the same skill as the caste or the education.";

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

  public InvalidSkillTalentSelectionException(Caste caste, Education education, IEnumerable<Guid> talentIds, string propertyName)
    : base(BuildMessage(caste, education, talentIds, propertyName))
  {
    WorldId worldId = GetWorldId(caste, education);

    WorldId = worldId.ToGuid();
    CasteId = caste.EntityId;
    EducationId = education.EntityId;
    TalentIds = talentIds;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Caste caste, Education education, IEnumerable<Guid> talentIds, string propertyName)
  {
    WorldId worldId = GetWorldId(caste, education);

    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append(nameof(WorldId)).Append(": ").Append(worldId.ToGuid()).AppendLine();
    message.Append(nameof(PropertyName)).Append(": ").AppendLine(propertyName);
    message.Append(nameof(TalentIds)).Append(':').AppendLine();
    foreach (Guid talentId in talentIds)
    {
      message.Append(" - ").Append(talentId).AppendLine();
    }

    return message.ToString();
  }

  private static WorldId GetWorldId(Caste caste, Education education) => new WorldId[] { caste.WorldId, education.WorldId }.Distinct().Single();
}
