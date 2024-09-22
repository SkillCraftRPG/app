using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;

namespace SkillCraft.Application.Characters;

internal class LanguagesCannotIncludeLineageLanguageException : BadRequestException
{
  private const string ErrorMessage = "The lineage languages should not be included in character languages.";

  public IEnumerable<Guid> LanguageIds
  {
    get => (IEnumerable<Guid>)Data[nameof(LanguageIds)]!;
    private set => Data[nameof(LanguageIds)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error => new PropertyError(this.GetErrorCode(), ErrorMessage, LanguageIds, PropertyName);

  public LanguagesCannotIncludeLineageLanguageException(IEnumerable<Guid> languageIds, string? propertyName = null)
    : base(BuildMessage(languageIds, propertyName))
  {
    LanguageIds = languageIds;
    PropertyName = propertyName;
  }

  private static string BuildMessage(IEnumerable<Guid> languageIds, string? propertyName)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append(nameof(PropertyName)).Append(": ").AppendLine(propertyName ?? "<null>");
    message.Append(nameof(LanguageIds)).Append(':').AppendLine();
    foreach (Guid languageId in languageIds)
    {
      message.Append(" - ").Append(languageId).AppendLine();
    }

    return message.ToString();
  }
}
