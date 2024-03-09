using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Tokens;

namespace SkillCraft.Contracts.Accounts;

public record SignInResult
{
  public SentMessage? AuthenticationLinkSentTo { get; private set; }
  public bool IsPasswordRequired { get; private set; }
  public OneTimePasswordValidation? OneTimePasswordValidation { get; private set; }
  public string? ProfileCompletionToken { get; private set; }
  public Session? Session { get; private set; }

  private SignInResult()
  {
  }

  public static SignInResult AuthenticationLinkSent(SentMessage sentMessage) => new()
  {
    AuthenticationLinkSentTo = sentMessage
  };

  public static SignInResult RequireOneTimePasswordValidation(OneTimePassword oneTimePassword, SentMessage sentMessage) => new()
  {
    OneTimePasswordValidation = new OneTimePasswordValidation(oneTimePassword, sentMessage)
  };

  public static SignInResult RequirePassword() => new()
  {
    IsPasswordRequired = true
  };

  public static SignInResult RequireProfileCompletion(CreatedToken createdToken) => new()
  {
    ProfileCompletionToken = createdToken.Token
  };

  public static SignInResult Succeed(Session session) => new()
  {
    Session = session
  };
}
