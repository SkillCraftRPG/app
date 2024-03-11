using Logitar.Portal.Contracts.Users;
using MediatR;

namespace SkillCraft.Application.Accounts.Commands;

public record ChangePasswordCommand(User User, Contracts.Accounts.ChangePasswordPayload Payload) : IRequest<User>;
