using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Errors;
using SkillCraft.Domain.Castes;

namespace SkillCraft.Application.Castes;

internal class CasteHasNoSkillTalentException : NotFoundException
{
  private const string ErrorMessage = "The specified caste does not have an associated skill talent.";

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
  public Skill? Skill
  {
    get => (Skill)Data[nameof(Skill)]!;
    private set => Data[nameof(Skill)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error => new PropertyError(this.GetErrorCode(), ErrorMessage, CasteId, PropertyName);

  public CasteHasNoSkillTalentException(Caste caste, string propertyName)
    : base(BuildMessage(caste, propertyName))
  {
    WorldId = caste.WorldId.ToGuid();
    CasteId = caste.EntityId;
    Skill = caste.Skill;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Caste caste, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), caste.WorldId.ToGuid())
    .AddData(nameof(CasteId), caste.EntityId)
    .AddData(nameof(Skill), caste.Skill, "<null>")
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
