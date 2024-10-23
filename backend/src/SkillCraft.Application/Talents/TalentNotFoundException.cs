using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;
using SkillCraft.Domain.Talents;

namespace SkillCraft.Application.Talents;

internal class TalentNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified talent could not be found.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
  public Guid Id
  {
    get => (Guid)Data[nameof(Id)]!;
    private set => Data[nameof(Id)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error => new PropertyError(this.GetErrorCode(), ErrorMessage, Id, PropertyName);

  public TalentNotFoundException(TalentId talentId, string propertyName)
    : base(BuildMessage(talentId, propertyName))
  {
    WorldId = talentId.WorldId.ToGuid();
    Id = talentId.EntityId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(TalentId talentId, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), talentId.WorldId.ToGuid())
    .AddData(nameof(Id), talentId.EntityId)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
