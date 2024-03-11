using MediatR;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Application.Accounts.Commands;

public record ResetPasswordCommand(ResetPasswordPayload Payload) : IRequest<Unit>;
