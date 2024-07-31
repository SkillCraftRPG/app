namespace SkillCraft.Contracts.Accounts;

public record ChangePhoneResult
{
  public OneTimePasswordValidation? OneTimePasswordValidation { get; set; }
  public UserProfile? UserProfile { get; set; }

  public ChangePhoneResult()
  {
  }

  public ChangePhoneResult(OneTimePasswordValidation oneTimePasswordValidation) : this()
  {
    OneTimePasswordValidation = oneTimePasswordValidation;
  }

  public ChangePhoneResult(UserProfile userProfile) : this()
  {
    UserProfile = userProfile;
  }
}
