using FluentValidation;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Application.Accounts.Validators;

internal class SignInValidator : AbstractValidator<SignInPayload>
{
  public SignInValidator(IPasswordSettings passwordSettings)
  {
    RuleFor(x => x.Locale).SetValidator(new LocaleValidator());

    When(x => x.Credentials != null, () => RuleFor(x => x.Credentials!).SetValidator(new CredentialsValidator()));
    When(x => x.OneTimePassword != null, () => RuleFor(x => x.OneTimePassword!).SetValidator(new OneTimePasswordValidator()));
    When(x => x.Profile != null, () => RuleFor(x => x.Profile!).SetValidator(new CompleteProfileValidator(passwordSettings)));

    RuleFor(x => x).Must(BeAValidPayload).WithErrorCode(nameof(SignInValidator))
      .WithMessage(x => $"Exactly one of the following properties must be provided: {nameof(x.Credentials)}, {x.Token}, {x.OneTimePassword}, {x.Profile}.");
  }

  private static bool BeAValidPayload(SignInPayload payload)
  {
    int notNull = 0;
    if (payload.Credentials != null)
    {
      notNull++;
    }
    if (!string.IsNullOrWhiteSpace(payload.Token))
    {
      notNull++;
    }
    if (payload.OneTimePassword != null)
    {
      notNull++;
    }
    if (payload.Profile != null)
    {
      notNull++;
    }
    return notNull == 1;
  }
}
