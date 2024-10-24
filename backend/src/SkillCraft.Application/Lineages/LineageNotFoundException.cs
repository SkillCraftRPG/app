using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;
using SkillCraft.Domain.Lineages;

namespace SkillCraft.Application.Lineages;

internal class LineageNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified lineage could not be found.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
  public Guid LineageId
  {
    get => (Guid)Data[nameof(LineageId)]!;
    private set => Data[nameof(LineageId)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error => new PropertyError(this.GetErrorCode(), ErrorMessage, LineageId, PropertyName);

  public LineageNotFoundException(LineageId lineageId, string propertyName)
    : base(BuildMessage(lineageId, propertyName))
  {
    WorldId = lineageId.WorldId.ToGuid();
    LineageId = lineageId.EntityId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(LineageId lineageId, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), lineageId.WorldId.ToGuid())
    .AddData(nameof(LineageId), lineageId.EntityId)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
