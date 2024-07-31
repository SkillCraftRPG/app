using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using Logitar.Security.Claims;
using SkillCraft.Application.Accounts.Constants;

namespace SkillCraft.Application.Accounts;

public static class TokenExtensions
{
  private const string PhoneExtensionPrefix = ";ext=";

  public static string FormatToE164WithExtension(this Phone phone)
  {
    StringBuilder formatted = new();

    formatted.Append(phone.E164Formatted);

    if (phone.Extension != null)
    {
      formatted.Append(PhoneExtensionPrefix);
      formatted.Append(phone.Extension);
    }

    return formatted.ToString();
  }

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

  public static Phone ParsePhone(this TokenClaim claim)
  {
    Phone phone = new();

    int index = claim.Value.IndexOf(PhoneExtensionPrefix);
    if (index < 0)
    {
      phone.Number = phone.E164Formatted = claim.Value;
    }
    else
    {
      phone.Number = phone.E164Formatted = claim.Value[..index];
      phone.Extension = claim.Value[(index + PhoneExtensionPrefix.Length)..];
    }

    return phone;
  }

  public static PhonePayload? TryGetPhonePayload(this ValidatedToken validatedToken)
  {
    PhonePayload payload = new();

    foreach (TokenClaim claim in validatedToken.Claims)
    {
      switch (claim.Name)
      {
        case ClaimNames.PhoneCountryCode:
          payload.CountryCode = claim.Value;
          break;
        case ClaimNames.PhoneNumberRaw:
          payload.Number = claim.Value;
          break;
        case Rfc7519ClaimNames.PhoneNumber:
          Phone phone = claim.ParsePhone();
          payload.Extension = phone.Extension;
          break;
        case Rfc7519ClaimNames.IsPhoneVerified:
          payload.IsVerified = bool.Parse(claim.Value);
          break;
      }
    }

    return string.IsNullOrWhiteSpace(payload.Number) ? null : payload;
  }
}
