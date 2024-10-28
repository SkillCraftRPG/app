using Logitar.Portal.Contracts.Search;
using SkillCraft.Contracts.Items;
using SkillCraft.Domain.Items;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Items;

public interface IItemQuerier
{
  Task<ItemModel> ReadAsync(Item item, CancellationToken cancellationToken = default);
  Task<ItemModel?> ReadAsync(ItemId id, CancellationToken cancellationToken = default);
  Task<ItemModel?> ReadAsync(WorldId worldId, Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<ItemModel>> SearchAsync(WorldId worldId, SearchItemsPayload payload, CancellationToken cancellationToken = default);
}
