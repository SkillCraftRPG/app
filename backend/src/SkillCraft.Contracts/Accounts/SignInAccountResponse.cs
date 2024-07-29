namespace SkillCraft.Contracts.Accounts;

public record SignInAccountResponse
{
  public SentMessage? AuthenticationLinkSentTo { get; set; }
  public bool IsPasswordRequired { get; set; }
  public OneTimePasswordValidation? OneTimePasswordValidation { get; set; }
  public string? ProfileCompletionToken { get; set; }
  public CurrentUser? CurrentUser { get; set; }

  public SignInAccountResponse()
  {
  }

  public SignInAccountResponse(SignInAccountResult result)
  {
    AuthenticationLinkSentTo = result.AuthenticationLinkSentTo;
    IsPasswordRequired = result.IsPasswordRequired;
    OneTimePasswordValidation = result.OneTimePasswordValidation;
    ProfileCompletionToken = result.ProfileCompletionToken;

    if (result.Session != null)
    {
      CurrentUser = new(result.Session);
    }
  }
}
