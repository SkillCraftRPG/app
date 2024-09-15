using FluentValidation;
using FluentValidation.Results;
using SkillCraft.Contracts.Aspects;

namespace SkillCraft.Domain.Aspects.Validators;

public class SkillsValidator : AbstractValidator<ISkills>
{
  public SkillsValidator()
  {
    When(x => x.Discounted1.HasValue, () => RuleFor(x => x.Discounted1!.Value).IsInEnum());
    When(x => x.Discounted2.HasValue, () => RuleFor(x => x.Discounted2!.Value).IsInEnum());
  }

  public override ValidationResult Validate(ValidationContext<ISkills> context)
  {
    const string errorMessage = "Each property must specify a different skill. A skill can only be specified by one property.";

    ValidationResult result = base.Validate(context);

    ISkills skills = context.InstanceToValidate;
    if (skills.Discounted1.HasValue && skills.Discounted1.Value == skills.Discounted2)
    {
      result.Errors.Add(new ValidationFailure(nameof(skills.Discounted1), errorMessage, skills.Discounted1.Value)
      {
        ErrorCode = "SkillsValidator"
      });
      result.Errors.Add(new ValidationFailure(nameof(skills.Discounted2), errorMessage, skills.Discounted2.Value)
      {
        ErrorCode = "SkillsValidator"
      });
    }

    return result;
  }
}
