using FluentValidation;
using Logitar.Identity.Domain.Shared;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Application.Accounts.Validators;

internal class VerifyPhoneValidator : AbstractValidator<VerifyPhonePayload>
{
  public VerifyPhoneValidator()
  {
    RuleFor(x => x.Locale).SetValidator(new LocaleValidator());
    RuleFor(x => x.ProfileCompletionToken).NotEmpty();

    When(x => x.Phone != null, () => RuleFor(x => x.Phone!).SetValidator(new AccountPhoneValidator()));
    When(x => x.OneTimePassword != null, () => RuleFor(x => x.OneTimePassword!).SetValidator(new OneTimePasswordValidator()));

    RuleFor(x => x).Must(BeAValidPayload).WithErrorCode(nameof(VerifyPhoneValidator))
      .WithMessage(x => $"Exactly one of the following must be specified: {nameof(x.Phone)}, {nameof(x.OneTimePassword)}.");
  }

  private static bool BeAValidPayload(VerifyPhonePayload payload)
  {
    int count = 0;
    if (payload.Phone != null)
    {
      count++;
    }
    if (payload.OneTimePassword != null)
    {
      count++;
    }
    return count == 1;
  }
}
