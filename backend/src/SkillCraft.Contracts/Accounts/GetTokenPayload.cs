namespace SkillCraft.Contracts.Accounts;

public record GetTokenPayload : SignInAccountPayload
{
  [JsonPropertyName("refresh_token")]
  public string? RefreshToken { get; set; }
}
