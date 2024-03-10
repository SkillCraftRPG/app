using Logitar.Portal.Contracts.Users;
using MediatR;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Application.Accounts.Commands;

public record SaveProfileCommand(User User, SaveProfilePayload Payload) : IRequest<User>;
