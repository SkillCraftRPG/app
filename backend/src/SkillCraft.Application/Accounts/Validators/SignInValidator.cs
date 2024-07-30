using FluentValidation;
using SkillCraft.Contracts.Accounts;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Accounts.Validators;

internal class SignInValidator : AbstractValidator<SignInPayload>
{
  public SignInValidator()
  {
    RuleFor(x => x.Locale).Locale();

    When(x => x.Credentials != null, () => RuleFor(x => x.Credentials!).SetValidator(new CredentialsValidator()));
    When(x => x.OneTimePassword != null, () => RuleFor(x => x.OneTimePassword!).SetValidator(new OneTimePasswordValidator()));
    //When(x => x.Profile != null, () => RuleFor(x => x.Profile!).SetValidator(new CompleteProfileValidator(passwordSettings))); // ISSUE: https://github.com/SkillCraftRPG/app/issues/5

    RuleFor(x => x).Must(BeAValidPayload).WithErrorCode(nameof(SignInValidator))
      .WithMessage(x => $"Exactly one of the following must be specified: {nameof(x.Credentials)}, {nameof(x.AuthenticationToken)}, {nameof(x.OneTimePassword)}."); // ISSUE: https://github.com/SkillCraftRPG/app/issues/5
  }

  private static bool BeAValidPayload(SignInPayload payload)
  {
    int count = 0;
    if (payload.Credentials != null)
    {
      count++;
    }
    if (!string.IsNullOrWhiteSpace(payload.AuthenticationToken))
    {
      count++;
    }
    if (payload.OneTimePassword != null)
    {
      count++;
    }
    //if (payload.Profile != null)
    //{
    //  count++;
    //} // ISSUE: https://github.com/SkillCraftRPG/app/issues/5
    return count == 1;
  }
}
