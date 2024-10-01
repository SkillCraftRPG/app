using SkillCraft.Application.Worlds.Commands;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Tools.ETL;

internal interface IApiClient
{
  Task<WorldModel> SaveWorldAsync(SaveWorldCommand command, CancellationToken cancellationToken = default);
}
