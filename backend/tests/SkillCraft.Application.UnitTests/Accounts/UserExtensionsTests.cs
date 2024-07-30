using Bogus;
using Logitar;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Application.Accounts;

[Trait(Traits.Category, Categories.Unit)]
public class UserExtensionsTests
{
  private readonly Faker _faker = new();

  [Fact(DisplayName = "CompleteProfile: it should set the correct custom attribute on the payload.")]
  public void CompleteProfile_it_should_set_the_correct_custom_attribute_on_the_payload()
  {
    UpdateUserPayload payload = new();
    Assert.Empty(payload.CustomAttributes);

    payload.CompleteProfile();
    Assert.Contains(payload.CustomAttributes, c => c.Key == "ProfileCompletedOn" && DateTime.TryParse(c.Value, out _));
  }

  [Fact(DisplayName = "GetCustomAttribute: it should return the found value.")]
  public void GetCustomAttribute_it_should_return_the_found_value()
  {
    string mfaMode = MultiFactorAuthenticationMode.Email.ToString();
    User user = new(_faker.Person.Email)
    {
      Id = Guid.NewGuid()
    };
    user.CustomAttributes.Add(new("MultiFactorAuthenticationMode", mfaMode));
    Assert.Equal(mfaMode, user.GetCustomAttribute("MultiFactorAuthenticationMode"));
  }

  [Fact(DisplayName = "GetCustomAttribute: it should throw ArgumentException when a custom attribute is not found.")]
  public void GetCustomAttribute_it_should_throw_ArgumentException_when_a_custom_attribute_is_not_found()
  {
    User user = new(_faker.Person.Email)
    {
      Id = Guid.NewGuid()
    };
    var exception = Assert.Throws<ArgumentException>(() => user.GetCustomAttribute("MultiFactorAuthenticationMode"));
    Assert.StartsWith($"The user 'Id={user.Id}' has no 'MultiFactorAuthenticationMode' custom attribute.", exception.Message);
    Assert.Equal("user", exception.ParamName);
  }

  [Fact(DisplayName = "GetCustomAttribute: it should throw ArgumentException when multiple custom attributes were found.")]
  public void GetCustomAttribute_it_should_throw_ArgumentException_when_multiple_custom_attributes_were_found()
  {
    User user = new()
    {
      Id = Guid.NewGuid()
    };
    user.CustomAttributes.Add(new("MultiFactorAuthenticationMode", "Email"));
    user.CustomAttributes.Add(new("MultiFactorAuthenticationMode", "Phone"));
    var exception = Assert.Throws<ArgumentException>(() => user.GetCustomAttribute("MultiFactorAuthenticationMode"));
    Assert.StartsWith($"The user 'Id={user.Id}' has 2 'MultiFactorAuthenticationMode' custom attributes.", exception.Message);
    Assert.Equal("user", exception.ParamName);
  }

  [Fact(DisplayName = "GetMultiFactorAuthenticationMode: it should return null when the user does not have the custom attribute.")]
  public void GetMultiFactorAuthenticationMode_it_should_return_null_when_the_user_does_not_have_the_custom_attribute()
  {
    User user = new(_faker.Person.UserName);
    Assert.Null(user.GetMultiFactorAuthenticationMode());
  }

  [Theory(DisplayName = "GetMultiFactorAuthenticationMode: it should return the correct value when the user has the custom attribute.")]
  [InlineData(MultiFactorAuthenticationMode.Phone)]
  public void GetMultiFactorAuthenticationMode_it_should_return_the_correct_value_when_the_user_has_the_custom_attribute(MultiFactorAuthenticationMode mfaMode)
  {
    User user = new(_faker.Person.UserName);
    user.CustomAttributes.Add(new(nameof(MultiFactorAuthenticationMode), mfaMode.ToString()));
    Assert.Equal(mfaMode, user.GetMultiFactorAuthenticationMode());
  }

  [Fact(DisplayName = "GetProfileCompleted: it should throw ArgumentException when the user profile is not completed.")]
  public void GetProfileCompleted_it_should_throw_ArgumentException_when_the_user_profile_is_not_completed()
  {
    User user = new(_faker.Person.UserName);
    Assert.Empty(user.CustomAttributes);
    var exception = Assert.Throws<ArgumentException>(user.ToUserProfile);
    Assert.StartsWith($"The user 'Id={user.Id}' has no 'ProfileCompletedOn' custom attribute.", exception.Message);
    Assert.Equal("user", exception.ParamName);
  }

  [Fact(DisplayName = "GetProfileCompleted: it should return the correct DateTime when the user profile is completed.")]
  public void GetProfileCompleted_it_should_return_the_correct_DateTime_when_the_user_profile_is_completed()
  {
    DateTime profileCompletedOn = DateTime.UtcNow;

    User user = new(_faker.Person.UserName);
    user.CustomAttributes.Add(new("ProfileCompletedOn", profileCompletedOn.ToISOString()));
    Assert.Equal(profileCompletedOn, user.GetProfileCompleted().ToUniversalTime());
  }

  [Theory(DisplayName = "GetSubject: it should return the correct subject claim value.")]
  [InlineData("5de18c2f-ab63-4a48-a1d8-31c0220e745e")]
  public void GetSubject_it_should_return_the_correct_subject_claim_value(string id)
  {
    User user = new(_faker.Person.UserName)
    {
      Id = Guid.Parse(id)
    };
    Assert.Equal(id, user.GetSubject());
  }

  [Fact(DisplayName = "HasCustomAttribute: it should return false when the custom attribute could not be found.")]
  public void HasCustomAttribute_it_should_return_false_when_the_custom_attribute_could_not_be_found()
  {
    User user = new(_faker.Person.Email)
    {
      Id = Guid.NewGuid()
    };
    Assert.False(user.HasCustomAttribute("MultiFactorAuthenticationMode"));
  }

  [Fact(DisplayName = "HasCustomAttribute: it should return true when the custom attribute was be found.")]
  public void HasCustomAttribute_it_should_return_true_when_the_custom_attribute_was_found()
  {
    User user = new(_faker.Person.Email)
    {
      Id = Guid.NewGuid()
    };
    user.CustomAttributes.Add(new("MultiFactorAuthenticationMode", "Email"));
    Assert.True(user.HasCustomAttribute("MultiFactorAuthenticationMode"));
  }

  [Fact(DisplayName = "HasCustomAttribute: it should throw ArgumentException when multiple custom attributes were found.")]
  public void HasCustomAttribute_it_should_throw_ArgumentException_when_multiple_custom_attributes_were_found()
  {
    User user = new(_faker.Person.Email)
    {
      Id = Guid.NewGuid()
    };
    user.CustomAttributes.Add(new("MultiFactorAuthenticationMode", "Email"));
    user.CustomAttributes.Add(new("MultiFactorAuthenticationMode", "Phone"));
    var exception = Assert.Throws<ArgumentException>(() => user.HasCustomAttribute("MultiFactorAuthenticationMode"));
    Assert.StartsWith($"The user 'Id={user.Id}' has 2 'MultiFactorAuthenticationMode' custom attributes.", exception.Message);
    Assert.Equal("user", exception.ParamName);
  }

  [Fact(DisplayName = "IsProfileCompleted: it should return false when the user profile is not completed.")]
  public void IsProfileCompleted_it_should_return_false_when_the_user_profile_is_not_completed()
  {
    User user = new(_faker.Person.UserName);
    Assert.Empty(user.CustomAttributes);
    Assert.False(user.IsProfileCompleted());
  }

  [Fact(DisplayName = "IsProfileCompleted: it should return true when the user profile is completed.")]
  public void IsProfileCompleted_it_should_return_true_when_the_user_profile_is_completed()
  {
    User user = new(_faker.Person.UserName);
    user.CustomAttributes.Add(new("ProfileCompletedOn", DateTime.UtcNow.ToISOString()));
    Assert.True(user.IsProfileCompleted());
  }

  [Theory(DisplayName = "SetMultiFactorAuthenticationMode: it should the correct custom attribute on the payload")]
  [InlineData(MultiFactorAuthenticationMode.Email)]
  public void SetMultiFactorAuthenticationMode_it_should_the_correct_custom_attribute_on_the_payload(MultiFactorAuthenticationMode mfaMode)
  {
    UpdateUserPayload payload = new();
    Assert.Empty(payload.CustomAttributes);

    payload.SetMultiFactorAuthenticationMode(mfaMode);
    Assert.Contains(payload.CustomAttributes, c => c.Key == nameof(MultiFactorAuthenticationMode) && c.Value == mfaMode.ToString());
  }

  [Fact(DisplayName = "ToUpdateUserPayload: it should return the correct payload.")]
  public void ToUpdateUserPayload_it_should_return_the_correct_payload()
  {
    SaveProfilePayload payload = new(_faker.Person.FirstName, _faker.Person.LastName, _faker.Locale, "America/Montreal")
    {
      MiddleName = null,
      Birthdate = _faker.Person.DateOfBirth,
      Gender = _faker.Person.Gender.ToString().ToLower()
    };
    UpdateUserPayload update = payload.ToUpdateUserPayload();
    Assert.Equal(payload.FirstName, update.FirstName?.Value);
    Assert.NotNull(update.MiddleName);
    Assert.Equal(payload.MiddleName, update.MiddleName.Value);
    Assert.Equal(payload.LastName, update.LastName?.Value);
    Assert.Equal(payload.Birthdate, update.Birthdate?.Value);
    Assert.Equal(payload.Gender, update.Gender?.Value);
    Assert.Equal(payload.Locale, update.Locale?.Value);
    Assert.Equal(payload.TimeZone, update.TimeZone?.Value);
  }

  [Fact(DisplayName = "ToUserProfile: it should return the correct user profile.")]
  public void ToUserProfile_it_should_return_the_correct_user_profile()
  {
    DateTime completedOn = DateTime.Now.AddHours(-6);
    User user = new(_faker.Person.UserName)
    {
      CreatedOn = DateTime.Now.AddDays(-1),
      UpdatedOn = DateTime.Now,
      PasswordChangedOn = DateTime.Now.AddMinutes(-10),
      AuthenticatedOn = DateTime.Now.AddHours(-1),
      Email = new Email(_faker.Person.Email),
      Phone = new Phone(countryCode: "CA", "(514) 845-4636", extension: null, "+15148454636"),
      FirstName = _faker.Person.FirstName,
      LastName = _faker.Person.LastName,
      FullName = _faker.Person.FullName,
      Birthdate = _faker.Person.DateOfBirth,
      Gender = _faker.Person.Gender.ToString().ToLower(),
      Locale = new Locale("fr-CA"),
      TimeZone = "America/Montreal"
    };
    user.CustomAttributes.Add(new CustomAttribute("MultiFactorAuthenticationMode", MultiFactorAuthenticationMode.Phone.ToString()));
    user.CustomAttributes.Add(new CustomAttribute("ProfileCompletedOn", completedOn.ToISOString()));
    UserProfile profile = user.ToUserProfile();
    Assert.NotNull(profile.Phone);

    Assert.Equal(user.CreatedOn, profile.CreatedOn);
    Assert.Equal(completedOn, profile.CompletedOn);
    Assert.Equal(user.UpdatedOn, profile.UpdatedOn);
    Assert.Equal(user.PasswordChangedOn, profile.PasswordChangedOn);
    Assert.Equal(user.AuthenticatedOn, profile.AuthenticatedOn);
    Assert.Equal(MultiFactorAuthenticationMode.Phone, profile.MultiFactorAuthenticationMode);
    Assert.Equal(user.Email.Address, profile.EmailAddress);
    Assert.Equal(user.Phone.CountryCode, profile.Phone.CountryCode);
    Assert.Equal(user.Phone.Number, profile.Phone.Number);
    Assert.Equal(user.FirstName, profile.FirstName);
    Assert.Equal(user.LastName, profile.LastName);
    Assert.Equal(user.FullName, profile.FullName);
    Assert.Equal(user.Birthdate, profile.Birthdate);
    Assert.Equal(user.Gender, profile.Gender);
    Assert.Equal(user.Locale, profile.Locale);
    Assert.Equal(user.TimeZone, profile.TimeZone);
  }

  [Fact(DisplayName = "TryGetCustomAttribute: it return null when a custom attribute is not found.")]
  public void TryGetCustomAttribute_it_should_throw_ArgumentException_when_a_custom_attribute_is_not_found()
  {
    User user = new(_faker.Person.Email)
    {
      Id = Guid.NewGuid()
    };
    Assert.Null(user.TryGetCustomAttribute("MultiFactorAuthenticationMode"));
  }

  [Fact(DisplayName = "TryGetCustomAttribute: it should return the found value.")]
  public void TryGetCustomAttribute_it_should_return_the_found_value()
  {
    string mfaMode = MultiFactorAuthenticationMode.Email.ToString();
    User user = new(_faker.Person.Email)
    {
      Id = Guid.NewGuid()
    };
    user.CustomAttributes.Add(new("MultiFactorAuthenticationMode", mfaMode));
    Assert.Equal(mfaMode, user.TryGetCustomAttribute("MultiFactorAuthenticationMode"));
  }

  [Fact(DisplayName = "TryGetCustomAttribute: it should throw ArgumentException when multiple custom attributes were found.")]
  public void TryGetCustomAttribute_it_should_throw_ArgumentException_when_multiple_custom_attributes_were_found()
  {
    User user = new(_faker.Person.Email)
    {
      Id = Guid.NewGuid()
    };
    user.CustomAttributes.Add(new("MultiFactorAuthenticationMode", "Email"));
    user.CustomAttributes.Add(new("MultiFactorAuthenticationMode", "Phone"));
    var exception = Assert.Throws<ArgumentException>(() => user.TryGetCustomAttribute("MultiFactorAuthenticationMode"));
    Assert.StartsWith($"The user 'Id={user.Id}' has 2 'MultiFactorAuthenticationMode' custom attributes.", exception.Message);
    Assert.Equal("user", exception.ParamName);
  }
}
