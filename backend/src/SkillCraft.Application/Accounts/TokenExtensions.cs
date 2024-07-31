using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;

namespace SkillCraft.Application.Accounts;

internal static class TokenExtensions
{
  public static EmailPayload GetEmailPayload(this ValidatedToken validatedToken)
  {
    if (validatedToken.Email == null)
    {
      throw new ArgumentException($"The {nameof(validatedToken.Email)} is required.", nameof(validatedToken));
    }

    return new EmailPayload(validatedToken.Email.Address, validatedToken.Email.IsVerified);
  }

  public static Guid GetUserId(this ValidatedToken validatedToken)
  {
    Guid userId;

    if (validatedToken.Subject == null)
    {
      throw new ArgumentException($"The '{nameof(validatedToken.Subject)}' claim is required.", nameof(validatedToken));
    }
    else if (!Guid.TryParse(validatedToken.Subject, out userId))
    {
      throw new ArgumentException($"The Subject claim value '{validatedToken.Subject}' is not a valid Guid.", nameof(validatedToken));
    }

    return userId;
  }
}
