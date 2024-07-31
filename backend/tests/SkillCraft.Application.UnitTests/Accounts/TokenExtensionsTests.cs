using Bogus;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using Logitar.Security.Claims;
using SkillCraft.Application.Accounts.Constants;

namespace SkillCraft.Application.Accounts;

[Trait(Traits.Category, Categories.Unit)]
public class TokenExtensionsTests
{
  private readonly Faker _faker = new();

  [Theory(DisplayName = "FormatToE164WithExtension: it should format the phone correctly.")]
  [InlineData("+15148454636", null)]
  [InlineData("+15148454636", "12345")]
  public void FormatToE164WithExtension_it_should_format_the_phone_correctly(string e164Formatted, string? extension)
  {
    string expected = extension == null ? e164Formatted : $"{e164Formatted};ext={extension}";
    Phone phone = new(countryCode: null, e164Formatted, extension, e164Formatted);
    Assert.Equal(expected, phone.FormatToE164WithExtension());
  }

  [Fact(DisplayName = "GetEmailPayload: it should return the correct email payload.")]
  public void GetEmailPayload_it_should_return_the_correct_email_payload()
  {
    ValidatedToken validatedToken = new()
    {
      Email = new(_faker.Person.Email)
      {
        IsVerified = true
      }
    };
    EmailPayload payload = validatedToken.GetEmailPayload();
    Assert.Equal(validatedToken.Email.Address, payload.Address);
    Assert.Equal(validatedToken.Email.IsVerified, payload.IsVerified);
  }

  [Fact(DisplayName = "GetEmailPayload: it should throw ArgumentException when the email claims are missing.")]
  public void GetEmailPayload_it_should_throw_ArgumentException_when_the_email_claims_are_missing()
  {
    ValidatedToken validatedToken = new();
    var exception = Assert.Throws<ArgumentException>(() => validatedToken.GetEmailPayload());
    Assert.StartsWith("The Email is required.", exception.Message);
    Assert.Equal("validatedToken", exception.ParamName);
  }

  [Fact(DisplayName = "GetUserId: it should return the user ID.")]
  public void GetUserId_it_should_return_the_user_Id()
  {
    Guid userId = Guid.NewGuid();
    ValidatedToken validatedToken = new()
    {
      Subject = userId.ToString()
    };
    Assert.Equal(userId, validatedToken.GetUserId());
  }

  [Fact(DisplayName = "GetUserId: it should throw ArgumentException when the subject claim is missing.")]
  public void GetUserId_it_should_throw_ArgumentException_when_the_subject_claim_is_missing()
  {
    ValidatedToken validatedToken = new();
    var exception = Assert.Throws<ArgumentException>(() => validatedToken.GetUserId());
    Assert.StartsWith("The 'Subject' claim is required.", exception.Message);
    Assert.Equal("validatedToken", exception.ParamName);
  }

  [Fact(DisplayName = "GetUserId: it should throw ArgumentException when the subject is not a valid Guid.")]
  public void GetUserId_it_should_throw_ArgumentException_when_the_subject_is_not_a_valid_Guid()
  {
    ValidatedToken validatedToken = new()
    {
      Subject = _faker.Person.UserName
    };
    var exception = Assert.Throws<ArgumentException>(() => validatedToken.GetUserId());
    Assert.StartsWith($"The Subject claim value '{validatedToken.Subject}' is not a valid Guid.", exception.Message);
    Assert.Equal("validatedToken", exception.ParamName);
  }

  [Theory(DisplayName = "ParsePhone: it should parse the correct phone.")]
  [InlineData("+15148454636", null)]
  [InlineData("+15148454636", "12345")]
  public void ParsePhone_it_should_parse_the_correct_phone(string e164Formatted, string? extension)
  {
    string value = extension == null ? e164Formatted : $"{e164Formatted};ext={extension}";
    TokenClaim claim = new(Rfc7519ClaimNames.PhoneNumber, value);
    Phone phone = claim.ParsePhone();
    Assert.Null(phone.CountryCode);
    Assert.Equal(e164Formatted, phone.Number);
    Assert.Equal(extension, phone.Extension);
    Assert.Equal(e164Formatted, phone.E164Formatted);
    Assert.False(phone.IsVerified);
    Assert.Null(phone.VerifiedBy);
    Assert.Null(phone.VerifiedOn);
  }

  [Theory(DisplayName = "TryGetPhonePayload: it should return the correct phone payload.")]
  [InlineData(null, null, null, false)]
  [InlineData(null, null, "+15148454636", false)]
  [InlineData("CA", "12345", "+15148454636", true)]
  public void TryGetPhonePayload_it_should_return_null_when_the_number_is_missing(string? countryCode, string? extension, string? e164Formatted, bool isVerified)
  {
    ValidatedToken validatedToken = new();

    Phone phone = new(countryCode, number: string.Empty, extension, e164Formatted ?? string.Empty)
    {
      IsVerified = isVerified
    };
    if (phone.CountryCode != null)
    {
      validatedToken.Claims.Add(new(ClaimNames.PhoneCountryCode, phone.CountryCode));
    }
    validatedToken.Claims.Add(new(Rfc7519ClaimNames.PhoneNumber, phone.FormatToE164WithExtension()));
    validatedToken.Claims.Add(new(Rfc7519ClaimNames.IsPhoneVerified, phone.IsVerified.ToString().ToLowerInvariant(), "http://www.w3.org/2001/XMLSchema#boolean"));

    Assert.Null(validatedToken.TryGetPhonePayload());
  }

  [Theory(DisplayName = "TryGetPhonePayload: it should return the correct phone payload.")]
  [InlineData(null, "(514) 845-4636", null, false)]
  [InlineData("CA", "(514) 845-4636", "12345", true)]
  public void TryGetPhonePayload_it_should_return_the_correct_phone_payload(string? countryCode, string number, string? extension, bool isVerified)
  {
    ValidatedToken validatedToken = new();

    Phone phone = new(countryCode, number, extension, e164Formatted: string.Empty)
    {
      IsVerified = isVerified
    };
    phone.E164Formatted = phone.FormatToE164();
    if (phone.CountryCode != null)
    {
      validatedToken.Claims.Add(new(ClaimNames.PhoneCountryCode, phone.CountryCode));
    }
    validatedToken.Claims.Add(new(ClaimNames.PhoneNumberRaw, phone.Number));
    validatedToken.Claims.Add(new(Rfc7519ClaimNames.PhoneNumber, phone.FormatToE164WithExtension()));
    validatedToken.Claims.Add(new(Rfc7519ClaimNames.IsPhoneVerified, phone.IsVerified.ToString().ToLowerInvariant(), "http://www.w3.org/2001/XMLSchema#boolean"));

    PhonePayload? payload = validatedToken.TryGetPhonePayload();
    Assert.NotNull(payload);

    Assert.Equal(phone.CountryCode, payload.CountryCode);
    Assert.Equal(phone.Number, payload.Number);
    Assert.Equal(phone.Extension, payload.Extension);
    Assert.Equal(phone.IsVerified, payload.IsVerified);
  }
}
