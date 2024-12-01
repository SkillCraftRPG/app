using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Errors;

namespace SkillCraft.Domain.Characters;

public class SkillMaximumRankReachedException : DomainException
{
  private const string ErrorMessage = "The specified character skill has reached its maximum rank.";

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
  public int MaximumSkillRank
  {
    get => (int)Data[nameof(MaximumSkillRank)]!;
    private set => Data[nameof(MaximumSkillRank)] = value;
  }
  public Skill Skill
  {
    get => (Skill)Data[nameof(Skill)]!;
    private set => Data[nameof(Skill)] = value;
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
      PropertyError error = new(this.GetErrorCode(), ErrorMessage, Skill, PropertyName); ;
      error.AddData(nameof(MaximumSkillRank), MaximumSkillRank.ToString());
      return error;
    }
  }

  public SkillMaximumRankReachedException(Character character, Skill skill, string propertyName)
    : base(BuildMessage(character, skill, propertyName))
  {
    WorldId = character.WorldId.ToGuid();
    CharacterId = character.EntityId;
    MaximumSkillRank = character.MaximumSkillRank;
    Skill = skill;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Character character, Skill skill, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), character.WorldId.ToGuid())
    .AddData(nameof(CharacterId), character.EntityId)
    .AddData(nameof(MaximumSkillRank), character.MaximumSkillRank)
    .AddData(nameof(Skill), skill)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
