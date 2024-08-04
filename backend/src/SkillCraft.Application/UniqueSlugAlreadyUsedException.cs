using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;
using SkillCraft.Domain;

namespace SkillCraft.Application;

internal class UniqueSlugAlreadyUsedException : ConflictException
{
  private const string ErrorMessage = "The specified unique slug is already used.";

  public string UniqueSlug
  {
    get => (string)Data[nameof(UniqueSlug)]!;
    private set => Data[nameof(UniqueSlug)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error => new PropertyError(this.GetErrorCode(), ErrorMessage, UniqueSlug, PropertyName);

  public UniqueSlugAlreadyUsedException(SlugUnit uniqueSlug, string propertyName) : base(BuildMessage(uniqueSlug, propertyName))
  {
    UniqueSlug = uniqueSlug.Value;
    PropertyName = propertyName;
  }

  private static string BuildMessage(SlugUnit uniqueSlug, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(UniqueSlug), uniqueSlug.Value)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
