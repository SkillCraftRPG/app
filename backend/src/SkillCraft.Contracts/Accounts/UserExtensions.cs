using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;

namespace SkillCraft.Contracts.Accounts;

public static class UserExtensions
{
  private const string MultiFactorAuthenticationModeKey = nameof(MultiFactorAuthenticationMode);
  private const string ProfileCompletedOnKey = "ProfileCompletedOn";

  public static MultiFactorAuthenticationMode? GetMultiFactorAuthenticationMode(this User user)
  {
    CustomAttribute? customAttribute = user.CustomAttributes.SingleOrDefault(x => x.Key == MultiFactorAuthenticationModeKey);
    return customAttribute == null ? null : Enum.Parse<MultiFactorAuthenticationMode>(customAttribute.Value);
  }

  public static string GetSubject(this User user) => user.Id.ToString();

  public static bool IsProfileCompleted(this User user)
  {
    return user.CustomAttributes.Any(x => x.Key == ProfileCompletedOnKey);
  }

  public static string Mask(this Email email)
  {
    throw new NotImplementedException(); // TODO(fpion): implement
  }
}
