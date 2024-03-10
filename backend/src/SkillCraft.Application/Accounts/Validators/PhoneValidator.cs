using FluentValidation;
using Logitar.Identity.Domain.Users;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Application.Accounts.Validators;

internal class PhoneValidator : AbstractValidator<Phone>
{
  public PhoneValidator()
  {
    When(x => x.CountryCode != null, () => RuleFor(x => x.CountryCode).NotEmpty().Length(2));
    RuleFor(x => x.Number).NotEmpty().MaximumLength(20);

    RuleFor(x => x).Must(phone => phone.ToPhonePayload().IsValid())
      .WithErrorCode(nameof(PhoneValidator))
      .WithMessage("'{PropertyName}' must be a valid phone.");
  }
}
