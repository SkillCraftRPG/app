﻿using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using SkillCraft.Application.Accounts;

namespace SkillCraft.Infrastructure.IdentityServices;

internal class TokenService : ITokenService
{
  private readonly ITokenClient _tokenClient;

  public TokenService(ITokenClient tokenClient)
  {
    _tokenClient = tokenClient;
  }

  public async Task<CreatedToken> CreateAsync(User user, string type, CancellationToken cancellationToken)
  {
    return await CreateAsync(user.GetSubject(), email: null, phone: null, type, cancellationToken);
  }
  public async Task<CreatedToken> CreateAsync(User? user, Email email, string type, CancellationToken cancellationToken)
  {
    return await CreateAsync(user?.GetSubject(), email, phone: null, type, cancellationToken);
  }
  public async Task<CreatedToken> CreateAsync(User? user, Phone phone, string type, CancellationToken cancellationToken)
  {
    return await CreateAsync(user?.GetSubject(), email: null, phone, type, cancellationToken);
  }
  private async Task<CreatedToken> CreateAsync(string? subject, Email? email, Phone? phone, string type, CancellationToken cancellationToken)
  {
    CreateTokenPayload payload = new()
    {
      IsConsumable = true,
      LifetimeSeconds = 3600,
      Type = type,
      Subject = subject,
    };
    if (email != null)
    {
      payload.Email = new EmailPayload(email.Address, email.IsVerified);
    }
    if (phone != null)
    {
      // TODO(fpion): implement
    }
    RequestContext context = new(cancellationToken);
    return await _tokenClient.CreateAsync(payload, context);
  }

  public async Task<ValidatedToken> ValidateAsync(string token, string type, CancellationToken cancellationToken)
  {
    return await ValidateAsync(token, consume: true, type, cancellationToken);
  }
  public async Task<ValidatedToken> ValidateAsync(string token, bool consume, string type, CancellationToken cancellationToken)
  {
    ValidateTokenPayload payload = new(token)
    {
      Consume = consume,
      Type = type
    };
    RequestContext context = new(cancellationToken);
    return await _tokenClient.ValidateAsync(payload, context);
  }
}
