using Logitar.Portal.Contracts.Search;
using SkillCraft.Contracts.Languages;
using SkillCraft.Domain.Languages;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Languages;

public interface ILanguageQuerier
{
  Task<IReadOnlyCollection<string>> ListScriptsAsync(WorldId worldId, CancellationToken cancellationToken = default);

  Task<LanguageModel> ReadAsync(Language language, CancellationToken cancellationToken = default);
  Task<LanguageModel?> ReadAsync(LanguageId id, CancellationToken cancellationToken = default);
  Task<LanguageModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<LanguageModel>> SearchAsync(WorldId worldId, SearchLanguagesPayload payload, CancellationToken cancellationToken = default);
}
