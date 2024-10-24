﻿using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Personalities;

namespace SkillCraft.Application.Personalities.Queries;

/// <exception cref="PermissionDeniedException"></exception>
public record ReadPersonalityQuery(Guid Id) : Activity, IRequest<PersonalityModel?>;

internal class ReadPersonalityQueryHandler : IRequestHandler<ReadPersonalityQuery, PersonalityModel?>
{
  private readonly IPermissionService _permissionService;
  private readonly IPersonalityQuerier _personalityQuerier;

  public ReadPersonalityQueryHandler(IPermissionService permissionService, IPersonalityQuerier personalityQuerier)
  {
    _permissionService = permissionService;
    _personalityQuerier = personalityQuerier;
  }

  public async Task<PersonalityModel?> Handle(ReadPersonalityQuery query, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanPreviewAsync(query, EntityType.Personality, cancellationToken);

    return await _personalityQuerier.ReadAsync(query.GetWorldId(), query.Id);
  }
}
