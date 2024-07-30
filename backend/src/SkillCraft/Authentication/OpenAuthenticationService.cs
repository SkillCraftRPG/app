using Logitar.Portal.Contracts.Sessions;
using Microsoft.IdentityModel.Tokens;
using SkillCraft.Constants;
using SkillCraft.Contracts.Accounts;
using SkillCraft.Extensions;
using SkillCraft.Settings;

namespace SkillCraft.Authentication;

internal class OpenAuthenticationService : IOpenAuthenticationService
{
  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly OpenAuthenticationSettings _settings;
  private readonly JwtSecurityTokenHandler _tokenHandler = new();

  public OpenAuthenticationService(IHttpContextAccessor httpContextAccessor, OpenAuthenticationSettings settings)
  {
    _httpContextAccessor = httpContextAccessor;
    _settings = settings;

    _tokenHandler.InboundClaimTypeMap.Clear();
  }

  public TokenResponse GetTokenResponse(Session session)
  {
    string baseUrl = GetBaseUrl();

    SecurityTokenDescriptor tokenDescriptor = new()
    {
      Audience = baseUrl,
      Expires = DateTime.UtcNow.AddSeconds(_settings.AccessToken.LifetimeSeconds),
      Issuer = baseUrl,
      SigningCredentials = new SigningCredentials(GetSecurityKey(), SecurityAlgorithms.HmacSha256),
      Subject = session.CreateClaimsIdentity(),
      TokenType = _settings.AccessToken.TokenType
    };
    SecurityToken securityToken = _tokenHandler.CreateToken(tokenDescriptor);
    string accessToken = _tokenHandler.WriteToken(securityToken);

    return new TokenResponse(accessToken, Schemes.Bearer)
    {
      ExpiresIn = _settings.AccessToken.LifetimeSeconds,
      RefreshToken = session.RefreshToken
    };
  }

  public ClaimsPrincipal ValidateToken(string token)
  {
    string baseUrl = GetBaseUrl();

    return _tokenHandler.ValidateToken(token, new TokenValidationParameters
    {
      IssuerSigningKey = GetSecurityKey(),
      ValidAudiences = [baseUrl],
      ValidIssuers = [baseUrl],
      ValidTypes = [_settings.AccessToken.TokenType],
      ValidateAudience = true,
      ValidateIssuer = true,
      ValidateIssuerSigningKey = true
    }, out _);
  }

  private string GetBaseUrl()
  {
    HttpContext context = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException($"The {nameof(_httpContextAccessor.HttpContext)} is required.");
    return context.GetBaseUri().ToString();
  }
  private SymmetricSecurityKey GetSecurityKey() => new(Encoding.ASCII.GetBytes(_settings.AccessToken.Secret));
}
