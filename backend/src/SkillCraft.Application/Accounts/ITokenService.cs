using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;

namespace SkillCraft.Application.Accounts;

public interface ITokenService
{
  Task<CreatedToken> CreateAsync(User user, string type, CancellationToken cancellationToken = default);
  Task<CreatedToken> CreateAsync(User? user, Email email, string type, CancellationToken cancellationToken = default);
  Task<ValidatedToken> ValidateAsync(string token, string type, CancellationToken cancellationToken = default);
}
