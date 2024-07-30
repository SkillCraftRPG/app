using Logitar.Portal.Contracts.Sessions;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Authentication;

public interface IOpenAuthenticationService
{
  TokenResponse GetTokenResponse(Session session);
  ClaimsPrincipal ValidateToken(string token);
}
