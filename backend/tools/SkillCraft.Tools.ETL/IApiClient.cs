using SkillCraft.Application.Aspects.Commands;
using SkillCraft.Application.Castes.Commands;
using SkillCraft.Application.Educations.Commands;
using SkillCraft.Application.Parties.Commands;
using SkillCraft.Application.Worlds.Commands;
using SkillCraft.Contracts.Aspects;
using SkillCraft.Contracts.Castes;
using SkillCraft.Contracts.Educations;
using SkillCraft.Contracts.Parties;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Tools.ETL;

internal interface IApiClient
{
  Task<AspectModel> SaveAspectAsync(SaveAspectCommand command, CancellationToken cancellationToken = default);
  Task<CasteModel> SaveCasteAsync(SaveCasteCommand command, CancellationToken cancellationToken = default);
  Task<EducationModel> SaveEducationAsync(SaveEducationCommand command, CancellationToken cancellationToken = default);
  Task<PartyModel> SavePartyAsync(SavePartyCommand command, CancellationToken cancellationToken = default);
  Task<WorldModel> SaveWorldAsync(SaveWorldCommand command, CancellationToken cancellationToken = default);
}
