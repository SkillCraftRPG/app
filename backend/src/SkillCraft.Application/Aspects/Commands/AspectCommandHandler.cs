using SkillCraft.Application.Storages;
using SkillCraft.Domain.Aspects;

namespace SkillCraft.Application.Aspects.Commands;

internal abstract class AspectCommandHandler
{
  private readonly IAspectRepository _aspectRepository;
  private readonly IStorageService _storageService;

  public AspectCommandHandler(IAspectRepository aspectRepository, IStorageService storageService)
  {
    _aspectRepository = aspectRepository;
    _storageService = storageService;
  }

  protected async Task SaveAsync(Aspect aspect, CancellationToken cancellationToken)
  {
    EntityMetadata entity = aspect.GetMetadata();
    await _storageService.EnsureAvailableAsync(entity, cancellationToken);

    await _aspectRepository.SaveAsync(aspect, cancellationToken);

    await _storageService.UpdateAsync(entity, cancellationToken);
  }
}
