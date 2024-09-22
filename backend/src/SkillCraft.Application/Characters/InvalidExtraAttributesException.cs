using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Application.Characters;

internal class InvalidExtraAttributesException : BadRequestException
{
  private const string ErrorMessage = "The specified extra attributes did not match the lineages expected extra attribute count.";

  public IEnumerable<Attribute> ExtraAttributes
  {
    get => (IEnumerable<Attribute>)Data[nameof(ExtraAttributes)]!;
    private set => Data[nameof(ExtraAttributes)] = value;
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
      PropertyError error = new(this.GetErrorCode(), ErrorMessage, ExtraAttributes, PropertyName);
      error.AddData(nameof(ExpectedCount), ExpectedCount.ToString());
      return error;
    }
  }

  public InvalidExtraAttributesException(IEnumerable<Attribute> extraAttributes, int expectedCount, string? propertyName = null)
    : base(BuildMessage(extraAttributes, expectedCount, propertyName))
  {
    ExtraAttributes = extraAttributes;
    ExpectedCount = expectedCount;
    PropertyName = propertyName;
  }

  private static string BuildMessage(IEnumerable<Attribute> extraAttributes, int expectedCount, string? propertyName)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append(nameof(ExpectedCount)).Append(": ").Append(expectedCount).AppendLine();
    message.Append(nameof(PropertyName)).Append(": ").AppendLine(propertyName ?? "<null>");
    message.Append(nameof(ExtraAttributes)).Append(':').AppendLine();
    foreach (Attribute attribute in extraAttributes)
    {
      message.Append(" - ").Append(attribute).AppendLine();
    }

    return message.ToString();
  }
}
