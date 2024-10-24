using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;
using SkillCraft.Domain.Lineages;

namespace SkillCraft.Application.Characters;

internal class InvalidCharacterLineageException : BadRequestException
{
  private const string ErrorMessage = "The lineage should be a species without nation, or a nation.";

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

  public InvalidCharacterLineageException(Lineage lineage, string propertyName)
    : base(BuildMessage(lineage, propertyName))
  {
    WorldId = lineage.WorldId.ToGuid();
    LineageId = lineage.EntityId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Lineage lineage, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), lineage.WorldId.ToGuid())
    .AddData(nameof(LineageId), lineage.EntityId)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
