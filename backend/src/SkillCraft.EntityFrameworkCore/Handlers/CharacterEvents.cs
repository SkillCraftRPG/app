using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Domain.Characters;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Handlers;

internal class CharacterEvents : INotificationHandler<Character.BonusRemovedEvent>,
  INotificationHandler<Character.BonusUpdatedEvent>,
  INotificationHandler<Character.CreatedEvent>,
  INotificationHandler<Character.ExperienceGainedEvent>,
  INotificationHandler<Character.InventoryUpdatedEvent>,
  INotificationHandler<Character.LanguageRemovedEvent>,
  INotificationHandler<Character.LanguageUpdatedEvent>,
  INotificationHandler<Character.LevelUpCancelledEvent>,
  INotificationHandler<Character.LeveledUpEvent>,
  INotificationHandler<Character.SkillRankIncreasedEvent>,
  INotificationHandler<Character.TalentRemovedEvent>,
  INotificationHandler<Character.TalentUpdatedEvent>,
  INotificationHandler<Character.UpdatedEvent>
{
  private readonly SkillCraftContext _context;

  public CharacterEvents(SkillCraftContext context)
  {
    _context = context;
  }

  public async Task Handle(Character.BonusRemovedEvent @event, CancellationToken cancellationToken)
  {
    CharacterEntity character = await _context.Characters
      .Include(x => x.Bonuses)
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The character entity 'AggregateId={@event.AggregateId}' could not be found.");

    character.RemoveBonus(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }

  public async Task Handle(Character.BonusUpdatedEvent @event, CancellationToken cancellationToken)
  {
    CharacterEntity character = await _context.Characters
      .Include(x => x.Bonuses)
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The character entity 'AggregateId={@event.AggregateId}' could not be found.");

    character.SetBonus(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }

  public async Task Handle(Character.CreatedEvent @event, CancellationToken cancellationToken)
  {
    CharacterEntity? character = await _context.Characters.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (character == null)
    {
      Guid worldId = new CharacterId(@event.AggregateId).WorldId.ToGuid();
      WorldEntity world = await _context.Worlds
        .SingleOrDefaultAsync(x => x.Id == worldId, cancellationToken)
        ?? throw new InvalidOperationException($"The world entity 'Id={worldId}' could not be found.");

      string lineageId = (@event.NationId ?? @event.SpeciesId).Value;
      LineageEntity lineage = await _context.Lineages
        .SingleOrDefaultAsync(x => x.AggregateId == lineageId, cancellationToken)
        ?? throw new InvalidOperationException($"The lineage entity 'AggregateId={lineageId}' could not be found.");
      NatureEntity nature = await _context.Natures
        .SingleOrDefaultAsync(x => x.AggregateId == @event.NatureId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The nature entity 'AggregateId={@event.NatureId}' could not be found.");
      CasteEntity caste = await _context.Castes
        .SingleOrDefaultAsync(x => x.AggregateId == @event.CasteId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The caste entity 'AggregateId={@event.CasteId}' could not be found.");
      EducationEntity education = await _context.Educations
        .SingleOrDefaultAsync(x => x.AggregateId == @event.EducationId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The education entity 'AggregateId={@event.EducationId}' could not be found.");

      HashSet<string> customizationIds = @event.CustomizationIds.Select(id => id.Value).ToHashSet();
      CustomizationEntity[] customizations = await _context.Customizations
        .Where(x => customizationIds.Contains(x.AggregateId))
        .ToArrayAsync(cancellationToken);

      HashSet<string> aspectIds = @event.AspectIds.Select(id => id.Value).ToHashSet();
      AspectEntity[] aspects = await _context.Aspects
        .Where(x => aspectIds.Contains(x.AggregateId))
        .ToArrayAsync(cancellationToken);

      character = new(world, lineage, nature, customizations, aspects, caste, education, @event);

      _context.Characters.Add(character);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task Handle(Character.ExperienceGainedEvent @event, CancellationToken cancellationToken)
  {
    CharacterEntity character = await _context.Characters
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The character entity 'AggregateId={@event.AggregateId}' could not be found.");

    character.GainExperience(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }

  public async Task Handle(Character.InventoryUpdatedEvent @event, CancellationToken cancellationToken)
  {
    CharacterEntity character = await _context.Characters
      .Include(x => x.Inventory)
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The character entity 'AggregateId={@event.AggregateId}' could not be found.");

    ItemEntity item = await _context.Items
      .SingleOrDefaultAsync(x => x.AggregateId == @event.Item.ItemId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The item entity 'AggregateId={@event.Item.ItemId}' could not be found.");

    character.SetItem(item, @event);

    await _context.SaveChangesAsync(cancellationToken);
  }

  public async Task Handle(Character.LanguageRemovedEvent @event, CancellationToken cancellationToken)
  {
    CharacterEntity character = await _context.Characters
      .Include(x => x.Languages).ThenInclude(x => x.Language)
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The character entity 'AggregateId={@event.AggregateId}' could not be found.");

    character.RemoveLanguage(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }

  public async Task Handle(Character.LanguageUpdatedEvent @event, CancellationToken cancellationToken)
  {
    CharacterEntity character = await _context.Characters
      .Include(x => x.Languages)
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The character entity 'AggregateId={@event.AggregateId}' could not be found.");

    LanguageEntity language = await _context.Languages
      .SingleOrDefaultAsync(x => x.AggregateId == @event.LanguageId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The language entity 'AggregateId={@event.LanguageId}' could not be found.");

    character.SetLanguage(language, @event);

    await _context.SaveChangesAsync(cancellationToken);
  }

  public async Task Handle(Character.LevelUpCancelledEvent @event, CancellationToken cancellationToken)
  {
    CharacterEntity character = await _context.Characters
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The character entity 'AggregateId={@event.AggregateId}' could not be found.");

    character.CancelLevelUp(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }

  public async Task Handle(Character.LeveledUpEvent @event, CancellationToken cancellationToken)
  {
    CharacterEntity character = await _context.Characters
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The character entity 'AggregateId={@event.AggregateId}' could not be found.");

    character.LevelUp(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }

  public async Task Handle(Character.SkillRankIncreasedEvent @event, CancellationToken cancellationToken)
  {
    CharacterEntity character = await _context.Characters
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The character entity 'AggregateId={@event.AggregateId}' could not be found.");

    character.IncreaseSkillRank(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }

  public async Task Handle(Character.TalentRemovedEvent @event, CancellationToken cancellationToken)
  {
    CharacterEntity character = await _context.Characters
      .Include(x => x.Talents)
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The character entity 'AggregateId={@event.AggregateId}' could not be found.");

    character.RemoveTalent(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }

  public async Task Handle(Character.TalentUpdatedEvent @event, CancellationToken cancellationToken)
  {
    CharacterEntity character = await _context.Characters
      .Include(x => x.Talents)
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The character entity 'AggregateId={@event.AggregateId}' could not be found.");

    TalentEntity talent = await _context.Talents
      .SingleOrDefaultAsync(x => x.AggregateId == @event.Talent.Id.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The talent entity 'AggregateId={@event.Talent.Id}' could not be found.");

    character.SetTalent(talent, @event);

    await _context.SaveChangesAsync(cancellationToken);
  }

  public async Task Handle(Character.UpdatedEvent @event, CancellationToken cancellationToken)
  {
    CharacterEntity character = await _context.Characters
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The character entity 'AggregateId={@event.AggregateId}' could not be found.");

    character.Update(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
