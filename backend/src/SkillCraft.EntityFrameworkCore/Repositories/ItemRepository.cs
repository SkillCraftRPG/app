using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using SkillCraft.Domain.Items;

namespace SkillCraft.EntityFrameworkCore.Repositories;

internal class ItemRepository : Logitar.EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IItemRepository
{
  public ItemRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer)
    : base(eventBus, eventContext, eventSerializer)
  {
  }

  public async Task<IReadOnlyCollection<Item>> LoadAsync(CancellationToken cancellationToken)
  {
    return (await LoadAsync<Item>(cancellationToken)).ToArray();
  }

  public async Task<Item?> LoadAsync(ItemId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, cancellationToken);
  }
  public async Task<Item?> LoadAsync(ItemId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<Item>(id.AggregateId, version, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Item>> LoadAsync(IEnumerable<ItemId> ids, CancellationToken cancellationToken)
  {
    IEnumerable<AggregateId> aggregateIds = ids.Distinct().Select(id => id.AggregateId);
    return (await LoadAsync<Item>(aggregateIds, cancellationToken)).ToArray();
  }

  public async Task SaveAsync(Item item, CancellationToken cancellationToken)
  {
    await base.SaveAsync(item, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Item> items, CancellationToken cancellationToken)
  {
    await base.SaveAsync(items, cancellationToken);
  }
}
