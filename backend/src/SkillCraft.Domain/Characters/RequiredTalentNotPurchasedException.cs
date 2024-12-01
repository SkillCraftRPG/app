using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;
using SkillCraft.Domain.Talents;

namespace SkillCraft.Domain.Characters;

public class RequiredTalentNotPurchasedException : DomainException
{
  private const string ErrorMessage = "The specified character did not purchase the required talent.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
  public Guid CharacterId
  {
    get => (Guid)Data[nameof(CharacterId)]!;
    private set => Data[nameof(CharacterId)] = value;
  }
  public Guid RequiringTalentId
  {
    get => (Guid)Data[nameof(RequiringTalentId)]!;
    private set => Data[nameof(RequiringTalentId)] = value;
  }
  public Guid RequiredTalentId
  {
    get => (Guid)Data[nameof(RequiredTalentId)]!;
    private set => Data[nameof(RequiredTalentId)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error
  {
    get
    {
      PropertyError error = new(this.GetErrorCode(), ErrorMessage, RequiringTalentId, PropertyName);
      error.AddData(nameof(RequiredTalentId), RequiredTalentId.ToString());
      return error;
    }
  }

  public RequiredTalentNotPurchasedException(Character character, Talent talent, string propertyName)
    : base(BuildMessage(character, talent, propertyName))
  {
    if (!talent.RequiredTalentId.HasValue)
    {
      throw new ArgumentException($"The {nameof(talent.RequiredTalentId)} is required.", nameof(talent));
    }

    WorldId = character.WorldId.ToGuid();
    CharacterId = character.EntityId;
    RequiringTalentId = talent.EntityId;
    RequiredTalentId = talent.RequiredTalentId.Value.EntityId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Character character, Talent talent, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), character.WorldId.ToGuid())
    .AddData(nameof(CharacterId), character.EntityId)
    .AddData(nameof(RequiringTalentId), talent.EntityId)
    .AddData(nameof(RequiredTalentId), talent.RequiredTalentId)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
