using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;
using SkillCraft.Domain.Personalities;

namespace SkillCraft.Application.Personalities;

internal class PersonalityNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified personality could not be found.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
  public Guid PersonalityId
  {
    get => (Guid)Data[nameof(PersonalityId)]!;
    private set => Data[nameof(PersonalityId)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error => new PropertyError(this.GetErrorCode(), ErrorMessage, PersonalityId, PropertyName);

  public PersonalityNotFoundException(PersonalityId personalityId, string propertyName)
    : base(BuildMessage(personalityId, propertyName))
  {
    WorldId = personalityId.WorldId.ToGuid();
    PersonalityId = personalityId.EntityId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(PersonalityId personalityId, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), personalityId.WorldId.ToGuid())
    .AddData(nameof(PersonalityId), personalityId.EntityId)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
