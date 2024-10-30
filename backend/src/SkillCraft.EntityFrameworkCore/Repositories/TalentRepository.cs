using Logitar;
using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Contracts;
using SkillCraft.Domain.Talents;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.EntityFrameworkCore.Repositories;

internal class TalentRepository : Logitar.EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, ITalentRepository
{
  private static readonly string AggregateType = typeof(Talent).GetNamespaceQualifiedName();

  private readonly ISqlHelper _sqlHelper;

  public TalentRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer, ISqlHelper sqlHelper)
    : base(eventBus, eventContext, eventSerializer)
  {
    _sqlHelper = sqlHelper;
  }

  public async Task<IReadOnlyCollection<Talent>> LoadAsync(CancellationToken cancellationToken)
  {
    return (await LoadAsync<Talent>(cancellationToken)).ToArray();
  }

  public async Task<Talent?> LoadAsync(TalentId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, cancellationToken);
  }
  public async Task<Talent?> LoadAsync(TalentId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<Talent>(id.AggregateId, version, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Talent>> LoadAsync(IEnumerable<TalentId> ids, CancellationToken cancellationToken)
  {
    IEnumerable<AggregateId> aggregateIds = ids.Distinct().Select(id => id.AggregateId);
    return (await LoadAsync<Talent>(aggregateIds, cancellationToken)).ToArray();
  }

  public async Task<Talent?> LoadAsync(WorldId worldId, Skill skill, CancellationToken cancellationToken)
  {
    IQuery query = _sqlHelper.QueryFrom(EventDb.Events.Table)
      .Join(SkillCraftDb.Talents.AggregateId, EventDb.Events.AggregateId,
        new OperatorCondition(EventDb.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Join(SkillCraftDb.Worlds.WorldId, SkillCraftDb.Talents.WorldId)
      .Where(SkillCraftDb.Worlds.Id, Operators.IsEqualTo(worldId.ToGuid()))
      .Where(SkillCraftDb.Talents.Skill, Operators.IsEqualTo(skill.ToString()))
      .SelectAll(EventDb.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return Load<Talent>(events.Select(EventSerializer.Deserialize)).SingleOrDefault();
  }

  public async Task SaveAsync(Talent talent, CancellationToken cancellationToken)
  {
    await base.SaveAsync(talent, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Talent> talents, CancellationToken cancellationToken)
  {
    await base.SaveAsync(talents, cancellationToken);
  }
}
