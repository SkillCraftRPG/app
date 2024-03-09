using FluentValidation;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Application.Accounts.Validators;

internal class CredentialsValidator : AbstractValidator<CredentialsPayload>
{
  public CredentialsValidator()
  {
    RuleFor(x => x.EmailAddress).NotEmpty().MaximumLength(byte.MaxValue).EmailAddress();
  }
}
