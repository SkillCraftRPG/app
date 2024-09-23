using Logitar.Portal.Contracts.Search;
using SkillCraft.Contracts.Parties;
using SkillCraft.Domain.Parties;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Parties;

public interface IPartyQuerier
{
  Task<PartyModel> ReadAsync(Party party, CancellationToken cancellationToken = default);
  Task<PartyModel?> ReadAsync(PartyId id, CancellationToken cancellationToken = default);
  Task<PartyModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<PartyModel>> SearchAsync(WorldId worldId, SearchPartiesPayload payload, CancellationToken cancellationToken = default);
}
