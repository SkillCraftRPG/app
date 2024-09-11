using Logitar;
using Logitar.Data;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Domain;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.EntityFrameworkCore.Repositories;

internal class WorldRepository : Logitar.EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IWorldRepository
{
  private static readonly string AggregateType = typeof(World).GetNamespaceQualifiedName();

  private readonly ISqlHelper _sqlHelper;

  public WorldRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer, ISqlHelper sqlHelper)
    : base(eventBus, eventContext, eventSerializer)
  {
    _sqlHelper = sqlHelper;
  }

  public async Task<IReadOnlyCollection<World>> LoadAsync(CancellationToken cancellationToken)
  {
    return (await LoadAsync<World>(cancellationToken)).ToArray();
  }

  public async Task<World?> LoadAsync(WorldId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, cancellationToken);
  }
  public async Task<World?> LoadAsync(WorldId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<World>(id.AggregateId, version, cancellationToken);
  }

  public async Task<World?> LoadAsync(Slug slug, CancellationToken cancellationToken)
  {
    IQuery query = _sqlHelper.QueryFrom(EventDb.Events.Table)
      .Join(SkillCraftDb.Worlds.AggregateId, EventDb.Events.AggregateId,
        new OperatorCondition(EventDb.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Where(SkillCraftDb.Worlds.SlugNormalized, Operators.IsEqualTo(SkillCraftDb.Helper.Normalize(slug.Value)))
      .SelectAll(EventDb.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return Load<World>(events.Select(EventSerializer.Deserialize)).SingleOrDefault();
  }

  public async Task SaveAsync(World world, CancellationToken cancellationToken)
  {
    await base.SaveAsync(world, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<World> worlds, CancellationToken cancellationToken)
  {
    await base.SaveAsync(worlds, cancellationToken);
  }
}
