using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Users;
using SkillCraft.Application.Accounts;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Infrastructure.IdentityServices;

internal class OneTimePasswordService : IOneTimePasswordService
{
  private const string Characters = "0123456789";
  private const int Length = 6;

  private readonly IOneTimePasswordClient _oneTimePasswordClient;

  public OneTimePasswordService(IOneTimePasswordClient oneTimePasswordClient)
  {
    _oneTimePasswordClient = oneTimePasswordClient;
  }

  public async Task<OneTimePassword> CreateAsync(User user, string purpose, CancellationToken cancellationToken)
  {
    CreateOneTimePasswordPayload payload = new(Characters, Length)
    {
      ExpiresOn = DateTime.Now.AddSeconds(3600),
      MaximumAttempts = 5
    };
    payload.SetUserId(user);
    payload.SetPurpose(purpose);
    RequestContext context = new(cancellationToken);
    return await _oneTimePasswordClient.CreateAsync(payload, context);
  }

  public async Task<OneTimePassword> ValidateAsync(OneTimePasswordPayload oneTimePassword, CancellationToken cancellationToken)
  {
    ValidateOneTimePasswordPayload payload = new(oneTimePassword.Code);
    RequestContext context = new(cancellationToken);
    return await _oneTimePasswordClient.ValidateAsync(oneTimePassword.Id, payload, context)
      ?? throw new InvalidOperationException($"The One-Time Password (OTP) 'Id={oneTimePassword.Id}' validation returned null.");
  }
}
