using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;

namespace SkillCraft.Application.Characters;

internal class InvalidExtraLanguagesException : BadRequestException
{
  private const string ErrorMessage = "The specified extra languages did not match the lineages expected extra language count.";

  public IEnumerable<Guid> Ids
  {
    get => (IEnumerable<Guid>)Data[nameof(Ids)]!;
    private set => Data[nameof(Ids)] = value;
  }
  public int ExpectedCount
  {
    get => (int)Data[nameof(ExpectedCount)]!;
    private set => Data[nameof(ExpectedCount)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error
  {
    get
    {
      PropertyError error = new(this.GetErrorCode(), ErrorMessage, Ids, PropertyName);
      error.AddData(nameof(ExpectedCount), ExpectedCount.ToString());
      return error;
    }
  }

  public InvalidExtraLanguagesException(IEnumerable<Guid> ids, int expectedCount, string? propertyName = null)
    : base(BuildMessage(ids, expectedCount, propertyName))
  {
    Ids = ids;
    ExpectedCount = expectedCount;
    PropertyName = propertyName;
  }

  private static string BuildMessage(IEnumerable<Guid> ids, int expectedCount, string? propertyName)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append(nameof(ExpectedCount)).Append(": ").Append(expectedCount).AppendLine();
    message.Append(nameof(PropertyName)).Append(": ").AppendLine(propertyName ?? "<null>");
    message.Append(nameof(Ids)).Append(':').AppendLine();
    foreach (Guid id in ids)
    {
      message.Append(" - ").Append(id).AppendLine();
    }

    return message.ToString();
  }
}
