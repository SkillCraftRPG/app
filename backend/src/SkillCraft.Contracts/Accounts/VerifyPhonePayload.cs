namespace SkillCraft.Contracts.Accounts;

public record VerifyPhonePayload
{
  public string Locale { get; set; }
  public string ProfileCompletionToken { get; set; }

  public AccountPhone? Phone { get; set; }
  public OneTimePasswordPayload? OneTimePassword { get; set; }

  public VerifyPhonePayload() : this(string.Empty, string.Empty)
  {
  }

  public VerifyPhonePayload(string locale, string profileCompletionToken)
  {
    Locale = locale;
    ProfileCompletionToken = profileCompletionToken;
  }

  public VerifyPhonePayload(string locale, string profileCompletionToken, AccountPhone? phone) : this(locale, profileCompletionToken)
  {
    Phone = phone;
  }

  public VerifyPhonePayload(string locale, string profileCompletionToken, OneTimePasswordPayload? oneTimePassword) : this(locale, profileCompletionToken)
  {
    OneTimePassword = oneTimePassword;
  }
}
