using MediatR;
using SkillCraft.Domain.Talents;

namespace SkillCraft.Application.Talents.Commands;

internal record SetRequiredTalentCommand(Talent Talent, Guid? Id) : IRequest;

internal class SetRequiredTalentCommandHandler : IRequestHandler<SetRequiredTalentCommand>
{
  private const string PropertyName = nameof(Talent.RequiredTalentId);

  private readonly ITalentRepository _talentRepository;

  public SetRequiredTalentCommandHandler(ITalentRepository talentRepository)
  {
    _talentRepository = talentRepository;
  }

  public async Task Handle(SetRequiredTalentCommand command, CancellationToken cancellationToken)
  {
    Talent talent = command.Talent;
    Talent? requiredTalent = null;

    if (command.Id.HasValue)
    {
      TalentId talentId = new(talent.WorldId, command.Id.Value);
      requiredTalent = await _talentRepository.LoadAsync(talentId, cancellationToken)
        ?? throw new AggregateNotFoundException<Talent>(talentId.AggregateId, PropertyName);

      if (requiredTalent.Tier > talent.Tier)
      {
        throw new InvalidRequiredTalentTierException(requiredTalent, PropertyName);
      }
    }

    talent.SetRequiredTalent(requiredTalent);
  }
}
