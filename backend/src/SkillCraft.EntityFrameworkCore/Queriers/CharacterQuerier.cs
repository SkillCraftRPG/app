using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
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

  public CharacterQuerier(IActorService actorService, SkillCraftContext context)
  {
    _actorService = actorService;
    _context = context;
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
      .Include(x => x.Aspects).ThenInclude(x => x.World)
      .Include(x => x.Caste).ThenInclude(x => x!.World)
      .Include(x => x.Customizations).ThenInclude(x => x.World)
      .Include(x => x.Education).ThenInclude(x => x!.World)
      .Include(x => x.Lineage).ThenInclude(x => x!.World)
      .Include(x => x.Personality).ThenInclude(x => x!.World)
      .Include(x => x.World)
      .SingleOrDefaultAsync(x => x.World!.Id == worldId.ToGuid() && x.Id == id, cancellationToken);

    if (character == null)
    {
      return null;
    }

    CharacterLanguageEntity[] languages = await _context.CharacterLanguages.AsNoTracking()
      .Include(x => x.Language).ThenInclude(x => x!.World)
      .Where(x => x.CharacterId == character.CharacterId)
      .ToArrayAsync(cancellationToken);
    character.Languages.AddRange(languages);

    CharacterTalentEntity[] talents = await _context.CharacterTalents.AsNoTracking()
      .Include(x => x.Talent).ThenInclude(x => x!.World)
      .Where(x => x.CharacterId == character.CharacterId)
      .ToArrayAsync(cancellationToken);
    character.Talents.AddRange(talents);

    InventoryEntity[] inventory = await _context.Inventory.AsNoTracking()
      .Include(x => x.Item).ThenInclude(x => x!.World)
      .Where(x => x.CharacterId == character.CharacterId)
      .ToArrayAsync(cancellationToken);
    character.Inventory.AddRange(inventory);

    return await MapAsync(character, cancellationToken);
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
