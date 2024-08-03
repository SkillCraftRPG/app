using Google.Apis.Auth;
using Logitar;
using Logitar.Portal.Contracts.Users;
using SkillCraft.Application.Accounts;

namespace SkillCraft.Infrastructure.IdentityServices;

internal class GoogleService : IGoogleService
{
  public async Task<GoogleIdentity> GetIdentityAsync(string token, CancellationToken cancellationToken)
  {
    GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(token);
    EmailPayload email = new(payload.Email, payload.EmailVerified);
    if (!email.IsVerified)
    {
      throw new ArgumentException("The email is not verified.", nameof(token));
    }

    return new GoogleIdentity(payload.Subject,
      email,
      payload.GivenName?.CleanTrim(),
      payload.FamilyName?.CleanTrim(),
      payload.Locale?.CleanTrim(),
      payload.Picture?.CleanTrim());
  }
}
