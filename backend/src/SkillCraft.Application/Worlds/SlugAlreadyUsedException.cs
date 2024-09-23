using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Worlds;

internal class SlugAlreadyUsedException : ConflictException
{
  private const string ErrorMessage = "The specified slug is already used.";

  public string Slug
  {
    get => (string)Data[nameof(Slug)]!;
    private set => Data[nameof(Slug)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }
  // TODO(fpion): ConflictId

  public override Error Error => new PropertyError(this.GetErrorCode(), ErrorMessage, Slug, PropertyName);
  // TODO(fpion): ConflictId

  public SlugAlreadyUsedException(World world) : base(BuildMessage(world))
  {
    Slug = world.Slug.Value;
    PropertyName = nameof(world.Slug);
  }

  private static string BuildMessage(World world) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(Slug), world.Slug)
    .AddData(nameof(PropertyName), nameof(world.Slug))
    .Build();
}
