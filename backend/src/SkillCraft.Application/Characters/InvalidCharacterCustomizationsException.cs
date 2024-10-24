using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Characters;

internal class InvalidCharacterCustomizationsException : BadRequestException
{
  private const string ErrorMessage = "The specified character customizations were not an equal number of gifts and disabilities.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
  public IEnumerable<Guid> CustomizationIds
  {
    get => (IEnumerable<Guid>)Data[nameof(CustomizationIds)]!;
    private set => Data[nameof(CustomizationIds)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error => new PropertyError(this.GetErrorCode(), ErrorMessage, CustomizationIds, PropertyName);

  public InvalidCharacterCustomizationsException(WorldId worldId, IEnumerable<Guid> customizationIds, string propertyName)
    : base(BuildMessage(worldId, customizationIds, propertyName))
  {
    WorldId = worldId.ToGuid();
    CustomizationIds = customizationIds;
    PropertyName = propertyName;
  }

  private static string BuildMessage(WorldId worldId, IEnumerable<Guid> customizationIds, string propertyName)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append(nameof(WorldId)).Append(": ").Append(worldId.ToGuid()).AppendLine();
    message.Append(nameof(PropertyName)).Append(": ").AppendLine(propertyName);
    message.Append(nameof(CustomizationIds)).AppendLine(":");
    foreach (Guid customizationId in customizationIds)
    {
      message.Append(" - ").Append(customizationId).AppendLine();
    }

    return message.ToString();
  }
}
