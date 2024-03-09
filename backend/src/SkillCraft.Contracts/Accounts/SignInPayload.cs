namespace SkillCraft.Contracts.Accounts;

public record SignInPayload
{
  public string Locale { get; set; }

  public CredentialsPayload? Credentials { get; set; }
  public string? Token { get; set; }
  public OneTimePasswordPayload? OneTimePassword { get; set; }
  public ProfilePayload? Profile { get; set; }

  public SignInPayload() : this(string.Empty)
  {
  }

  public SignInPayload(string locale)
  {
    Locale = locale;
  }
}
