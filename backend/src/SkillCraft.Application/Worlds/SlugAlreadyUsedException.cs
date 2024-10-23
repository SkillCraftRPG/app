using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Worlds;

internal class SlugAlreadyUsedException : ConflictException
{
  private const string ErrorMessage = "The specified slug is already used.";

  public IEnumerable<Guid> ConflictingIds
  {
    get => (IEnumerable<Guid>)Data[nameof(ConflictingIds)]!;
    private set => Data[nameof(ConflictingIds)] = value;
  }
  public string Slug
  {
    get => (string)Data[nameof(Slug)]!;
    private set => Data[nameof(Slug)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error => new PropertyError(this.GetErrorCode(), ErrorMessage, Slug, PropertyName);

  public SlugAlreadyUsedException(World world, WorldId conflictId, string? propertyName = null)
    : base(BuildMessage(world, conflictId, propertyName))
  {
    ConflictingIds = [world.Id.ToGuid(), conflictId.ToGuid()];
    Slug = world.Slug.Value;
    PropertyName = nameof(world.Slug);
  }

  private static string BuildMessage(World world, WorldId conflictId, string? propertyName)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append(nameof(Slug)).Append(": ").Append(world.Slug).AppendLine();
    message.Append(nameof(PropertyName)).Append(": ").AppendLine(propertyName);
    message.Append(nameof(ConflictingIds)).Append(':').AppendLine();
    message.Append(" - ").Append(world.Id.ToGuid()).AppendLine();
    message.Append(" - ").Append(conflictId.ToGuid()).AppendLine();

    return message.ToString();
  }
}
