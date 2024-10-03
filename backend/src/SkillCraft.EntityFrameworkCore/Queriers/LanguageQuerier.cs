using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Search;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Application.Actors;
using SkillCraft.Application.Languages;
using SkillCraft.Contracts.Languages;
using SkillCraft.Domain.Languages;
using SkillCraft.Domain.Worlds;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Queriers;

internal class LanguageQuerier : ILanguageQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<LanguageEntity> _languages;
  private readonly ISqlHelper _sqlHelper;

  public LanguageQuerier(IActorService actorService, SkillCraftContext context, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _languages = context.Languages;
    _sqlHelper = sqlHelper;
  }

  public async Task<IReadOnlyCollection<string>> ListScriptsAsync(WorldId worldId, CancellationToken cancellationToken)
  {
    string[] scripts = await _languages.AsNoTracking()
      .Where(x => x.World!.Id == worldId.ToGuid() && x.Script != null)
      .OrderBy(x => x.Script)
      .Select(x => x.Script!)
      .Distinct()
      .ToArrayAsync(cancellationToken);

    return scripts.AsReadOnly();
  }

  public async Task<LanguageModel> ReadAsync(Language language, CancellationToken cancellationToken)
  {
    return await ReadAsync(language.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The language entity 'Id={language.Id.ToGuid()}' could not be found.");
  }
  public async Task<LanguageModel?> ReadAsync(LanguageId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.ToGuid(), cancellationToken);
  }
  public async Task<LanguageModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    LanguageEntity? language = await _languages.AsNoTracking()
      .Include(x => x.World)
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    return language == null ? null : await MapAsync(language, cancellationToken);
  }

  public async Task<SearchResults<LanguageModel>> SearchAsync(WorldId worldId, SearchLanguagesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.QueryFrom(SkillCraftDb.Languages.Table).SelectAll(SkillCraftDb.Languages.Table)
      .Join(SkillCraftDb.Worlds.WorldId, SkillCraftDb.Languages.WorldId)
      .Where(SkillCraftDb.Worlds.Id, Operators.IsEqualTo(worldId.ToGuid()))
      .ApplyIdFilter(payload, SkillCraftDb.Languages.Id);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, SkillCraftDb.Languages.Name, SkillCraftDb.Languages.Script, SkillCraftDb.Languages.TypicalSpeakers);

    if (!string.IsNullOrEmpty(payload.Script))
    {
      builder.Where(SkillCraftDb.Languages.Script, Operators.IsEqualTo(payload.Script));
    }

    IQueryable<LanguageEntity> query = _languages.FromQuery(builder).AsNoTracking()
      .Include(x => x.World);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<LanguageEntity>? ordered = null;
    foreach (LanguageSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case LanguageSort.CreatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case LanguageSort.Name:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name));
          break;
        case LanguageSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;
    query = query.ApplyPaging(payload);

    LanguageEntity[] languages = await query.ToArrayAsync(cancellationToken);
    IEnumerable<LanguageModel> items = await MapAsync(languages, cancellationToken);

    return new SearchResults<LanguageModel>(items, total);
  }

  private async Task<LanguageModel> MapAsync(LanguageEntity language, CancellationToken cancellationToken)
  {
    return (await MapAsync([language], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<LanguageModel>> MapAsync(IEnumerable<LanguageEntity> languages, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = languages.SelectMany(language => language.GetActorIds());
    IReadOnlyCollection<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return languages.Select(mapper.ToLanguage).ToArray();
  }
}
