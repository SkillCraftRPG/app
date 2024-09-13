using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Educations;

namespace SkillCraft.Application.Educations.Queries;

public record ReadEducationQuery(Guid Id) : Activity, IRequest<EducationModel?>;

internal class ReadEducationQueryHandler : IRequestHandler<ReadEducationQuery, EducationModel?>
{
  private readonly IEducationQuerier _educationQuerier;
  private readonly IPermissionService _permissionService;

  public ReadEducationQueryHandler(IEducationQuerier educationQuerier, IPermissionService permissionService)
  {
    _educationQuerier = educationQuerier;
    _permissionService = permissionService;
  }

  public async Task<EducationModel?> Handle(ReadEducationQuery query, CancellationToken cancellationToken)
  {
    EducationModel? education = await _educationQuerier.ReadAsync(query.Id, cancellationToken);
    if (education != null)
    {
      await _permissionService.EnsureCanPreviewAsync(query, EntityMetadata.From(education), cancellationToken);
    }

    return education;
  }
}
