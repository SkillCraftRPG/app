using MediatR;
using SkillCraft.Application.Storages;
using SkillCraft.Domain.Lineages;

namespace SkillCraft.Application.Lineages.Commands;

internal record SaveLineageCommand(Lineage Lineage) : IRequest;

internal class SaveLineageCommandHandler : IRequestHandler<SaveLineageCommand>
{
  private readonly ILineageRepository _lineageRepository;
  private readonly IStorageService _storageService;

  public SaveLineageCommandHandler(ILineageRepository lineageRepository, IStorageService storageService)
  {
    _lineageRepository = lineageRepository;
    _storageService = storageService;
  }

  public async Task Handle(SaveLineageCommand command, CancellationToken cancellationToken)
  {
    Lineage lineage = command.Lineage;

    EntityMetadata entity = lineage.GetMetadata();
    await _storageService.EnsureAvailableAsync(entity, cancellationToken);

    await _lineageRepository.SaveAsync(lineage, cancellationToken);

    await _storageService.UpdateAsync(entity, cancellationToken);
  }
}
