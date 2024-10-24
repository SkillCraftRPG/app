using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;
using SkillCraft.Domain.Castes;

namespace SkillCraft.Application.Castes;

internal class CasteNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified caste could not be found.";

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
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error => new PropertyError(this.GetErrorCode(), ErrorMessage, CasteId, PropertyName);

  public CasteNotFoundException(CasteId casteId, string propertyName)
    : base(BuildMessage(casteId, propertyName))
  {
    WorldId = casteId.WorldId.ToGuid();
    CasteId = casteId.EntityId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(CasteId casteId, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), casteId.WorldId.ToGuid())
    .AddData(nameof(CasteId), casteId.EntityId)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
