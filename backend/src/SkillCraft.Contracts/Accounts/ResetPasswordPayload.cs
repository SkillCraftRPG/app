namespace SkillCraft.Contracts.Accounts;

public record ResetPasswordPayload
{
  public string Locale { get; set; }

  public string? EmailAddress { get; set; }

  public ResetPasswordPayload() : this(string.Empty)
  {
  }

  public ResetPasswordPayload(string locale)
  {
    Locale = locale;
  }

  public ResetPasswordPayload(string locale, string emailAddress) : this(locale)
  {
    EmailAddress = emailAddress;
  }
}
