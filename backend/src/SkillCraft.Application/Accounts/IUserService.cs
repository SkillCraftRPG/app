using Logitar.Portal.Contracts.Users;

namespace SkillCraft.Application.Accounts;

public interface IUserService
{
  Task<User> AuthenticateAsync(User user, string password, CancellationToken cancellationToken = default);
  Task<User> CreateAsync(Email email, CancellationToken cancellationToken = default);
  Task<User?> FindAsync(Guid id, CancellationToken cancellationToken = default);
  Task<User?> FindAsync(string uniqueName, CancellationToken cancellationToken = default);
  Task<User> UpdateEmailAsync(User user, Email email, CancellationToken cancellationToken = default);
}
