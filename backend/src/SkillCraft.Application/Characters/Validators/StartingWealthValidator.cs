using FluentValidation;
using SkillCraft.Contracts.Characters;

namespace SkillCraft.Application.Characters.Validators;

internal class StartingWealthValidator : AbstractValidator<StartingWealthPayload>
{
  public StartingWealthValidator()
  {
    RuleFor(x => x.ItemId).NotEmpty();
    RuleFor(x => x.Quantity).GreaterThan(0);
  }
}
