using FluentValidation;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Domain.Lineages.Validators;

internal class NamesValidator : AbstractValidator<Names>
{
  public NamesValidator()
  {
    When(x => x.Text != null, () => RuleFor(x => x.Text!).NamesText());
    RuleForEach(x => x.Family).NotEmpty();
    RuleForEach(x => x.Female).NotEmpty();
    RuleForEach(x => x.Male).NotEmpty();
    RuleForEach(x => x.Unisex).NotEmpty();
    RuleForEach(x => x.Custom.Keys).NotEmpty();
    RuleForEach(x => x.Custom.Values).NotEmpty();
  }
}
