using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Models.Account;

public record SignInResponse<T>
{
  public SentMessage? AuthenticationLinkSentTo { get; private set; }
  public bool IsPasswordRequired { get; private set; }
  public OneTimePasswordValidation? OneTimePasswordValidation { get; private set; }
  public string? ProfileCompletionToken { get; private set; }
  public T? Success { get; private set; }

  public SignInResponse(SignInResult result, T? success)
  {
    AuthenticationLinkSentTo = result.AuthenticationLinkSentTo;
    IsPasswordRequired = result.IsPasswordRequired;
    OneTimePasswordValidation = result.OneTimePasswordValidation;
    ProfileCompletionToken = result.ProfileCompletionToken;
    Success = success;
  }
}
