using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Passwords;

namespace SkillCraft.Application.Accounts;

internal static class OneTimePasswordExtensions
{
  private const string UserIdKey = "UserId";

  internal static string GetCustomAttribute(this OneTimePassword oneTimePassword, string key)
  {
    return oneTimePassword.TryGetCustomAttribute(key) ?? throw new ArgumentException($"The One-Time Password 'Id={oneTimePassword.Id}' has no custom attribute '{key}'.", nameof(oneTimePassword));
  }
  internal static bool HasCustomAttribute(this OneTimePassword oneTimePassword, string key) => oneTimePassword.TryGetCustomAttribute(key) != null;
  internal static string? TryGetCustomAttribute(this OneTimePassword oneTimePassword, string key)
  {
    key = key.Trim();

    List<CustomAttribute> customAttributes = new(capacity: oneTimePassword.CustomAttributes.Count);
    foreach (CustomAttribute customAttribute in oneTimePassword.CustomAttributes)
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
      throw new ArgumentException($"The One-Time Password 'Id={oneTimePassword.Id}' has {customAttributes.Count} custom attributes '{key}'.", nameof(oneTimePassword));
    }

    return customAttributes.Single().Value;
  }

  public static Guid GetUserId(this OneTimePassword oneTimePassword) => Guid.Parse(oneTimePassword.GetCustomAttribute(UserIdKey));
}
