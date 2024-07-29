namespace SkillCraft.Settings;

public record OpenAuthenticationSettings
{
  public const string SectionKey = "OpenAuthentication";

  public AccessTokenSettings AccessToken { get; set; } = new();
}
