﻿using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Permissions;

public interface IPermissionService
{
  Task EnsureCanCreateAsync(Activity activity, EntityType entityType, CancellationToken cancellationToken = default);
  Task EnsureCanPreviewAsync(Activity activity, WorldModel world, CancellationToken cancellationToken = default);
  Task EnsureCanUpdateAsync(Activity activity, World world, CancellationToken cancellationToken = default);
}
