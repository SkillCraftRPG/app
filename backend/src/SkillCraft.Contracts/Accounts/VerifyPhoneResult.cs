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

  public VerifyPhoneResult(string profileCompletionToken) : this()
  {
    ProfileCompletionToken = profileCompletionToken;
  }
}
