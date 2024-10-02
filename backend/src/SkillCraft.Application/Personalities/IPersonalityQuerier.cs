using Logitar.Portal.Contracts.Search;
using SkillCraft.Contracts.Personalities;
using SkillCraft.Domain.Personalities;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Personalities;

public interface IPersonalityQuerier
{
  Task<PersonalityModel> ReadAsync(Personality personality, CancellationToken cancellationToken = default);
  Task<PersonalityModel?> ReadAsync(PersonalityId id, CancellationToken cancellationToken = default);
  Task<PersonalityModel?> ReadAsync(WorldId worldId, Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<PersonalityModel>> SearchAsync(WorldId worldId, SearchPersonalitiesPayload payload, CancellationToken cancellationToken = default);
}
