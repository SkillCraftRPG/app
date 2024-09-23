namespace SkillCraft.Domain.Parties;

public interface IPartyRepository
{
  Task<IReadOnlyCollection<Party>> LoadAsync(CancellationToken cancellationToken = default);

  Task<Party?> LoadAsync(PartyId id, CancellationToken cancellationToken = default);
  Task<Party?> LoadAsync(PartyId id, long? version, CancellationToken cancellationToken = default);

  Task SaveAsync(Party party, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Party> parties, CancellationToken cancellationToken = default);
}
