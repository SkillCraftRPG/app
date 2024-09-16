namespace SkillCraft.Domain.Customizations;

public interface ICustomizationRepository
{
  Task<IReadOnlyCollection<Customization>> LoadAsync(CancellationToken cancellationToken = default);

  Task<Customization?> LoadAsync(CustomizationId id, CancellationToken cancellationToken = default);
  Task<Customization?> LoadAsync(CustomizationId id, long? version, CancellationToken cancellationToken = default);

  Task SaveAsync(Customization customization, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Customization> customizations, CancellationToken cancellationToken = default);
}
