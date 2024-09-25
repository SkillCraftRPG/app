using SkillCraft.Contracts;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Permissions;

public interface IPermissionService
{
  Task EnsureCanCommentAsync(Activity activity, EntityMetadata entity, CancellationToken cancellationToken = default);
  Task EnsureCanCommentAsync(Activity activity, WorldModel world, CancellationToken cancellationToken = default);

  Task EnsureCanCreateAsync(Activity activity, EntityType entityType, CancellationToken cancellationToken = default);

  Task EnsureCanPreviewAsync(Activity activity, EntityMetadata entity, CancellationToken cancellationToken = default);
  Task EnsureCanPreviewAsync(Activity activity, EntityType entityType, CancellationToken cancellationToken = default);
  Task EnsureCanPreviewAsync(Activity activity, WorldModel world, CancellationToken cancellationToken = default);

  Task EnsureCanUpdateAsync(Activity activity, EntityMetadata entity, CancellationToken cancellationToken = default);
  Task EnsureCanUpdateAsync(Activity activity, World world, CancellationToken cancellationToken = default);

  Task EnsureCanViewAsync(Activity activity, EntityMetadata entity, CancellationToken cancellationToken = default);
  Task EnsureCanViewAsync(Activity activity, WorldModel world, CancellationToken cancellationToken = default);
}
