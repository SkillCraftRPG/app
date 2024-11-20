using MediatR;
using SkillCraft.Application.Storages;
using SkillCraft.Domain.Natures;

namespace SkillCraft.Application.Natures.Commands;

internal record SaveNatureCommand(Nature Nature) : IRequest;

internal class SaveNatureCommandHandler : IRequestHandler<SaveNatureCommand>
{
  private readonly INatureRepository _natureRepository;
  private readonly IStorageService _storageService;

  public SaveNatureCommandHandler(INatureRepository natureRepository, IStorageService storageService)
  {
    _natureRepository = natureRepository;
    _storageService = storageService;
  }

  public async Task Handle(SaveNatureCommand command, CancellationToken cancellationToken)
  {
    Nature nature = command.Nature;

    EntityMetadata entity = nature.GetMetadata();
    await _storageService.EnsureAvailableAsync(entity, cancellationToken);

    await _natureRepository.SaveAsync(nature, cancellationToken);

    await _storageService.UpdateAsync(entity, cancellationToken);
  }
}
