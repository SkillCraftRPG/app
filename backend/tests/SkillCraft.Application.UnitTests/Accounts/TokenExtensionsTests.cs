using Bogus;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;

namespace SkillCraft.Application.Accounts;

[Trait(Traits.Category, Categories.Unit)]
public class TokenExtensionsTests
{
  private readonly Faker _faker = new();

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
}
