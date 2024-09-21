using FluentValidation;
using SkillCraft.Contracts.Characters;

namespace SkillCraft.Application.Characters.Validators;

internal class BaseAttributesValidator : AbstractValidator<BaseAttributesPayload>
{
  private const int MinimumScore = 6;
  private const int MaximumScore = 11;
  private const int Sum = 57; // 7 attributes × 8 points + 1 point = 57 points

  public BaseAttributesValidator()
  {
    RuleFor(x => x.Agility).InclusiveBetween(MinimumScore, MaximumScore);
    RuleFor(x => x.Coordination).InclusiveBetween(MinimumScore, MaximumScore);
    RuleFor(x => x.Intellect).InclusiveBetween(MinimumScore, MaximumScore);
    RuleFor(x => x.Presence).InclusiveBetween(MinimumScore, MaximumScore);
    RuleFor(x => x.Sensitivity).InclusiveBetween(MinimumScore, MaximumScore);
    RuleFor(x => x.Spirit).InclusiveBetween(MinimumScore, MaximumScore);
    RuleFor(x => x.Vigor).InclusiveBetween(MinimumScore, MaximumScore);
    RuleFor(x => x).Must(x => (x.Agility + x.Coordination + x.Intellect + x.Presence + x.Sensitivity + x.Spirit + x.Vigor) == Sum)
      .WithErrorCode("BaseAttributeScoresValidator")
      .WithMessage($"The sum of the base scores must equal {Sum}.");

    RuleFor(x => x.Best).IsInEnum().NotEqual(x => x.Worst);
    RuleFor(x => x.Worst).IsInEnum().NotEqual(x => x.Best);
    RuleFor(x => x.Optional).Must(x => x.Count == 2)
      .WithErrorCode("OptionalAttributesValidator")
      .WithMessage("'{PropertyName}' must contain exactly 2 attributes.");
    RuleForEach(x => x.Optional).IsInEnum();

    RuleForEach(x => x.Extra).IsInEnum();
  }
}
