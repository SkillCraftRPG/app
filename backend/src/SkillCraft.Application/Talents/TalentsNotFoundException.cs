using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;

namespace SkillCraft.Application.Talents;

internal class TalentsNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified talents could not be found.";

  public IEnumerable<Guid> Ids
  {
    get => (IEnumerable<Guid>)Data[nameof(Ids)]!;
    private set => Data[nameof(Ids)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error => new PropertyError(this.GetErrorCode(), ErrorMessage, Ids, PropertyName);

  public TalentsNotFoundException(IEnumerable<Guid> ids, string? propertyName = null)
    : base(BuildMessage(ids, propertyName))
  {
    Ids = ids;
    PropertyName = propertyName;
  }

  private static string BuildMessage(IEnumerable<Guid> ids, string? propertyName)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append(nameof(PropertyName)).Append(": ").AppendLine(propertyName ?? "<null>");
    message.Append(nameof(Ids)).AppendLine(":");
    foreach (Guid id in ids)
    {
      message.Append(" - ").Append(id).AppendLine();
    }

    return message.ToString();
  }
}
