using SkillCraft.Application.Aspects.Commands;
using SkillCraft.Application.Parties.Commands;
using SkillCraft.Application.Worlds.Commands;
using SkillCraft.Contracts.Aspects;
using SkillCraft.Contracts.Parties;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Tools.ETL;

internal interface IApiClient
{
  Task<AspectModel> SaveAspectAsync(SaveAspectCommand command, CancellationToken cancellationToken = default);
  Task<PartyModel> SavePartyAsync(SavePartyCommand command, CancellationToken cancellationToken = default);
  Task<WorldModel> SaveWorldAsync(SaveWorldCommand command, CancellationToken cancellationToken = default);
}
