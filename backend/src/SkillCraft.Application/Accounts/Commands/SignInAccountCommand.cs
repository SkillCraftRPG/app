using Logitar.Portal.Contracts;
using MediatR;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Application.Accounts.Commands;

public record SignInAccountCommand(SignInAccountPayload Payload, IEnumerable<CustomAttribute> CustomAttributes) : IRequest<SignInAccountResult>;
