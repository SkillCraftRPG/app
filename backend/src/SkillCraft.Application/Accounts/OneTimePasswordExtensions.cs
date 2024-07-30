using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Users;

namespace SkillCraft.Application.Accounts;

public static class OneTimePasswordExtensions
{
  private const string PhoneCountryCodeKey = "PhoneCountryCode";
  private const string PhoneNumberKey = "PhoneNumber";
  private const string PhoneE164FormattedKey = "PhoneE164Formatted";
  private const string PurposeKey = "Purpose";
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

  public static void SetPhone(this CreateOneTimePasswordPayload payload, Phone phone)
  {
    if (phone.CountryCode != null)
    {
      payload.CustomAttributes.Add(new CustomAttribute(PhoneCountryCodeKey, phone.CountryCode));
    }
    payload.CustomAttributes.Add(new CustomAttribute(PhoneNumberKey, phone.Number));
    payload.CustomAttributes.Add(new CustomAttribute(PhoneE164FormattedKey, phone.E164Formatted));
  }

  public static void EnsurePurpose(this OneTimePassword oneTimePassword, string purpose)
  {
    if (!oneTimePassword.HasPurpose(purpose))
    {
      throw new InvalidOneTimePasswordPurposeException(oneTimePassword, purpose);
    }
  }
  public static string GetPurpose(this OneTimePassword oneTimePassword) => oneTimePassword.GetCustomAttribute(PurposeKey);
  public static bool HasPurpose(this OneTimePassword oneTimePassword, string purpose)
    => oneTimePassword.TryGetPurpose()?.Equals(purpose, StringComparison.InvariantCultureIgnoreCase) == true;
  public static string? TryGetPurpose(this OneTimePassword oneTimePassword) => oneTimePassword.TryGetCustomAttribute(PurposeKey);
  public static void SetPurpose(this CreateOneTimePasswordPayload payload, string purpose)
  {
    payload.CustomAttributes.Add(new CustomAttribute(PurposeKey, purpose));
  }

  public static Guid GetUserId(this OneTimePassword oneTimePassword) => Guid.Parse(oneTimePassword.GetCustomAttribute(UserIdKey));
  public static void SetUserId(this CreateOneTimePasswordPayload payload, User user)
  {
    payload.CustomAttributes.Add(new CustomAttribute(UserIdKey, user.Id.ToString()));
  }
}
