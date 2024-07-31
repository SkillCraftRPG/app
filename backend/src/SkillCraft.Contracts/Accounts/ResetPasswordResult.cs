using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Tokens;

namespace SkillCraft.Contracts.Accounts;

public record ResetPasswordResult
{
  public SentMessage? RecoveryLinkSentTo { get; set; }
  public string? ProfileCompletionToken { get; set; }
  public Session? Session { get; set; }

  public ResetPasswordResult()
  {
  }

  public static ResetPasswordResult RecoveryLinkSent(SentMessage to) => new()
  {
    RecoveryLinkSentTo = to
  };

  public static ResetPasswordResult RequireProfileCompletion(CreatedToken profileCompletion) => new()
  {
    ProfileCompletionToken = profileCompletion.Token
  };

  public static ResetPasswordResult Success(Session session) => new()
  {
    Session = session
  };
}
