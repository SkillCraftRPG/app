using MediatR;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Application.Worlds.Commands;

public record ReplaceWorldCommand(Guid Id, ReplaceWorldPayload Payload, long? Version) : Activity, IRequest<World?>;
