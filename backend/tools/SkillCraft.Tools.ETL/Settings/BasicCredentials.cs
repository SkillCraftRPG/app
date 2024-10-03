using Logitar.Net;

namespace SkillCraft.Tools.ETL.Settings;

internal record BasicCredentials
{
  public string Username { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;

  public Credentials GetCredentials() => new(Username, Password);
}
