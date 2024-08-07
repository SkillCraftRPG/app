using MediatR;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Application.Worlds.Commands;

public record DeleteWorldCommand(Guid Id) : Activity, IRequest<World?>;
