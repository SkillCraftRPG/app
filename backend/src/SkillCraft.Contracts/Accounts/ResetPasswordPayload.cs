namespace SkillCraft.Contracts.Accounts;

public record ResetPasswordPayload
{
  public string Locale { get; set; }

  public string? EmailAddress { get; set; }

  public string? Token { get; set; }
  public string? Password { get; set; }

  public ResetPasswordPayload(string locale)
  {
    Locale = locale;
  }
}
