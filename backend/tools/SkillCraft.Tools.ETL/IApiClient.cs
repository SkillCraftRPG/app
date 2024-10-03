using SkillCraft.Application.Aspects.Commands;
using SkillCraft.Application.Castes.Commands;
using SkillCraft.Application.Customizations.Commands;
using SkillCraft.Application.Educations.Commands;
using SkillCraft.Application.Languages.Commands;
using SkillCraft.Application.Parties.Commands;
using SkillCraft.Application.Personalities.Commands;
using SkillCraft.Application.Worlds.Commands;
using SkillCraft.Contracts.Aspects;
using SkillCraft.Contracts.Castes;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Contracts.Educations;
using SkillCraft.Contracts.Languages;
using SkillCraft.Contracts.Parties;
using SkillCraft.Contracts.Personalities;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Tools.ETL;

internal interface IApiClient
{
  Task<AspectModel> CreateOrReplaceAspectAsync(CreateOrReplaceAspectCommand command, CancellationToken cancellationToken = default);
  Task<CasteModel> CreateOrReplaceCasteAsync(CreateOrReplaceCasteCommand command, CancellationToken cancellationToken = default);
  Task<CustomizationModel> CreateOrReplaceCustomizationAsync(CreateOrReplaceCustomizationCommand command, CancellationToken cancellationToken = default);
  Task<EducationModel> CreateOrReplaceEducationAsync(CreateOrReplaceEducationCommand command, CancellationToken cancellationToken = default);
  Task<LanguageModel> CreateOrReplaceLanguageAsync(CreateOrReplaceLanguageCommand command, CancellationToken cancellationToken = default);
  Task<PartyModel> CreateOrReplacePartyAsync(CreateOrReplacePartyCommand command, CancellationToken cancellationToken = default);
  Task<PersonalityModel> CreateOrReplacePersonalityAsync(CreateOrReplacePersonalityCommand command, CancellationToken cancellationToken = default);
  Task<WorldModel> CreateOrReplaceWorldAsync(CreateOrReplaceWorldCommand command, CancellationToken cancellationToken = default);
}
