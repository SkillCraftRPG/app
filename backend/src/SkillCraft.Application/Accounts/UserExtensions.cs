using Logitar;
using Logitar.Identity.Contracts;
using Logitar.Identity.Domain.Users;
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
    return user.TryGetCustomAttribute(key) ?? throw new ArgumentException($"The user 'Id={user.Id}' has no '{key}' custom attribute.", nameof(user));
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
      throw new ArgumentException($"The user 'Id={user.Id}' has {customAttributes.Count} '{key}' custom attributes.", nameof(user));
    }

    return customAttributes.Single().Value;
  }

  public static bool IsEqualTo(this Email email, EmailPayload other)
  {
    return email.Address == other.Address && email.IsVerified == other.IsVerified;
  }
  public static bool IsEqualTo(this Phone phone, PhonePayload other)
  {
    return phone.CountryCode == other.CountryCode && phone.Number == other.Number
      && phone.Extension == other.Extension && phone.IsVerified == other.IsVerified;
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
  public static void SetMultiFactorAuthenticationMode(this UpdateUserPayload payload, MultiFactorAuthenticationMode mode)
  {
    payload.CustomAttributes.Add(new CustomAttributeModification(MultiFactorAuthenticationModeKey, mode.ToString()));
  }

  public static void CompleteProfile(this UpdateUserPayload payload)
  {
    string completedOn = DateTime.UtcNow.ToISOString();
    payload.CustomAttributes.Add(new CustomAttributeModification(ProfileCompletedOnKey, completedOn));
  }
  public static DateTime GetProfileCompleted(this User user) => DateTime.Parse(user.GetCustomAttribute(ProfileCompletedOnKey)).AsUniversalTime();
  public static bool IsProfileCompleted(this User user) => user.HasCustomAttribute(ProfileCompletedOnKey);

  public static string GetSubject(this User user) => user.Id.ToString();

  public static Phone ToPhone(this AccountPhone source)
  {
    Phone phone = new(source.CountryCode, source.Number, extension: null, e164Formatted: string.Empty);
    phone.E164Formatted = phone.FormatToE164();
    return phone;
  }

  public static UpdateUserPayload ToUpdateUserPayload(this SaveProfilePayload payload) => new()
  {
    FirstName = new Modification<string>(payload.FirstName),
    MiddleName = new Modification<string>(payload.MiddleName),
    LastName = new Modification<string>(payload.LastName),
    Birthdate = new Modification<DateTime?>(payload.Birthdate),
    Gender = new Modification<string>(payload.Gender),
    Locale = new Modification<string>(payload.Locale),
    TimeZone = new Modification<string>(payload.TimeZone)
  };

  public static UserProfile ToUserProfile(this User user) => new()
  {
    CreatedOn = user.CreatedOn,
    CompletedOn = user.GetProfileCompleted(),
    UpdatedOn = user.UpdatedOn,
    PasswordChangedOn = user.PasswordChangedOn,
    AuthenticatedOn = user.AuthenticatedOn,
    MultiFactorAuthenticationMode = user.GetMultiFactorAuthenticationMode() ?? MultiFactorAuthenticationMode.None,
    EmailAddress = user.Email?.Address ?? user.UniqueName,
    Phone = AccountPhone.TryCreate(user.Phone),
    FirstName = user.FirstName ?? string.Empty,
    MiddleName = user.MiddleName,
    LastName = user.LastName ?? string.Empty,
    FullName = user.FullName ?? string.Empty,
    Birthdate = user.Birthdate,
    Gender = user.Gender,
    Locale = user.Locale ?? new Locale(),
    TimeZone = user.TimeZone ?? string.Empty
  };
}
