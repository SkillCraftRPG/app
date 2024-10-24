using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;
using SkillCraft.Domain.Lineages;

namespace SkillCraft.Application.Characters;

internal class InvalidExtraLanguagesException : BadRequestException
{
  private const string ErrorMessage = "The specified extra languages did not match the lineages expected extra language count.";

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
  public IEnumerable<Guid> LanguageIds
  {
    get => (IEnumerable<Guid>)Data[nameof(LanguageIds)]!;
    private set => Data[nameof(LanguageIds)] = value;
  }
  public int ExpectedCount
  {
    get => (int)Data[nameof(ExpectedCount)]!;
    private set => Data[nameof(ExpectedCount)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error
  {
    get
    {
      PropertyError error = new(this.GetErrorCode(), ErrorMessage, LanguageIds, PropertyName);
      error.AddData(nameof(ExpectedCount), ExpectedCount.ToString());
      return error;
    }
  }

  public InvalidExtraLanguagesException(Lineage lineage, IEnumerable<Guid> languageIds, int expectedCount, string propertyName)
    : base(BuildMessage(lineage, languageIds, expectedCount, propertyName))
  {
    WorldId = lineage.WorldId.ToGuid();
    LineageId = lineage.EntityId;
    LanguageIds = languageIds;
    ExpectedCount = expectedCount;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Lineage lineage, IEnumerable<Guid> languageIds, int expectedCount, string propertyName)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append(nameof(WorldId)).Append(": ").Append(lineage.WorldId.ToGuid()).AppendLine();
    message.Append(nameof(LineageId)).Append(": ").Append(lineage.EntityId).AppendLine();
    message.Append(nameof(ExpectedCount)).Append(": ").Append(expectedCount).AppendLine();
    message.Append(nameof(PropertyName)).Append(": ").AppendLine(propertyName);
    message.Append(nameof(LanguageIds)).Append(':').AppendLine();
    foreach (Guid languageId in languageIds)
    {
      message.Append(" - ").Append(languageId).AppendLine();
    }

    return message.ToString();
  }
}
