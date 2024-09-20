using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Domain.Talents;
using Action = SkillCraft.Application.Permissions.Action;

namespace SkillCraft.Application.Talents.Commands;

internal record SetRequiredTalentCommand(Activity Activity, Talent Talent, Guid? Id) : IRequest;

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
      TalentId talentId = new(command.Id.Value);
      requiredTalent = await _talentRepository.LoadAsync(talentId, cancellationToken)
        ?? throw new AggregateNotFoundException<Talent>(talentId.AggregateId, PropertyName);

      if (requiredTalent.WorldId != talent.WorldId)
      {
        Activity activity = command.Activity;
        throw new PermissionDeniedException(Action.Preview, Domain.EntityType.Talent, activity.GetUser(), activity.GetWorld(), requiredTalent.Id.ToGuid());
      }
      else if (requiredTalent.Tier > talent.Tier)
      {
        throw new InvalidRequiredTalentTierException(requiredTalent, PropertyName);
      }
    }

    talent.SetRequiredTalent(requiredTalent);
  }
}
