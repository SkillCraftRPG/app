namespace SkillCraft.Contracts.Accounts;

public record ResetPasswordResponse
{
  public SentMessage? RecoveryLinkSentTo { get; set; }
  public CurrentUser? CurrentUser { get; set; }

  public ResetPasswordResponse()
  {
  }

  public ResetPasswordResponse(ResetPasswordResult result)
  {
    RecoveryLinkSentTo = result.RecoveryLinkSentTo;

    if (result.Session != null)
    {
      CurrentUser = new(result.Session);
    }
  }
}
