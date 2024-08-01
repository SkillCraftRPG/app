using FluentValidation;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Passwords.Validators;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Application.Accounts.Validators;

internal class ResetPayloadValidator : AbstractValidator<ResetPayload>
{
  public ResetPayloadValidator(IPasswordSettings passwordSettings)
  {
    RuleFor(x => x.Token).NotEmpty();
    RuleFor(x => x.Password).SetValidator(new PasswordValidator(passwordSettings));
  }
}
