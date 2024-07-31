namespace SkillCraft.Contracts.Accounts;

public record ResetPasswordPayload
{
  public string Locale { get; set; }

  public string? EmailAddress { get; set; }

  public string? Token { get; set; } // TODO(fpion): not nullable, same object as Password
  public string? Password { get; set; } // TODO(fpion): not nullable, same object as Token

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

  public ResetPasswordPayload(string locale, string token, string password) : this(locale)
  {
    Token = token;
    Password = password;
  }
}
