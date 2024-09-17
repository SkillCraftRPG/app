using FluentValidation;

namespace SkillCraft.Domain.Lineages.Validators;

internal class NamesValidator : AbstractValidator<Names>
{
  public NamesValidator()
  {
    When(x => x.Text != null, () => RuleFor(x => x.Text!).NotEmpty().MaximumLength(Names.MaximumLength));
    RuleForEach(x => x.Family).NotEmpty();
    RuleForEach(x => x.Female).NotEmpty();
    RuleForEach(x => x.Male).NotEmpty();
    RuleForEach(x => x.Unisex).NotEmpty();
    RuleForEach(x => x.Custom.Keys).NotEmpty();
    RuleForEach(x => x.Custom.Values).NotEmpty();
  }
}
