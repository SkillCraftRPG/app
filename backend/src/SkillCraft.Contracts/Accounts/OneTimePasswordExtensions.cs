using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Passwords;

namespace SkillCraft.Contracts.Accounts;

public static class OneTimePasswordExtensions
{
  private const string PurposeKey = "Purpose";
  private const string UserIdKey = "UserId";

  public static Guid GetUserId(this OneTimePassword oneTimePassword)
  {
    CustomAttribute customAttribute = oneTimePassword.CustomAttributes.SingleOrDefault(x => x.Key == UserIdKey)
      ?? throw new InvalidOperationException($"The One-Time Password (OTP) has no '{UserIdKey}' custom attribute.");
    return Guid.Parse(customAttribute.Value);
  }

  public static void EnsurePurpose(this OneTimePassword oneTimePassword, string purpose)
  {
    if (!oneTimePassword.HasPurpose(purpose))
    {
      throw new InvalidOneTimePasswordPurpose(oneTimePassword, purpose);
    }
  }
  public static bool HasPurpose(this OneTimePassword oneTimePassword, string purpose)
  {
    return oneTimePassword.GetPurpose().Equals(purpose, StringComparison.OrdinalIgnoreCase);
  }
  public static string GetPurpose(this OneTimePassword oneTimePassword)
  {
    string? purpose = oneTimePassword.TryGetPurpose();
    return purpose ?? throw new InvalidOperationException($"The One-Time Password (OTP) has no '{PurposeKey}' custom attribute.");
  }
  public static string? TryGetPurpose(this OneTimePassword oneTimePassword)
  {
    CustomAttribute? customAttribute = oneTimePassword.CustomAttributes.SingleOrDefault(x => x.Key == PurposeKey);
    return customAttribute?.Value;
  }
}
