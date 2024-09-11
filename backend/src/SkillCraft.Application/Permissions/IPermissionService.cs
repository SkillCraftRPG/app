using SkillCraft.Domain;

namespace SkillCraft.Application.Permissions;

public interface IPermissionService
{
  Task EnsureCanCreateAsync(Activity activity, EntityType entityType, CancellationToken cancellationToken = default);
}
