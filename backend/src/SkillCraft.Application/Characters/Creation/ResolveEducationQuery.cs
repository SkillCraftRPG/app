﻿using MediatR;
using SkillCraft.Application.Educations;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain.Educations;

namespace SkillCraft.Application.Characters.Creation;

internal record ResolveEducationQuery(Activity Activity, Guid Id) : IRequest<Education>;

internal class ResolveEducationQueryHandler : IRequestHandler<ResolveEducationQuery, Education>
{
  private readonly IEducationRepository _educationRepository;
  private readonly IPermissionService _permissionService;

  public ResolveEducationQueryHandler(IEducationRepository educationRepository, IPermissionService permissionService)
  {
    _educationRepository = educationRepository;
    _permissionService = permissionService;
  }

  public async Task<Education> Handle(ResolveEducationQuery query, CancellationToken cancellationToken)
  {
    Activity activity = query.Activity;

    EducationId id = new(activity.GetWorldId(), query.Id);
    Education education = await _educationRepository.LoadAsync(id, cancellationToken)
      ?? throw new AggregateNotFoundException<Education>(id.AggregateId, nameof(CreateCharacterPayload.EducationId));

    await _permissionService.EnsureCanPreviewAsync(activity, education.GetMetadata(), cancellationToken);

    return education;
  }
}
