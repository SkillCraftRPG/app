﻿using Logitar;

namespace SkillCraft.Application.Accounts;

public class OneTimePasswordNotFoundException : Exception // TODO(fpion): InvalidCredentialsException
{
  private const string ErrorMessage = "The specified One-Time Password (OTP) could not be found.";

  public Guid OneTimePasswordId
  {
    get => (Guid)Data[nameof(OneTimePasswordId)]!;
    private set => Data[nameof(OneTimePasswordId)] = value;
  }

  public OneTimePasswordNotFoundException(Guid oneTimePasswordId) : base(BuildMessage(oneTimePasswordId))
  {
    OneTimePasswordId = oneTimePasswordId;
  }

  private static string BuildMessage(Guid oneTimePasswordId) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(OneTimePasswordId), oneTimePasswordId)
    .Build();
}
