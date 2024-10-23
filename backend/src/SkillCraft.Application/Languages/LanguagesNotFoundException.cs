using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;

namespace SkillCraft.Application.Languages;

internal class LanguagesNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified languages could not be found.";

  public IEnumerable<Guid> LanguageIds
  {
    get => (IEnumerable<Guid>)Data[nameof(LanguageIds)]!;
    private set => Data[nameof(LanguageIds)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error => new PropertyError(this.GetErrorCode(), ErrorMessage, LanguageIds, PropertyName);

  public LanguagesNotFoundException(IEnumerable<Guid> ids, string propertyName)
    : base(BuildMessage(ids, propertyName))
  {
    LanguageIds = ids;
    PropertyName = propertyName;
  }

  private static string BuildMessage(IEnumerable<Guid> ids, string propertyName)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append(nameof(PropertyName)).Append(": ").AppendLine(propertyName);
    message.Append(nameof(LanguageIds)).AppendLine(":");
    foreach (Guid id in ids)
    {
      message.Append(" - ").Append(id).AppendLine();
    }

    return message.ToString();
  }
}
