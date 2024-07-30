namespace SkillCraft.Settings;

public record AccessTokenSettings
{
  public string Secret { get; set; }
  public string TokenType { get; set; }
  public int LifetimeSeconds { get; set; }

  public AccessTokenSettings() : this(string.Empty, string.Empty)
  {
  }

  public AccessTokenSettings(string secret, string tokenType, int lifetimeSeconds = 0)
  {
    Secret = secret;
    TokenType = tokenType;
    LifetimeSeconds = lifetimeSeconds;
  }
}
