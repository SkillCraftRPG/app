using FluentValidation;
using FluentValidation.Results;
using SkillCraft.Contracts.Aspects;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Domain.Aspects.Validators;

public class AttributeSelectionValidator : AbstractValidator<IAttributeSelection>
{
  public AttributeSelectionValidator()
  {
    When(x => x.Mandatory1.HasValue, () => RuleFor(x => x.Mandatory1!.Value).IsInEnum());
    When(x => x.Mandatory2.HasValue, () => RuleFor(x => x.Mandatory2!.Value).IsInEnum());
    When(x => x.Optional1.HasValue, () => RuleFor(x => x.Optional1!.Value).IsInEnum());
    When(x => x.Optional2.HasValue, () => RuleFor(x => x.Optional2!.Value).IsInEnum());
  }

  public override ValidationResult Validate(ValidationContext<IAttributeSelection> context)
  {
    const string errorMessage = "Each property must specify a different attribute. An attribute can only be specified by one property.";

    ValidationResult result = base.Validate(context);

    Dictionary<Attribute, List<string>> properties = new(capacity: 4);
    IAttributeSelection attributes = context.InstanceToValidate;
    Fill(attributes.Mandatory1, nameof(attributes.Mandatory1), properties);
    Fill(attributes.Mandatory2, nameof(attributes.Mandatory2), properties);
    Fill(attributes.Optional1, nameof(attributes.Optional1), properties);
    Fill(attributes.Optional2, nameof(attributes.Optional2), properties);
    foreach (KeyValuePair<Attribute, List<string>> property in properties)
    {
      if (property.Value.Count > 1)
      {
        foreach (string propertyName in property.Value)
        {
          result.Errors.Add(new ValidationFailure(propertyName, errorMessage, property.Key)
          {
            ErrorCode = "AttributesValidator"
          });
        }
      }
    }

    return result;
  }
  private static void Fill(Attribute? attribute, string propertyName, Dictionary<Attribute, List<string>> properties)
  {
    if (attribute.HasValue)
    {
      if (!properties.TryGetValue(attribute.Value, out List<string>? propertyNames))
      {
        propertyNames = new(capacity: 4);
        properties[attribute.Value] = propertyNames;
      }

      propertyNames.Add(propertyName);
    }
  }
}
