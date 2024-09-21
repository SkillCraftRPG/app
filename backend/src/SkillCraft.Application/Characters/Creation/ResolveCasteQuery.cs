using MediatR;
using SkillCraft.Application.Castes;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain.Castes;

namespace SkillCraft.Application.Characters.Creation;

internal record ResolveCasteQuery(Activity Activity, Guid Id) : IRequest<Caste>;

internal class ResolveCasteQueryHandler : IRequestHandler<ResolveCasteQuery, Caste>
{
  private readonly ICasteRepository _casteRepository;
  private readonly IPermissionService _permissionService;

  public ResolveCasteQueryHandler(ICasteRepository casteRepository, IPermissionService permissionService)
  {
    _casteRepository = casteRepository;
    _permissionService = permissionService;
  }

  public async Task<Caste> Handle(ResolveCasteQuery query, CancellationToken cancellationToken)
  {
    CasteId id = new(query.Id);
    Caste caste = await _casteRepository.LoadAsync(id, cancellationToken)
      ?? throw new AggregateNotFoundException<Caste>(id.AggregateId, nameof(CreateCharacterPayload.CasteId));

    await _permissionService.EnsureCanPreviewAsync(query.Activity, caste.GetMetadata(), cancellationToken);

    return caste;
  }
}
