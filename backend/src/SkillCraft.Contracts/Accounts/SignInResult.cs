using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Tokens;

namespace SkillCraft.Contracts.Accounts;

public record SignInResult
{
  public SentMessage? AuthenticationLinkSentTo { get; set; }
  public bool IsPasswordRequired { get; set; }
  public OneTimePasswordValidation? OneTimePasswordValidation { get; set; }
  public string? ProfileCompletionToken { get; set; }
  public Session? Session { get; set; }

  public SignInResult()
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
