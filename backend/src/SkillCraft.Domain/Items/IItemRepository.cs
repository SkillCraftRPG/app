﻿namespace SkillCraft.Domain.Items;

public interface IItemRepository
{
  Task<IReadOnlyCollection<Item>> LoadAsync(CancellationToken cancellationToken = default);

  Task<Item?> LoadAsync(ItemId id, CancellationToken cancellationToken = default);
  Task<Item?> LoadAsync(ItemId id, long? version, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<Item>> LoadAsync(IEnumerable<ItemId> ids, CancellationToken cancellationToken = default);

  Task SaveAsync(Item item, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Item> items, CancellationToken cancellationToken = default);
}
