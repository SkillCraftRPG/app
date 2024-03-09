using Logitar.Portal.Contracts.Sessions;
using SkillCraft.Models.Account;

namespace SkillCraft.Authentication;

public interface IAuthenticationService
{
  TokenResponse GetTokenResponse(Session session);
}
