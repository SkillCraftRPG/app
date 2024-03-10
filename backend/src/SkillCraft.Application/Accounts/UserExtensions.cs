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
    return user.CustomAttributes.Any(x => x.Key == ProfileCompletedOnKey);
  }
}
