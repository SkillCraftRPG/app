using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;
using SkillCraft.Domain.Lineages;

namespace SkillCraft.Application.Characters;

internal class LanguagesCannotIncludeLineageLanguageException : BadRequestException
{
  private const string ErrorMessage = "The lineage languages should not be included in character languages.";

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
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error => new PropertyError(this.GetErrorCode(), ErrorMessage, LanguageIds, PropertyName);

  public LanguagesCannotIncludeLineageLanguageException(Lineage lineage, IEnumerable<Guid> languageIds, string propertyName)
    : base(BuildMessage(lineage, languageIds, propertyName))
  {
    WorldId = lineage.WorldId.ToGuid();
    LineageId = lineage.EntityId;
    LanguageIds = languageIds;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Lineage lineage, IEnumerable<Guid> languageIds, string propertyName)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append(nameof(WorldId)).Append(": ").Append(lineage.WorldId.ToGuid()).AppendLine();
    message.Append(nameof(LineageId)).Append(": ").Append(lineage.EntityId).AppendLine();
    message.Append(nameof(PropertyName)).Append(": ").AppendLine(propertyName);
    message.Append(nameof(PropertyName)).Append(": ").AppendLine(propertyName);
    message.Append(nameof(LanguageIds)).Append(':').AppendLine();
    foreach (Guid languageId in languageIds)
    {
      message.Append(" - ").Append(languageId).AppendLine();
    }

    return message.ToString();
  }
}
