using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Application.Accounts;

public static class UserExtensions
{
  private const string MultiFactorAuthenticationModeKey = nameof(MultiFactorAuthenticationMode);
  private const string ProfileCompletedOnKey = "ProfileCompletedOn";

  internal static string GetCustomAttribute(this User user, string key)
  {
    return user.TryGetCustomAttribute(key) ?? throw new ArgumentException($"The user 'Id={user.Id}' has no custom attribute '{key}'.", nameof(user));
  }
  internal static bool HasCustomAttribute(this User user, string key) => user.TryGetCustomAttribute(key) != null;
  internal static string? TryGetCustomAttribute(this User user, string key)
  {
    key = key.Trim();

    List<CustomAttribute> customAttributes = new(capacity: user.CustomAttributes.Count);
    foreach (CustomAttribute customAttribute in user.CustomAttributes)
    {
      if (customAttribute.Key.Trim().Equals(key, StringComparison.InvariantCultureIgnoreCase))
      {
        customAttributes.Add(customAttribute);
      }
    }

    if (customAttributes.Count == 0)
    {
      return null;
    }
    else if (customAttributes.Count > 1)
    {
      throw new ArgumentException($"The user 'Id={user.Id}' has {customAttributes.Count} custom attributes '{key}'.", nameof(user));
    }

    return customAttributes.Single().Value;
  }

  public static MultiFactorAuthenticationMode? GetMultiFactorAuthenticationMode(this User user)
  {
    MultiFactorAuthenticationMode mode;

    string? value = user.TryGetCustomAttribute(MultiFactorAuthenticationModeKey);
    if (value == null)
    {
      return null;
    }
    else if (!Enum.TryParse(value, out mode) || !Enum.IsDefined(mode))
    {
      throw new ArgumentException($"The value '{value}' is not valid for custom attribute '{MultiFactorAuthenticationModeKey}' (UserId={user.Id}).");
    }

    return mode;
  }
  public static bool IsProfileCompleted(this User user) => user.HasCustomAttribute(ProfileCompletedOnKey);
}
