namespace SkillCraft.Application.Settings;

public record AccountSettings
{
  public const string SectionKey = "Account";

  public long AllocatedBytes { get; set; }
  public string DefaultTimeZone { get; set; }
  public int WorldLimit { get; set; }

  public AccountSettings() : this(string.Empty)
  {
  }

  public AccountSettings(string defaultTimeZone)
  {
    DefaultTimeZone = defaultTimeZone;
  }
}
