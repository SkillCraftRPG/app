using Logitar.Portal.Contracts.Tokens;

namespace SkillCraft.Contracts.Accounts;

public record VerifyPhoneResult
{
  public OneTimePasswordValidation? OneTimePasswordValidation { get; set; }
  public string? ProfileCompletionToken { get; set; }

  public VerifyPhoneResult()
  {
  }

  public VerifyPhoneResult(OneTimePasswordValidation oneTimePasswordValidation) : this()
  {
    OneTimePasswordValidation = oneTimePasswordValidation;
  }

  public VerifyPhoneResult(CreatedToken profileCompletion) : this(profileCompletion.Token)
  {
  }

  public VerifyPhoneResult(string profileCompletionToken) : this()
  {
    ProfileCompletionToken = profileCompletionToken;
  }
}
