using FluentValidation;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Passwords.Validators;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users.Validators;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Application.Accounts.Validators;

internal class ProfileValidator : AbstractValidator<ProfilePayload>
{
  public ProfileValidator(IPasswordSettings passwordSettings)
  {
    RuleFor(x => x.Token).NotEmpty();

    RuleFor(x => x.FirstName).SetValidator(new PersonNameValidator());
    RuleFor(x => x.LastName).SetValidator(new PersonNameValidator());

    When(x => x.Password != null, () => RuleFor(x => x.Password!).SetValidator(new PasswordValidator(passwordSettings)));
    RuleFor(x => x.MultiFactorAuthenticationMode).IsInEnum();
    When(x => x.MultiFactorAuthenticationMode == MultiFactorAuthenticationMode.Phone, () => RuleFor(x => x.Phone).NotNull());
    When(x => x.Phone != null, () => RuleFor(x => x.Phone!).SetValidator(new PhoneValidator()));

    When(x => x.Birthdate.HasValue, () => RuleFor(x => x.Birthdate!.Value).SetValidator(new BirthdateValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Gender), () => RuleFor(x => x.Gender!).SetValidator(new GenderValidator()));
    RuleFor(x => x.Locale).SetValidator(new LocaleValidator());
    RuleFor(x => x.TimeZone).SetValidator(new TimeZoneValidator());
  }
}
