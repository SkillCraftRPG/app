using Logitar.Portal.Contracts.Users;

namespace SkillCraft.Application.Accounts;

public record GoogleIdentity(string Id, EmailPayload Email, string? FirstName, string? LastName, string? Locale, string? Picture);
