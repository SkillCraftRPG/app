using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Talents;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain.Castes;
using SkillCraft.Domain.Educations;
using SkillCraft.Domain.Talents;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Characters.Creation;

internal record ResolveTalentsQuery(Activity Activity, Caste Caste, Education Education, IEnumerable<Guid> Ids) : IRequest<IReadOnlyCollection<Talent>>;

internal class ResolveTalentsQueryHandler : IRequestHandler<ResolveTalentsQuery, IReadOnlyCollection<Talent>>
{
  private const string PropertyName = nameof(CreateCharacterPayload.TalentIds);

  private readonly IPermissionService _permissionService;
  private readonly ITalentRepository _talentRepository;

  public ResolveTalentsQueryHandler(IPermissionService permissionService, ITalentRepository talentRepository)
  {
    _permissionService = permissionService;
    _talentRepository = talentRepository;
  }

  public async Task<IReadOnlyCollection<Talent>> Handle(ResolveTalentsQuery query, CancellationToken cancellationToken)
  {
    Activity activity = query.Activity;
    await _permissionService.EnsureCanPreviewAsync(activity, EntityType.Talent, cancellationToken);

    Caste caste = query.Caste;
    if (!caste.Skill.HasValue)
    {
      throw new NotImplementedException(); // TODO(fpion): typed exception
    }

    Education education = query.Education;
    if (!education.Skill.HasValue)
    {
      throw new NotImplementedException(); // TODO(fpion): typed exception
    }

    WorldId worldId = activity.GetWorldId();
    IEnumerable<TalentId> ids = query.Ids.Distinct().Select(id => new TalentId(worldId, id));
    IReadOnlyCollection<Talent> talents = await _talentRepository.LoadAsync(ids, cancellationToken);

    foreach (Talent talent in talents)
    {
      if (talent.Skill == null)
      {
        throw new NotImplementedException(); // TODO(fpion): typed exception
      }
      else if (talent.Skill == caste.Skill)
      {
        throw new NotImplementedException(); // TODO(fpion): typed exception
      }
      else if (talent.Skill == education.Skill)
      {
        throw new NotImplementedException(); // TODO(fpion): typed exception
      }
    }

    IEnumerable<Guid> foundIds = talents.Select(talent => talent.EntityId).Distinct();
    IEnumerable<Guid> missingIds = query.Ids.Except(foundIds).Distinct();
    if (missingIds.Any())
    {
      throw new TalentsNotFoundException(missingIds, PropertyName);
    }

    // TODO(fpion): add caste talent
    // TODO(fpion): add education talent
    // TODO(fpion): there should be exactly 4 skill talents

    return talents;
  }
}
