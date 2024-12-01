using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;

namespace SkillCraft.Domain.Lineages;

public class InvalidParentLineageException : DomainException
{
  private const string ErrorMessage = "The specified parent lineage has a parent lineage.";

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

  public InvalidParentLineageException(Lineage lineage, string propertyName) : base(BuildMessage(lineage, propertyName))
  {
    WorldId = lineage.WorldId.ToGuid();
    LineageId = lineage.EntityId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Lineage parent, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), parent.WorldId.ToGuid())
    .AddData(nameof(LineageId), parent.EntityId)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
