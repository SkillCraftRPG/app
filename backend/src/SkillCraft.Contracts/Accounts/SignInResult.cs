using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Tokens;

namespace SkillCraft.Contracts.Accounts;

public record SignInResult
{
  public bool IsPasswordRequired { get; private set; }
  public SentMessage? MultiFactorAuthentication { get; private set; }
  public string? ProfileCompletionToken { get; private set; }
  public SentMessage? SentMessage { get; private set; }
  public Session? Session { get; private set; }

  private SignInResult()
  {
  }

  public SignInResult(SentMessage sentMessage) : this()
  {
    SentMessage = sentMessage;
  }

  public SignInResult(Session session) : this()
  {
    Session = session;
  }

  public static SignInResult RequireMultiFactorAuthentication(SentMessage sentMessage) => new()
  {
    MultiFactorAuthentication = sentMessage
  };

  public static SignInResult RequirePassword() => new()
  {
    IsPasswordRequired = true
  };

  public static SignInResult RequireProfileCompletion(CreatedToken createdToken) => new()
  {
    ProfileCompletionToken = createdToken.Token
  };
}
