using FluentValidation;

namespace SkillCraft.Domain.Characters.Validators;

internal class BaseAttributesValidator : AbstractValidator<BaseAttributes>
{
  private const int MinimumScore = BaseAttributes.MinimumScore;
  private const int MaximumScore = BaseAttributes.MaximumScore;
  private const int ScoreSum = BaseAttributes.ScoreSum;

  public BaseAttributesValidator()
  {
    RuleFor(x => x.Agility).InclusiveBetween(MinimumScore, MaximumScore);
    RuleFor(x => x.Coordination).InclusiveBetween(MinimumScore, MaximumScore);
    RuleFor(x => x.Intellect).InclusiveBetween(MinimumScore, MaximumScore);
    RuleFor(x => x.Presence).InclusiveBetween(MinimumScore, MaximumScore);
    RuleFor(x => x.Sensitivity).InclusiveBetween(MinimumScore, MaximumScore);
    RuleFor(x => x.Spirit).InclusiveBetween(MinimumScore, MaximumScore);
    RuleFor(x => x.Vigor).InclusiveBetween(MinimumScore, MaximumScore);
    RuleFor(x => x).Must(x => (x.Agility + x.Coordination + x.Intellect + x.Presence + x.Sensitivity + x.Spirit + x.Vigor) == ScoreSum)
      .WithErrorCode("BaseAttributeScoreSumValidator")
      .WithMessage($"The sum of the base attribute scores must equal {ScoreSum}.");

    RuleFor(x => x.Best).IsInEnum()
      .NotEqual(x => x.Worst);
    RuleFor(x => x.Worst).IsInEnum()
      .NotEqual(x => x.Best);
    RuleFor(x => x.Mandatory).Must(x => x.Count == 2)
      .WithErrorCode("MandatoryAttributesValidator")
      .WithMessage("'{PropertyName}' must provide exactly 2 attributes.");
    RuleForEach(x => x.Mandatory).IsInEnum();
    RuleFor(x => x.Optional).Must(x => x.Count == 2)
      .WithErrorCode("OptionalAttributesValidator")
      .WithMessage("'{PropertyName}' must provide exactly 2 attributes.");
    RuleForEach(x => x.Optional).IsInEnum();

    RuleForEach(x => x.Extra).IsInEnum();
  }
}
