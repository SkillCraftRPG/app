using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Users;

namespace SkillCraft.Application.Accounts;

public interface IMessageService
{
  Task<SentMessages> SendAsync(string template, Email email, string? locale, IEnumerable<KeyValuePair<string, string>>? variables, CancellationToken cancellationToken = default);
  Task<SentMessages> SendAsync(string template, User user, string? locale, IEnumerable<KeyValuePair<string, string>>? variables, CancellationToken cancellationToken = default);
}
