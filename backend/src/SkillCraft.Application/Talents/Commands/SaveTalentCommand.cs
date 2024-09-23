using Logitar.EventSourcing;
using MediatR;
using SkillCraft.Application.Storages;
using SkillCraft.Domain.Talents;

namespace SkillCraft.Application.Talents.Commands;

internal record SaveTalentCommand(Talent Talent) : IRequest;

internal class SaveTalentCommandHandler : IRequestHandler<SaveTalentCommand>
{
  private readonly IStorageService _storageService;
  private readonly ITalentQuerier _talentQuerier;
  private readonly ITalentRepository _talentRepository;

  public SaveTalentCommandHandler(IStorageService storageService, ITalentQuerier talentQuerier, ITalentRepository talentRepository)
  {
    _storageService = storageService;
    _talentQuerier = talentQuerier;
    _talentRepository = talentRepository;
  }

  public async Task Handle(SaveTalentCommand command, CancellationToken cancellationToken)
  {
    Talent talent = command.Talent;

    bool hasSkillChanged = false;
    foreach (DomainEvent change in talent.Changes)
    {
      if (change is Talent.CreatedEvent || change is Talent.UpdatedEvent updatedEvent && updatedEvent.Skill != null)
      {
        hasSkillChanged = true;
        break;
      }
    }

    if (hasSkillChanged && talent.Skill.HasValue)
    {
      TalentId? otherId = await _talentQuerier.FindIdAsync(talent.WorldId, talent.Skill.Value, cancellationToken);
      if (otherId.HasValue && talent.Id != otherId.Value)
      {
        throw new TalentSkillAlreadyExistingException(talent, otherId.Value, nameof(Talent.Name));
      }
    }

    EntityMetadata entity = talent.GetMetadata();
    await _storageService.EnsureAvailableAsync(entity, cancellationToken);

    await _talentRepository.SaveAsync(talent, cancellationToken);

    await _storageService.UpdateAsync(entity, cancellationToken);
  }
}
