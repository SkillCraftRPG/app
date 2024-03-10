using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Models.Account;

public record SignInResponse<T>
{
  public SentMessage? AuthenticationLinkSentTo { get; }
  public bool IsPasswordRequired { get; }
  public OneTimePasswordValidation? OneTimePasswordValidation { get; }
  public string? ProfileCompletionToken { get; }
  public T? Success { get; }

  public SignInResponse(SignInResult? result, T? success)
  {
    if (result != null)
    {
      AuthenticationLinkSentTo = result.AuthenticationLinkSentTo;
      IsPasswordRequired = result.IsPasswordRequired;
      OneTimePasswordValidation = result.OneTimePasswordValidation;
      ProfileCompletionToken = result.ProfileCompletionToken;
    }

    Success = success;
  }
}
