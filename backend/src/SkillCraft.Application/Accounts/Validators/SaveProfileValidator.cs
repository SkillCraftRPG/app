using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users.Validators;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Application.Accounts.Validators;

internal abstract class SaveProfileValidator<T> : AbstractValidator<T> where T : SaveProfilePayload
{
  public SaveProfileValidator()
  {
    RuleFor(x => x.FirstName).SetValidator(new PersonNameValidator());
    RuleFor(x => x.LastName).SetValidator(new PersonNameValidator());

    RuleFor(x => x.MultiFactorAuthenticationMode).IsInEnum();
    When(x => x.MultiFactorAuthenticationMode == MultiFactorAuthenticationMode.Phone, () => RuleFor(x => x.Phone).NotNull().WithErrorCode(nameof(SaveProfileValidator)));
    When(x => x.Phone != null, () => RuleFor(x => x.Phone!).SetValidator(new PhoneValidator()));

    When(x => x.Birthdate.HasValue, () => RuleFor(x => x.Birthdate!.Value).SetValidator(new BirthdateValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Gender), () => RuleFor(x => x.Gender!).SetValidator(new GenderValidator()));
    RuleFor(x => x.Locale).SetValidator(new LocaleValidator());
    RuleFor(x => x.TimeZone).SetValidator(new TimeZoneValidator());
  }
}

internal class SaveProfileValidator : SaveProfileValidator<SaveProfilePayload>
{
  public SaveProfileValidator() : base()
  {
  }
}
