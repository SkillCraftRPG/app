using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;
using SkillCraft.Domain.Lineages;

namespace SkillCraft.Application.Lineages;

internal class InvalidParentLineageException : BadRequestException
{
  private const string ErrorMessage = "The specified parent lineage has a parent lineage.";

  public Guid ParentId
  {
    get => (Guid)Data[nameof(ParentId)]!;
    private set => Data[nameof(ParentId)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error => new PropertyError(this.GetErrorCode(), ErrorMessage, ParentId, PropertyName);

  public InvalidParentLineageException(Lineage parent, string? propertyName = null) : base(BuildMessage(parent, propertyName))
  {
    ParentId = parent.Id.ToGuid();
    PropertyName = propertyName;
  }

  private static string BuildMessage(Lineage parent, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(ParentId), parent.Id)
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
