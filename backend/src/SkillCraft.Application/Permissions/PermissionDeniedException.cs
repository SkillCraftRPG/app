using Logitar;
using Logitar.Portal.Contracts.Errors;
using Logitar.Portal.Contracts.Users;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Application.Permissions;

internal class PermissionDeniedException : ForbiddenException
{
  private const string ErrorMessage = "The specified permission was denied to the current user.";

  public Action Action
  {
    get => (Action)Data[nameof(Action)]!;
    private set => Data[nameof(Action)] = value;
  }
  public EntityType EntityType
  {
    get => (EntityType)Data[nameof(EntityType)]!;
    private set => Data[nameof(EntityType)] = value;
  }
  public Guid UserId
  {
    get => (Guid)Data[nameof(UserId)]!;
    private set => Data[nameof(UserId)] = value;
  }
  public Guid? WorldId
  {
    get => (Guid?)Data[nameof(WorldId)];
    private set => Data[nameof(WorldId)] = value;
  }
  public Guid? EntityId
  {
    get => (Guid?)Data[nameof(EntityId)];
    private set => Data[nameof(EntityId)] = value;
  }

  public override Error Error => new(this.GetErrorCode(), ErrorMessage);

  public PermissionDeniedException(Action action, EntityType entityType, User user, WorldModel? world = null, Guid? entityId = null)
    : base(BuildMessage(action, entityType, user, world, entityId))
  {
    Action = action;
    EntityType = entityType;
    UserId = user.Id;
    WorldId = world?.Id;
    EntityId = entityId;
  }

  private static string BuildMessage(Action action, EntityType entityType, User user, WorldModel? world, Guid? entityId) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(Action), action)
    .AddData(nameof(EntityType), entityType)
    .AddData(nameof(UserId), user.Id)
    .AddData(nameof(WorldId), world?.Id, "<null>")
    .AddData(nameof(EntityId), entityId, "<null>")
    .Build();
}

