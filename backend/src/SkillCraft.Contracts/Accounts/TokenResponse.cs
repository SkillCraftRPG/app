namespace SkillCraft.Contracts.Accounts;

public record TokenResponse
{
  [JsonPropertyName("access_token")]
  public string AccessToken { get; set; }

  [JsonPropertyName("token_type")]
  public string TokenType { get; set; }

  [JsonPropertyName("expires_in")]
  public int? ExpiresIn { get; set; }

  [JsonPropertyName("refresh_token")]
  public string? RefreshToken { get; set; }

  [JsonPropertyName("scope")]
  public string? Scope { get; set; }

  public TokenResponse() : this(string.Empty, string.Empty)
  {
  }

  public TokenResponse(string accessToken, string tokenType, int? expiresIn = null, string? refreshToken = null, string? scope = null)
  {
    AccessToken = accessToken;
    TokenType = tokenType;
    ExpiresIn = expiresIn;
    RefreshToken = refreshToken;
    Scope = scope;
  }
}
