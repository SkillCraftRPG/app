using FluentValidation;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Passwords.Validators;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Application.Accounts.Validators;

internal class ChangePasswordValidator : AbstractValidator<ChangeAccountPasswordPayload>
{
  public ChangePasswordValidator(IPasswordSettings passwordSettings)
  {
    RuleFor(x => x.Current).NotEmpty();
    RuleFor(x => x.New).SetValidator(new PasswordValidator(passwordSettings));
  }
}
