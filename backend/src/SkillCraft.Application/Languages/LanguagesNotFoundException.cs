using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Languages;

internal class LanguagesNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified languages could not be found.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
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

  public LanguagesNotFoundException(WorldId worldId, IEnumerable<Guid> languageIds, string propertyName)
    : base(BuildMessage(worldId, languageIds, propertyName))
  {
    WorldId = worldId.ToGuid();
    LanguageIds = languageIds;
    PropertyName = propertyName;
  }

  private static string BuildMessage(WorldId worldId, IEnumerable<Guid> languageIds, string propertyName)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append(nameof(WorldId)).Append(": ").Append(worldId.ToGuid()).AppendLine();
    message.Append(nameof(PropertyName)).Append(": ").AppendLine(propertyName);
    message.Append(nameof(LanguageIds)).AppendLine(":");
    foreach (Guid languageId in languageIds)
    {
      message.Append(" - ").Append(languageId).AppendLine();
    }

    return message.ToString();
  }
}
