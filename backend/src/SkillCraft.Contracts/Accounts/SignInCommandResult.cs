using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Tokens;

namespace SkillCraft.Contracts.Accounts;

public record SignInCommandResult
{
  public SentMessage? AuthenticationLinkSentTo { get; set; }
  public bool IsPasswordRequired { get; set; }
  public OneTimePasswordValidation? OneTimePasswordValidation { get; set; }
  public string? ProfileCompletionToken { get; set; }
  public Session? Session { get; set; }

  public static SignInCommandResult AuthenticationLinkSent(SentMessage to) => new()
  {
    AuthenticationLinkSentTo = to
  };

  public static SignInCommandResult RequirePassword() => new()
  {
    IsPasswordRequired = true
  };

  public static SignInCommandResult RequireOneTimePasswordValidation(OneTimePassword oneTimePassword, SentMessage sentMessage) => new()
  {
    OneTimePasswordValidation = new OneTimePasswordValidation(oneTimePassword.Id, sentMessage)
  };

  public static SignInCommandResult RequireProfileCompletion(CreatedToken createdToken) => new()
  {
    ProfileCompletionToken = createdToken.Token
  };

  public static SignInCommandResult Success(Session session) => new()
  {
    Session = session
  };
}
