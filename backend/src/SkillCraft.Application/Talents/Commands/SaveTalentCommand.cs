using MediatR;
using SkillCraft.Application.Storages;
using SkillCraft.Domain.Talents;

namespace SkillCraft.Application.Talents.Commands;

internal record SaveTalentCommand(Talent Talent) : IRequest;

internal class SaveTalentCommandHandler : IRequestHandler<SaveTalentCommand>
{
  private readonly IStorageService _storageService;
  private readonly ITalentRepository _talentRepository;

  public SaveTalentCommandHandler(IStorageService storageService, ITalentRepository talentRepository)
  {
    _storageService = storageService;
    _talentRepository = talentRepository;
  }

  public async Task Handle(SaveTalentCommand command, CancellationToken cancellationToken)
  {
    Talent talent = command.Talent;

    EntityMetadata entity = talent.GetMetadata();
    await _storageService.EnsureAvailableAsync(entity, cancellationToken);

    await _talentRepository.SaveAsync(talent, cancellationToken);

    await _storageService.UpdateAsync(entity, cancellationToken);
  }
}
