using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;
using SkillCraft.Domain.Customizations;

namespace SkillCraft.Application.Characters;

internal class CustomizationsCannotIncludePersonalityGiftException : BadRequestException
{
  private const string ErrorMessage = "The personality's gift should not be included in character customizations.";

  public Guid CustomizationId
  {
    get => (Guid)Data[nameof(CustomizationId)]!;
    private set => Data[nameof(CustomizationId)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error => new PropertyError(this.GetErrorCode(), ErrorMessage, CustomizationId, PropertyName);

  public CustomizationsCannotIncludePersonalityGiftException(Customization customization, string? propertyName = null)
    : base(BuildMessage(customization, propertyName))
  {
    CustomizationId = customization.Id.ToGuid();
    PropertyName = propertyName;
  }

  private static string BuildMessage(Customization customization, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(CustomizationId), customization.Id.ToGuid())
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
