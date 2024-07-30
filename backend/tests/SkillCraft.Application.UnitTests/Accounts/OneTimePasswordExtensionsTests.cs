using Logitar.Portal.Contracts.Passwords;

namespace SkillCraft.Application.Accounts;

[Trait(Traits.Category, Categories.Unit)]
public class OneTimePasswordExtensionsTests
{
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
    Assert.StartsWith($"The One-Time Password 'Id={oneTimePassword.Id}' has no custom attribute 'UserId'.", exception.Message);
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
    Assert.StartsWith($"The One-Time Password 'Id={oneTimePassword.Id}' has 2 custom attributes 'UserId'.", exception.Message);
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
    Assert.StartsWith($"The One-Time Password 'Id={oneTimePassword.Id}' has 2 custom attributes 'UserId'.", exception.Message);
    Assert.Equal("oneTimePassword", exception.ParamName);
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
    Assert.StartsWith($"The One-Time Password 'Id={oneTimePassword.Id}' has 2 custom attributes 'UserId'.", exception.Message);
    Assert.Equal("oneTimePassword", exception.ParamName);
  }
}

// TODO(fpion): EnsurePurpose
// TODO(fpion): GetPurpose
// TODO(fpion): HasPurpose
// TODO(fpion): SetPurpose
// TODO(fpion): SetUserId
// TODO(fpion): TryGetPurpose
