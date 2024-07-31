namespace SkillCraft.Contracts.Accounts;

public record ChangePhonePayload
{
  public string Locale { get; set; }

  public AccountPhone? Phone { get; set; }
  public OneTimePasswordPayload? OneTimePassword { get; set; }

  public ChangePhonePayload() : this(string.Empty)
  {
  }

  public ChangePhonePayload(string locale)
  {
    Locale = locale;
  }

  public ChangePhonePayload(string locale, AccountPhone? phone) : this(locale)
  {
    Phone = phone;
  }

  public ChangePhonePayload(string locale, OneTimePasswordPayload? oneTimePassword) : this(locale)
  {
    OneTimePassword = oneTimePassword;
  }
}
