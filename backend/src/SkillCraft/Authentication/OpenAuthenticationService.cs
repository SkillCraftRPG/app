using Logitar.Portal.Contracts.Sessions;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Authentication;

internal class OpenAuthenticationService : IOpenAuthenticationService
{
  public TokenResponse GetTokenResponse(Session session)
  {
    throw new NotImplementedException(); // TODO(fpion): implement
  }

  public ClaimsPrincipal ValidateToken(string token)
  {
    throw new NotImplementedException(); // TODO(fpion): implement
  }
}
