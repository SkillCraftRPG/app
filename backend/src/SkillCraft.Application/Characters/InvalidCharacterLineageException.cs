using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;
using SkillCraft.Domain.Lineages;

namespace SkillCraft.Application.Characters;

internal class InvalidCharacterLineageException : BadRequestException
{
  private const string ErrorMessage = "The lineage should be a species without nation, or a nation.";

  public Guid Id
  {
    get => (Guid)Data[nameof(Id)]!;
    private set => Data[nameof(Id)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error => new PropertyError(this.GetErrorCode(), ErrorMessage, Id, PropertyName);

  public InvalidCharacterLineageException(Lineage lineage, string? propertyName = null)
    : base(BuildMessage(lineage, propertyName))
  {
    Id = lineage.Id.ToGuid();
    PropertyName = propertyName;
  }

  private static string BuildMessage(Lineage lineage, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(Id), lineage.Id.ToGuid())
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
