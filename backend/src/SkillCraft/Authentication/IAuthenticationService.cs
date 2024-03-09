using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Tokens;
using SkillCraft.Models.Account;

namespace SkillCraft.Authentication;

public interface IAuthenticationService
{
  TokenResponse GetTokenResponse(Session session);
  ValidatedToken ValidateAccessToken(string accessToken);
}
