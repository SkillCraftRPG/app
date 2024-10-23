using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Educations;

namespace SkillCraft.Application.Educations.Queries;

/// <exception cref="PermissionDeniedException"></exception>
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
    await _permissionService.EnsureCanPreviewAsync(query, EntityType.Education, cancellationToken);

    return await _educationQuerier.ReadAsync(query.GetWorldId(), query.Id, cancellationToken);
  }
}
