using Logitar.Portal.Contracts.Search;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Customizations;

public interface ICustomizationQuerier
{
  Task<CustomizationModel> ReadAsync(Customization customization, CancellationToken cancellationToken = default);
  Task<CustomizationModel?> ReadAsync(CustomizationId id, CancellationToken cancellationToken = default);
  Task<CustomizationModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<CustomizationModel>> SearchAsync(WorldId worldId, SearchCustomizationsPayload payload, CancellationToken cancellationToken = default);
}
