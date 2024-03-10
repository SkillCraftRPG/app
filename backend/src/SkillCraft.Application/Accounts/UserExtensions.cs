using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Application.Accounts;

public static class UserExtensions
{
  private const string MultiFactorAuthenticationModeKey = nameof(MultiFactorAuthenticationMode);
  private const string ProfileCompletedOnKey = "ProfileCompletedOn";

  public static MultiFactorAuthenticationMode? GetMultiFactorAuthenticationMode(this User user)
  {
    CustomAttribute? customAttribute = user.CustomAttributes.SingleOrDefault(x => x.Key == MultiFactorAuthenticationModeKey);
    return customAttribute == null ? null : Enum.Parse<MultiFactorAuthenticationMode>(customAttribute.Value);
  }
  public static void SetMultiFactorAuthenticationMode(this UpdateUserPayload payload, MultiFactorAuthenticationMode mode)
  {
    payload.CustomAttributes.Add(new CustomAttributeModification(MultiFactorAuthenticationModeKey, mode.ToString()));
  }

  public static string GetSubject(this User user) => user.Id.ToString();

  public static void CompleteProfile(this UpdateUserPayload payload)
  {
    string completedOn = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture);
    payload.CustomAttributes.Add(new CustomAttributeModification(ProfileCompletedOnKey, completedOn));
  }
  public static bool IsProfileCompleted(this User user)
  {
    return user.GetProfileCompleted().HasValue;
  }
  public static DateTime? GetProfileCompleted(this User user)
  {
    CustomAttribute? customAttribute = user.CustomAttributes.SingleOrDefault(x => x.Key == ProfileCompletedOnKey);
    return customAttribute == null ? null : DateTime.Parse(customAttribute.Value);
  }

  public static PhonePayload ToPhonePayload(this Contracts.Accounts.Phone phone)
  {
    return new PhonePayload(phone.CountryCode, phone.Number, extension: null, isVerified: true);
  }

  public static UserProfile ToUserProfile(this User user)
  {
    string emailAddress = user.Email?.Address ?? throw new ArgumentException($"The {nameof(user.Email)} is required.", nameof(user));
    string firstName = user.FirstName ?? throw new ArgumentException($"The {nameof(user.FirstName)} is required.", nameof(user));
    string lastName = user.LastName ?? throw new ArgumentException($"The {nameof(user.LastName)} is required.", nameof(user));
    Locale locale = user.Locale ?? throw new ArgumentException($"The {nameof(user.Locale)} is required.", nameof(user));
    string timeZone = user.TimeZone ?? throw new ArgumentException($"The {nameof(user.TimeZone)} is required.", nameof(user));

    UserProfile profile = new(emailAddress, firstName, lastName, locale, timeZone)
    {
      RegisteredOn = user.CreatedOn,
      PasswordChangedOn = user.PasswordChangedOn,
      Birthdate = user.Birthdate,
      Gender = user.Gender
    };
    if (user.Phone != null)
    {
      profile.Phone = new Contracts.Accounts.Phone(user.Phone.CountryCode, user.Phone.Number);
    }

    DateTime? profileCompletedOn = user.GetProfileCompleted();
    if (profileCompletedOn.HasValue)
    {
      profile.CompletedOn = profileCompletedOn.Value.ToUniversalTime();
    }

    MultiFactorAuthenticationMode? mfaMode = user.GetMultiFactorAuthenticationMode();
    if (mfaMode.HasValue)
    {
      profile.MultiFactorAuthenticationMode = mfaMode.Value;
    }

    return profile;
  }
}
