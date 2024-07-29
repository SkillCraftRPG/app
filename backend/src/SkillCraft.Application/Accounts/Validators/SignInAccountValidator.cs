﻿using FluentValidation;
using SkillCraft.Contracts.Accounts;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Accounts.Validators;

internal class SignInAccountValidator : AbstractValidator<SignInAccountPayload>
{
  public SignInAccountValidator()
  {
    RuleFor(x => x.Locale).Locale();

    When(x => x.Credentials != null, () => RuleFor(x => x.Credentials!).SetValidator(new CredentialsValidator()));
    //When(x => x.OneTimePassword != null, () => RuleFor(x => x.OneTimePassword!).SetValidator(new OneTimePasswordValidator())); // TODO(fpion): OneTimePassword
    //When(x => x.Profile != null, () => RuleFor(x => x.Profile!).SetValidator(new CompleteProfileValidator(passwordSettings))); // TODO(fpion): Profile

    RuleFor(x => x).Must(BeAValidPayload).WithErrorCode(nameof(SignInAccountValidator))
      .WithMessage(x => $"Exactly one of the following must be specified: {nameof(x.Credentials)}, {nameof(x.AuthenticationToken)}."); // TODO(fpion): OneTimePassword, Profile
  }

  private static bool BeAValidPayload(SignInAccountPayload payload)
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
    //if (payload.OneTimePassword != null)
    //{
    //  count++;
    //} // TODO(fpion): OneTimePassword
    //if (payload.Profile != null)
    //{
    //  count++;
    //} // TODO(fpion): Profile
    return count == 1;
  }
}
