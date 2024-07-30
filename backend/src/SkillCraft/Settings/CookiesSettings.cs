namespace SkillCraft.Settings;

internal record CookiesSettings
{
  public const string SectionKey = "Cookies";

  public RefreshTokenCookieSettings RefreshToken { get; set; } = new();
  public SessionCookieSettings Session { get; set; } = new();
}
