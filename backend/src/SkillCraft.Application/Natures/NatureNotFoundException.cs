using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;
using SkillCraft.Domain.Natures;

namespace SkillCraft.Application.Natures;

internal class NatureNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified nature could not be found.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
  public Guid NatureId
  {
    get => (Guid)Data[nameof(NatureId)]!;
    private set => Data[nameof(NatureId)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error => new PropertyError(this.GetErrorCode(), ErrorMessage, NatureId, PropertyName);

  public NatureNotFoundException(NatureId natureId, string propertyName)
    : base(BuildMessage(natureId, propertyName))
  {
    WorldId = natureId.WorldId.ToGuid();
    NatureId = natureId.EntityId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(NatureId natureId, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), natureId.WorldId.ToGuid())
    .AddData(nameof(NatureId), natureId.EntityId)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
