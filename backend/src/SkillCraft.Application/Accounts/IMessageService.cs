using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Users;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Application.Accounts;

public interface IMessageService
{
  Task<SentMessages> SendAsync(string template, Email email, LocaleUnit locale, IReadOnlyDictionary<string, string> variables, CancellationToken cancellationToken = default);
  Task<SentMessages> SendAsync(string template, Phone phone, LocaleUnit locale, IReadOnlyDictionary<string, string> variables, CancellationToken cancellationToken = default);
  Task<SentMessages> SendAsync(string template, User user, ContactType contactType, LocaleUnit locale, IReadOnlyDictionary<string, string> variables, CancellationToken cancellationToken = default);
}
