using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;
using SkillCraft.Domain.Talents;

namespace SkillCraft.Domain.Characters;

internal class TalentTierCannotExceedCharacterTierException : DomainException
{
  private const string ErrorMessage = "The specified talent tier was greater than the character tier.";

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
  public int CharacterTier
  {
    get => (int)Data[nameof(CharacterTier)]!;
    private set => Data[nameof(CharacterTier)] = value;
  }
  public Guid TalentId
  {
    get => (Guid)Data[nameof(TalentId)]!;
    private set => Data[nameof(TalentId)] = value;
  }
  public int TalentTier
  {
    get => (int)Data[nameof(TalentTier)]!;
    private set => Data[nameof(TalentTier)] = value;
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
      PropertyError error = new(this.GetErrorCode(), ErrorMessage, TalentId, PropertyName);
      error.AddData(nameof(CharacterTier), CharacterTier.ToString());
      error.AddData(nameof(TalentTier), TalentTier.ToString());
      return error;
    }
  }

  public TalentTierCannotExceedCharacterTierException(Character character, Talent talent, string propertyName)
    : base(BuildMessage(character, talent, propertyName))
  {
    WorldId = character.WorldId.ToGuid();
    CharacterId = character.EntityId;
    CharacterTier = character.Tier;
    TalentId = talent.EntityId;
    TalentTier = talent.Tier;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Character character, Talent talent, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), character.WorldId.ToGuid())
    .AddData(nameof(CharacterId), character.EntityId)
    .AddData(nameof(CharacterTier), character.Tier)
    .AddData(nameof(TalentId), talent.EntityId)
    .AddData(nameof(TalentTier), talent.Tier)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
