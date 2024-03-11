using FluentValidation;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Passwords.Validators;
using Logitar.Identity.Domain.Shared;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Application.Accounts.Validators;

internal class ResetPasswordValidator : AbstractValidator<ResetPasswordPayload>
{
  public ResetPasswordValidator(IPasswordSettings passwordSettings)
  {
    RuleFor(x => x.Locale).SetValidator(new LocaleValidator());

    When(x => string.IsNullOrWhiteSpace(x.EmailAddress), () =>
    {
      RuleFor(x => x.Token).NotEmpty();
      RuleFor(x => x.Password).NotEmpty();
      When(x => x.Password != null, () => RuleFor(x => x.Password!).SetValidator(new PasswordValidator(passwordSettings)));
    }).Otherwise(() =>
    {
      RuleFor(x => x.Token).Empty();
      RuleFor(x => x.Password).Empty();
    });
  }
}
