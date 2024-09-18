using FluentValidation;
using SkillCraft.Contracts.Lineages;

namespace SkillCraft.Application.Lineages.Validators;

internal class UpdateAttributesValidator : AbstractValidator<UpdateAttributesPayload>
{
  public UpdateAttributesValidator()
  {
    When(x => x.Agility.HasValue, () => RuleFor(x => x.Agility!.Value).InclusiveBetween(0, 2));
    When(x => x.Coordination.HasValue, () => RuleFor(x => x.Coordination!.Value).InclusiveBetween(0, 2));
    When(x => x.Intellect.HasValue, () => RuleFor(x => x.Intellect!.Value).InclusiveBetween(0, 2));
    When(x => x.Presence.HasValue, () => RuleFor(x => x.Presence!.Value).InclusiveBetween(0, 2));
    When(x => x.Sensitivity.HasValue, () => RuleFor(x => x.Sensitivity!.Value).InclusiveBetween(0, 2));
    When(x => x.Spirit.HasValue, () => RuleFor(x => x.Spirit!.Value).InclusiveBetween(0, 2));
    When(x => x.Vigor.HasValue, () => RuleFor(x => x.Vigor!.Value).InclusiveBetween(0, 2));

    When(x => x.Extra.HasValue, () => RuleFor(x => x.Extra!.Value).GreaterThanOrEqualTo(0).LessThanOrEqualTo(attributes => GetExtraMaximumValue(attributes)));
  }

  private static int GetExtraMaximumValue(UpdateAttributesPayload payload)
  {
    int maximum = 7;
    if (payload.Agility > 0)
    {
      maximum--;
    }
    if (payload.Coordination > 0)
    {
      maximum--;
    }
    if (payload.Intellect > 0)
    {
      maximum--;
    }
    if (payload.Presence > 0)
    {
      maximum--;
    }
    if (payload.Sensitivity > 0)
    {
      maximum--;
    }
    if (payload.Spirit > 0)
    {
      maximum--;
    }
    if (payload.Vigor > 0)
    {
      maximum--;
    }
    return maximum;
  }
}
