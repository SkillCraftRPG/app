using Logitar.Portal.Contracts.Users;
using MediatR;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Worlds.Commands;

internal record SaveWorldCommand(User User, WorldAggregate World, int PreviousSize = 0) : IRequest;
