﻿using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Users;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Application.Accounts;

public interface IOneTimePasswordService
{
  Task<OneTimePassword> CreateAsync(User user, string purpose, CancellationToken cancellationToken = default);
  Task<OneTimePassword> ValidateAsync(OneTimePasswordPayload oneTimePassword, CancellationToken cancellationToken = default);
}
