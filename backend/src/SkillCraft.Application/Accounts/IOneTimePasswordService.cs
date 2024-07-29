using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Users;

namespace SkillCraft.Application.Accounts;

public interface IOneTimePasswordService
{
  Task<OneTimePassword> CreateAsync(User user, string purpose, CancellationToken cancellationToken = default);
}
