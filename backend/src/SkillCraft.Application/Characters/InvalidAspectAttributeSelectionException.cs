using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Application.Characters;

internal class InvalidAspectAttributeSelectionException : BadRequestException
{
  private const string ErrorMessage = "The specified attribute was not in the aspects attribute selection.";

  public Attribute Attribute
  {
    get => (Attribute)Data[nameof(Attribute)]!;
    private set => Data[nameof(Attribute)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error => new PropertyError(this.GetErrorCode(), ErrorMessage, Attribute, PropertyName);

  public InvalidAspectAttributeSelectionException(Attribute attribute, string? propertyName = null)
    : base(BuildMessage(attribute, propertyName))
  {
    Attribute = attribute;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Attribute attribute, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(Attribute), attribute)
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
