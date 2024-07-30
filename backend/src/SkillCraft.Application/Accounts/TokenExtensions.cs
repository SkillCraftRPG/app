using Logitar;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using Logitar.Security.Claims;
using SkillCraft.Application.Accounts.Constants;

namespace SkillCraft.Application.Accounts;

internal static class TokenExtensions
{
  public static PhonePayload? GetPhonePayload(this ValidatedToken validatedToken)
  {
    PhonePayload phone = new();
    foreach (TokenClaim claim in validatedToken.Claims)
    {
      switch (claim.Name)
      {
        case ClaimNames.PhoneCountryCode:
          phone.CountryCode = claim.Value;
          break;
        case ClaimNames.PhoneNumberRaw:
          phone.Number = claim.Value;
          break;
        case Rfc7519ClaimNames.IsPhoneVerified:
          phone.IsVerified = bool.Parse(claim.Value);
          break;
        case Rfc7519ClaimNames.PhoneNumber:
          int index = claim.Value.IndexOf(';');
          if (index >= 0)
          {
            phone.Extension = claim.Value[(index + 1)..].Remove("ext=");
          }
          break;
      }
    }
    if (string.IsNullOrWhiteSpace(phone.Number))
    {
      return null;
    }
    return phone;
  }
}
