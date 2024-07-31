using Bogus;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Users;
using System.Text.Json;

namespace SkillCraft.Application.Accounts;

[Trait(Traits.Category, Categories.Unit)]
public class OneTimePasswordExtensionsTests
{
  private const string Purpose = "MultiFactorAuthentication";

  private readonly Faker _faker = new();

  [Fact(DisplayName = "EnsurePurpose: it should do nothing when the One-Time Password has the expected purpose.")]
  public void EnsurePurpose_it_should_do_nothing_when_the_One_Time_Password_has_the_expected_purpose()
  {
    OneTimePassword password = new();
    password.CustomAttributes.Add(new("Purpose", Purpose));
    password.EnsurePurpose(Purpose);
  }

  [Fact(DisplayName = "EnsurePurpose: it should throw InvalidOneTimePasswordPurpose when the One-Time Password has no purpose.")]
  public void EnsurePurpose_it_should_throw_InvalidOneTimePasswordPurpose_when_the_One_Time_Password_has_no_purpose()
  {
    OneTimePassword password = new()
    {
      Id = Guid.NewGuid()
    };
    Assert.Empty(password.CustomAttributes);
    var exception = Assert.Throws<InvalidOneTimePasswordPurposeException>(() => password.EnsurePurpose(Purpose));
    Assert.Equal(password.Id, exception.OneTimePasswordId);
    Assert.Equal(Purpose, exception.ExpectedPurpose);
    Assert.Null(exception.ActualPurpose);
  }

  [Fact(DisplayName = "EnsurePurpose: it should throw InvalidOneTimePasswordPurpose when the purpose is not expected.")]
  public void EnsurePurpose_it_should_throw_InvalidOneTimePasswordPurpose_when_the_purpose_is_not_expected()
  {
    OneTimePassword password = new()
    {
      Id = Guid.NewGuid()
    };

    string purpose = "Test";
    password.CustomAttributes.Add(new("Purpose", purpose));

    var exception = Assert.Throws<InvalidOneTimePasswordPurposeException>(() => password.EnsurePurpose(Purpose));
    Assert.Equal(password.Id, exception.OneTimePasswordId);
    Assert.Equal(Purpose, exception.ExpectedPurpose);
    Assert.Equal(purpose, exception.ActualPurpose);
  }

  [Fact(DisplayName = "GetCustomAttribute: it should return the found value.")]
  public void GetCustomAttribute_it_should_return_the_found_value()
  {
    string userId = Guid.NewGuid().ToString();
    OneTimePassword oneTimePassword = new()
    {
      Id = Guid.NewGuid()
    };
    oneTimePassword.CustomAttributes.Add(new("UserId", userId));
    Assert.Equal(userId, oneTimePassword.GetCustomAttribute("UserId"));
  }

  [Fact(DisplayName = "GetCustomAttribute: it should throw ArgumentException when a custom attribute is not found.")]
  public void GetCustomAttribute_it_should_throw_ArgumentException_when_a_custom_attribute_is_not_found()
  {
    OneTimePassword oneTimePassword = new()
    {
      Id = Guid.NewGuid()
    };
    var exception = Assert.Throws<ArgumentException>(() => oneTimePassword.GetCustomAttribute("UserId"));
    Assert.StartsWith($"The One-Time Password (OTP) 'Id={oneTimePassword.Id}' has no 'UserId' custom attribute.", exception.Message);
    Assert.Equal("oneTimePassword", exception.ParamName);
  }

  [Fact(DisplayName = "GetCustomAttribute: it should throw ArgumentException when multiple custom attributes were found.")]
  public void GetCustomAttribute_it_should_throw_ArgumentException_when_multiple_custom_attributes_were_found()
  {
    OneTimePassword oneTimePassword = new()
    {
      Id = Guid.NewGuid()
    };
    oneTimePassword.CustomAttributes.Add(new("UserId", Guid.NewGuid().ToString()));
    oneTimePassword.CustomAttributes.Add(new("UserId", Guid.NewGuid().ToString()));
    var exception = Assert.Throws<ArgumentException>(() => oneTimePassword.GetCustomAttribute("UserId"));
    Assert.StartsWith($"The One-Time Password (OTP) 'Id={oneTimePassword.Id}' has 2 'UserId' custom attributes.", exception.Message);
    Assert.Equal("oneTimePassword", exception.ParamName);
  }

  [Fact(DisplayName = "GetPhone: it should return the correct phone.")]
  public void GetPhone_it_should_return_the_correct_phone()
  {
    Phone phone = new("CA", "(514) 845-4636", extension: null, "+15148454636");
    string json = @"{""CountryCode"":""CA"",""Number"":""(514) 845-4636"",""E164Formatted"":""+15148454636""}";

    OneTimePassword oneTimePassword = new();
    oneTimePassword.CustomAttributes.Add(new("Phone", json));

    Phone result = oneTimePassword.GetPhone();
    Assert.Equal(phone, result);
  }

  [Fact(DisplayName = "GetPurpose: it should return the purpose when the One-Time Password has one.")]
  public void GetPurpose_it_should_return_the_purpose_when_the_One_Time_Password_has_one()
  {
    OneTimePassword password = new();
    password.CustomAttributes.Add(new("Purpose", Purpose));
    Assert.Equal(Purpose, password.GetPurpose());
  }

  [Fact(DisplayName = "GetPurpose: it should throw ArgumentException when the One-Time Password has no purpose.")]
  public void GetPurpose_it_should_throw_ArgumentException_when_the_One_Time_Password_has_no_purpose()
  {
    OneTimePassword password = new()
    {
      Id = Guid.NewGuid()
    };
    Assert.Empty(password.CustomAttributes);
    var exception = Assert.Throws<ArgumentException>(password.GetPurpose);
    Assert.StartsWith($"The One-Time Password (OTP) 'Id={password.Id}' has no 'Purpose' custom attribute.", exception.Message);
    Assert.Equal("oneTimePassword", exception.ParamName);
  }

  [Fact(DisplayName = "UserId: it should return the correct user ID.")]
  public void GetUserId_it_should_return_the_correct_user_Id()
  {
    Guid userId = Guid.NewGuid();
    OneTimePassword oneTimePassword = new()
    {
      Id = Guid.NewGuid()
    };
    oneTimePassword.CustomAttributes.Add(new("UserId", userId.ToString()));
    Assert.Equal(userId, oneTimePassword.GetUserId());
  }

  [Fact(DisplayName = "HasCustomAttribute: it should return false when the custom attribute could not be found.")]
  public void HasCustomAttribute_it_should_return_false_when_the_custom_attribute_could_not_be_found()
  {
    OneTimePassword oneTimePassword = new()
    {
      Id = Guid.NewGuid()
    };
    Assert.False(oneTimePassword.HasCustomAttribute("UserId"));
  }

  [Fact(DisplayName = "HasCustomAttribute: it should return true when the custom attribute was be found.")]
  public void HasCustomAttribute_it_should_return_true_when_the_custom_attribute_was_found()
  {
    OneTimePassword oneTimePassword = new()
    {
      Id = Guid.NewGuid()
    };
    oneTimePassword.CustomAttributes.Add(new("UserId", Guid.NewGuid().ToString()));
    Assert.True(oneTimePassword.HasCustomAttribute("UserId"));
  }

  [Fact(DisplayName = "HasCustomAttribute: it should throw ArgumentException when multiple custom attributes were found.")]
  public void HasCustomAttribute_it_should_throw_ArgumentException_when_multiple_custom_attributes_were_found()
  {
    OneTimePassword oneTimePassword = new()
    {
      Id = Guid.NewGuid()
    };
    oneTimePassword.CustomAttributes.Add(new("UserId", Guid.NewGuid().ToString()));
    oneTimePassword.CustomAttributes.Add(new("UserId", Guid.NewGuid().ToString()));
    var exception = Assert.Throws<ArgumentException>(() => oneTimePassword.HasCustomAttribute("UserId"));
    Assert.StartsWith($"The One-Time Password (OTP) 'Id={oneTimePassword.Id}' has 2 'UserId' custom attributes.", exception.Message);
    Assert.Equal("oneTimePassword", exception.ParamName);
  }

  [Fact(DisplayName = "HasPurpose: it should return false when the One-Time Password does not have the expected purpose.")]
  public void HasPurpose_it_should_return_false_when_the_One_Time_Password_does_not_have_the_expected_purpose()
  {
    OneTimePassword password = new();
    Assert.Empty(password.CustomAttributes);
    Assert.False(password.HasPurpose(Purpose));

    password.CustomAttributes.Add(new("Purpose", "Test"));
    Assert.False(password.HasPurpose(Purpose));
  }

  [Fact(DisplayName = "HasPurpose: it should return true when the One-Time Password has the expected purpose.")]
  public void HasPurpose_it_should_return_true_when_the_One_Time_Password_has_the_expected_purpose()
  {
    OneTimePassword password = new();
    password.CustomAttributes.Add(new("Purpose", Purpose.ToUpper()));
    Assert.True(password.HasPurpose(Purpose.ToLower()));
  }

  [Fact(DisplayName = "SetPhone: it should add the correct custom attribute.")]
  public void SetPhone_it_should_add_the_correct_custom_attribute()
  {
    Phone phone = new("CA", "(514) 845-4636", extension: null, "+15148454636")
    {
      IsVerified = true,
      VerifiedBy = Actor.System,
      VerifiedOn = DateTime.Now
    };
    CreateOneTimePasswordPayload payload = new();
    payload.SetPhone(phone);

    string json = JsonSerializer.Serialize(phone);
    Assert.Equal(json, Assert.Single(payload.CustomAttributes, c => c.Key == "Phone").Value);
  }

  [Fact(DisplayName = "SetPurpose: it should add the correct custom attribute.")]
  public void SetPurpose_it_should_add_the_correct_custom_attribute()
  {
    CreateOneTimePasswordPayload payload = new();
    Assert.Empty(payload.CustomAttributes);

    payload.SetPurpose(Purpose);
    Assert.Contains(payload.CustomAttributes, c => c.Key == "Purpose" && c.Value == Purpose);
  }

  [Fact(DisplayName = "SetPurpose: it should replace the correct custom attribute.")]
  public void SetPurpose_it_should_replace_the_correct_custom_attribute()
  {
    CreateOneTimePasswordPayload payload = new();
    payload.CustomAttributes.Add(new("UserId", Guid.NewGuid().ToString()));

    payload.SetPurpose(Purpose);
    Assert.Single(payload.CustomAttributes, c => c.Key == "Purpose" && c.Value == Purpose);
  }

  [Fact(DisplayName = "SetUserId: it should add the correct custom attribute.")]
  public void SetUserId_it_should_add_the_correct_custom_attribute()
  {
    CreateOneTimePasswordPayload payload = new();
    Assert.Empty(payload.CustomAttributes);

    User user = new(_faker.Person.UserName)
    {
      Id = Guid.NewGuid()
    };
    payload.SetUserId(user);
    Assert.Contains(payload.CustomAttributes, c => c.Key == "UserId" && c.Value == user.Id.ToString());
  }

  [Fact(DisplayName = "SetUserId: it should replace the correct custom attribute.")]
  public void SetUserId_it_should_replace_the_correct_custom_attribute()
  {
    CreateOneTimePasswordPayload payload = new();
    payload.CustomAttributes.Add(new("UserId", Guid.NewGuid().ToString()));

    User user = new(_faker.Person.UserName)
    {
      Id = Guid.NewGuid()
    };
    payload.SetUserId(user);
    Assert.Single(payload.CustomAttributes, c => c.Key == "UserId" && c.Value == user.Id.ToString());
  }

  [Fact(DisplayName = "TryGetCustomAttribute: it return null when a custom attribute is not found.")]
  public void TryGetCustomAttribute_it_should_throw_ArgumentException_when_a_custom_attribute_is_not_found()
  {
    OneTimePassword oneTimePassword = new()
    {
      Id = Guid.NewGuid()
    };
    Assert.Null(oneTimePassword.TryGetCustomAttribute("UserId"));
  }

  [Fact(DisplayName = "TryGetCustomAttribute: it should return the found value.")]
  public void TryGetCustomAttribute_it_should_return_the_found_value()
  {
    string userId = Guid.NewGuid().ToString();
    OneTimePassword oneTimePassword = new()
    {
      Id = Guid.NewGuid()
    };
    oneTimePassword.CustomAttributes.Add(new("UserId", userId));
    Assert.Equal(userId, oneTimePassword.TryGetCustomAttribute("UserId"));
  }

  [Fact(DisplayName = "TryGetCustomAttribute: it should throw ArgumentException when multiple custom attributes were found.")]
  public void TryGetCustomAttribute_it_should_throw_ArgumentException_when_multiple_custom_attributes_were_found()
  {
    OneTimePassword oneTimePassword = new()
    {
      Id = Guid.NewGuid()
    };
    oneTimePassword.CustomAttributes.Add(new("UserId", Guid.NewGuid().ToString()));
    oneTimePassword.CustomAttributes.Add(new("UserId", Guid.NewGuid().ToString()));
    var exception = Assert.Throws<ArgumentException>(() => oneTimePassword.TryGetCustomAttribute("UserId"));
    Assert.StartsWith($"The One-Time Password (OTP) 'Id={oneTimePassword.Id}' has 2 'UserId' custom attributes.", exception.Message);
    Assert.Equal("oneTimePassword", exception.ParamName);
  }

  [Fact(DisplayName = "TryGetPurpose: it should return null when the One-Time Password has no purpose.")]
  public void TryGetPurpose_it_should_return_null_when_the_One_Time_Password_has_no_purpose()
  {
    OneTimePassword password = new();
    Assert.Empty(password.CustomAttributes);
    Assert.Null(password.TryGetPurpose());
  }

  [Fact(DisplayName = "TryGetPurpose: it should return the purpose when the One-Time Password has one.")]
  public void TryGetPurpose_it_should_return_the_purpose_when_the_One_Time_Password_has_one()
  {
    OneTimePassword password = new();
    password.CustomAttributes.Add(new("Purpose", Purpose));
    Assert.Equal(Purpose, password.TryGetPurpose());
  }
}
