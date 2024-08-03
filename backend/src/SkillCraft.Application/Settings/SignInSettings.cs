namespace SkillCraft.Application.Settings;

public record SignInSettings
{
  public const string SectionKey = "SignIn";

  public string DefaultTimeZone { get; set; }

  public SignInSettings() : this(string.Empty)
  {
  }

  public SignInSettings(string defaultTimeZone)
  {
    DefaultTimeZone = defaultTimeZone;
  }
}
