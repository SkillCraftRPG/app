using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;
using SkillCraft.Domain.Customizations;

namespace SkillCraft.Application.Personalities;

internal class CustomizationIsNotGiftException : BadRequestException
{
  private const string ErrorMessage = "The specified customization is not a gift.";

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

  public CustomizationIsNotGiftException(Customization customization, string? propertyName = null)
    : base(BuildMessage(customization, propertyName))
  {
    CustomizationId = customization.EntityId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Customization customization, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(CustomizationId), customization.EntityId)
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
