namespace SkillCraft.Settings;

public record AccessTokenSettings
{
  public string TokenType { get; set; }
  public int LifetimeSeconds { get; set; }

  public AccessTokenSettings() : this(string.Empty)
  {
  }

  public AccessTokenSettings(string tokenType, int lifetimeSeconds = 0)
  {
    TokenType = tokenType;
    LifetimeSeconds = lifetimeSeconds;
  }
}
