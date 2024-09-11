using Logitar;
using Logitar.Data;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Domain.Storages;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.EntityFrameworkCore.Repositories;

internal class StorageRepository : Logitar.EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IStorageRepository
{
  private static readonly string AggregateType = typeof(Storage).GetNamespaceQualifiedName();

  private readonly ISqlHelper _sqlHelper;

  public StorageRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer, ISqlHelper sqlHelper)
    : base(eventBus, eventContext, eventSerializer)
  {
    _sqlHelper = sqlHelper;
  }

  public async Task<Storage?> LoadAsync(WorldId worldId, CancellationToken cancellationToken)
  {
    IQuery query = _sqlHelper.QueryFrom(EventDb.Events.Table)
      .Join(SkillCraftDb.StorageSummaries.AggregateId, EventDb.Events.AggregateId,
        new OperatorCondition(EventDb.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Join(SkillCraftDb.Worlds.UserId, SkillCraftDb.StorageSummaries.UserId)
      .Where(SkillCraftDb.Worlds.Id, Operators.IsEqualTo(worldId.ToGuid()))
      .SelectAll(EventDb.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return Load<Storage>(events.Select(EventSerializer.Deserialize)).SingleOrDefault();
  }

  public async Task SaveAsync(Storage storage, CancellationToken cancellationToken)
  {
    await base.SaveAsync(storage, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Storage> storages, CancellationToken cancellationToken)
  {
    await base.SaveAsync(storages, cancellationToken);
  }
}
