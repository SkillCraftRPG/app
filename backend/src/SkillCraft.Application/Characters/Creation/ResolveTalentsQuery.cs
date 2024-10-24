using MediatR;
using SkillCraft.Application.Castes;
using SkillCraft.Application.Educations;
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
    Talent casteTalent = await FindCasteTalentAsync(caste, cancellationToken);

    Education education = query.Education;
    Talent educationTalent = await FindEducationTalentAsync(education, cancellationToken);

    if (casteTalent.Equals(educationTalent))
    {
      throw new InvalidCasteEducationSelectionException(caste, education);
    }

    WorldId worldId = activity.GetWorldId();
    IEnumerable<TalentId> ids = query.Ids.Distinct().Select(id => new TalentId(worldId, id));
    IReadOnlyCollection<Talent> talents = await _talentRepository.LoadAsync(ids, cancellationToken);

    HashSet<Guid> foundIds = new(capacity: talents.Count);
    HashSet<Guid> invalidIds = new(capacity: talents.Count);
    foreach (Talent talent in talents)
    {
      foundIds.Add(talent.EntityId);

      if (talent.Skill == null || talent.Equals(casteTalent) || talent.Equals(educationTalent))
      {
        invalidIds.Add(talent.EntityId);
      }
    }
    if (invalidIds.Count > 0)
    {
      throw new InvalidSkillTalentSelectionException(caste, education, invalidIds, nameof(CreateCharacterPayload.TalentIds));
    }

    IEnumerable<Guid> missingIds = query.Ids.Except(foundIds).Distinct();
    if (missingIds.Any())
    {
      throw new TalentsNotFoundException(worldId, missingIds, PropertyName);
    }

    return talents.Concat([casteTalent, educationTalent]).ToArray().AsReadOnly();
  }

  private async Task<Talent> FindCasteTalentAsync(Caste caste, CancellationToken cancellationToken)
  {
    Talent? talent = null;
    if (caste.Skill.HasValue)
    {
      talent = await _talentRepository.LoadAsync(caste.WorldId, caste.Skill.Value, cancellationToken);
    }
    if (talent == null)
    {
      throw new CasteHasNoSkillTalentException(caste, nameof(CreateCharacterPayload.CasteId));
    }
    return talent;
  }

  private async Task<Talent> FindEducationTalentAsync(Education education, CancellationToken cancellationToken)
  {
    Talent? talent = null;
    if (education.Skill.HasValue)
    {
      talent = await _talentRepository.LoadAsync(education.WorldId, education.Skill.Value, cancellationToken);
    }
    if (talent == null)
    {
      throw new EducationHasNoSkillTalentException(education, nameof(CreateCharacterPayload.CasteId));
    }
    return talent;
  }
}
