﻿namespace SkillCraft.Domain.Languages;

public interface ILanguageRepository
{
  Task<IReadOnlyCollection<Language>> LoadAsync(CancellationToken cancellationToken = default);

  Task<Language?> LoadAsync(LanguageId id, CancellationToken cancellationToken = default);
  Task<Language?> LoadAsync(LanguageId id, long? version, CancellationToken cancellationToken = default);

  Task SaveAsync(Language language, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Language> languages, CancellationToken cancellationToken = default);
}