using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Search;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Application.Actors;
using SkillCraft.Application.Characters;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain.Characters;
using SkillCraft.Domain.Worlds;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Queriers;

internal class CharacterQuerier : ICharacterQuerier
{
  private readonly IActorService _actorService;
  private readonly SkillCraftContext _context;
  private readonly ISqlHelper _sqlHelper;

  public CharacterQuerier(IActorService actorService, SkillCraftContext context, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _context = context;
    _sqlHelper = sqlHelper;
  }

  public async Task<IReadOnlyCollection<string>> ListPlayersAsync(WorldId worldId, CancellationToken cancellationToken)
  {
    string[] scripts = await _context.Characters.AsNoTracking()
      .Where(x => x.World!.Id == worldId.ToGuid() && x.PlayerName != null)
      .OrderBy(x => x.PlayerName)
      .Select(x => x.PlayerName!)
      .Distinct()
      .ToArrayAsync(cancellationToken);

    return scripts.AsReadOnly();
  }

  public async Task<CharacterModel> ReadAsync(Character character, CancellationToken cancellationToken)
  {
    return await ReadAsync(character.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The character entity 'AggregateId={character.Id}' could not be found.");
  }
  public async Task<CharacterModel?> ReadAsync(CharacterId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.WorldId, id.EntityId, cancellationToken);
  }
  public async Task<CharacterModel?> ReadAsync(WorldId worldId, Guid id, CancellationToken cancellationToken)
  {
    CharacterEntity? character = await _context.Characters.AsNoTracking()
      .Include(x => x.Aspects)
      .Include(x => x.Caste)
      .Include(x => x.Customizations)
      .Include(x => x.Education)
      .Include(x => x.Lineage).ThenInclude(x => x!.Languages)
      .Include(x => x.Lineage).ThenInclude(x => x!.Species).ThenInclude(x => x!.Languages)
      .Include(x => x.Nature).ThenInclude(x => x!.Gift)
      .Include(x => x.World)
      .SingleOrDefaultAsync(x => x.World!.Id == worldId.ToGuid() && x.Id == id, cancellationToken);

    if (character == null)
    {
      return null;
    }

    CharacterLanguageEntity[] languages = await _context.CharacterLanguages.AsNoTracking()
      .Include(x => x.Language)
      .Where(x => x.CharacterId == character.CharacterId)
      .ToArrayAsync(cancellationToken);
    character.Languages.AddRange(languages);

    CharacterTalentEntity[] talents = await _context.CharacterTalents.AsNoTracking()
      .Include(x => x.Talent)
      .Where(x => x.CharacterId == character.CharacterId)
      .ToArrayAsync(cancellationToken);
    character.Talents.AddRange(talents);

    InventoryEntity[] inventory = await _context.Inventory.AsNoTracking()
      .Include(x => x.Item)
      .Where(x => x.CharacterId == character.CharacterId)
      .ToArrayAsync(cancellationToken);
    character.Inventory.AddRange(inventory);

    return await MapAsync(character, cancellationToken);
  }

  public async Task<SearchResults<CharacterModel>> SearchAsync(WorldId worldId, SearchCharactersPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.QueryFrom(SkillCraftDb.Characters.Table).SelectAll(SkillCraftDb.Characters.Table)
      .Join(SkillCraftDb.Worlds.WorldId, SkillCraftDb.Characters.WorldId)
      .Where(SkillCraftDb.Worlds.Id, Operators.IsEqualTo(worldId.ToGuid()))
      .ApplyIdFilter(payload, SkillCraftDb.Characters.Id);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, SkillCraftDb.Characters.Name, SkillCraftDb.Characters.PlayerName);

    if (!string.IsNullOrEmpty(payload.PlayerName))
    {
      builder.Where(SkillCraftDb.Characters.PlayerName, Operators.IsEqualTo(payload.PlayerName));
    }

    IQueryable<CharacterEntity> query = _context.Characters.FromQuery(builder).AsNoTracking()
      .Include(x => x.Caste)
      .Include(x => x.Education)
      .Include(x => x.Lineage).ThenInclude(x => x!.Species)
      .Include(x => x.Nature).ThenInclude(x => x!.Gift)
      .Include(x => x.World);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<CharacterEntity>? ordered = null;
    foreach (CharacterSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case CharacterSort.CreatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case CharacterSort.Name:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name));
          break;
        case CharacterSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;
    query = query.ApplyPaging(payload);

    CharacterEntity[] characters = await query.ToArrayAsync(cancellationToken);
    IEnumerable<CharacterModel> items = await MapAsync(characters, cancellationToken);

    return new SearchResults<CharacterModel>(items, total);
  }

  private async Task<CharacterModel> MapAsync(CharacterEntity character, CancellationToken cancellationToken)
  {
    return (await MapAsync([character], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<CharacterModel>> MapAsync(IEnumerable<CharacterEntity> characters, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = characters.SelectMany(character => character.GetActorIds());
    IReadOnlyCollection<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return characters.Select(mapper.ToCharacter).ToArray();
  }
}
