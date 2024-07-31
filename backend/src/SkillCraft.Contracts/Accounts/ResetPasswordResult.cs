using Logitar.Portal.Contracts.Sessions;

namespace SkillCraft.Contracts.Accounts;

public record ResetPasswordResult
{
  public SentMessage? RecoveryLinkSentTo { get; set; }
  public Session? Session { get; set; }

  public ResetPasswordResult()
  {
  }

  public static ResetPasswordResult RecoveryLinkSent(SentMessage to) => new()
  {
    RecoveryLinkSentTo = to
  };

  public static ResetPasswordResult Succeed(Session session) => new()
  {
    Session = session
  };
}
