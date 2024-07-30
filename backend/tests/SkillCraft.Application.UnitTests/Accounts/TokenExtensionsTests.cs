using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using Logitar.Security.Claims;
using SkillCraft.Application.Accounts.Constants;

namespace SkillCraft.Application.Accounts;

[Trait(Traits.Category, Categories.Unit)]
public class TokenExtensionsTests
{
  [Theory(DisplayName = "GetPhonePayload: it should return the correct phone from token claims.")]
  [InlineData(null, "(514) 845-4636", null, "+15148454636", false)]
  [InlineData("CA", "(514) 845-4636", "98772", "+15148454636", true)]
  [InlineData("CA", "(514) 845-4636", "98772", "", false)]
  [InlineData("CA", "          ", null, "+15148454636", false)]
  public void GetPhonePayload_it_should_return_the_correct_phone_from_token_claims(string? countryCode, string number, string? extension, string e164Formatted, bool isVerified)
  {
    ValidatedToken validatedToken = new();
    if (countryCode != null)
    {
      validatedToken.Claims.Add(new TokenClaim(ClaimNames.PhoneCountryCode, countryCode));
    }
    validatedToken.Claims.Add(new TokenClaim(ClaimNames.PhoneNumberRaw, number));
    validatedToken.Claims.Add(new TokenClaim(Rfc7519ClaimNames.PhoneNumber, extension == null ? e164Formatted : $"{e164Formatted};ext={extension}"));
    validatedToken.Claims.Add(new TokenClaim(Rfc7519ClaimNames.IsPhoneVerified, isVerified.ToString().ToLower(), ClaimValueTypes.Boolean));

    PhonePayload? phone = validatedToken.GetPhonePayload();
    if (string.IsNullOrWhiteSpace(number))
    {
      Assert.Null(phone);
    }
    else
    {
      Assert.NotNull(phone);
      Assert.Equal(countryCode, phone.CountryCode);
      Assert.Equal(number, phone.Number);
      Assert.Equal(extension, phone.Extension);
      Assert.Equal(isVerified, phone.IsVerified);
    }
  }
}
