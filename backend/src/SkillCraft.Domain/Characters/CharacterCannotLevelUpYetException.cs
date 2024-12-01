using Logitar;
using Logitar.Portal.Contracts.Errors;

namespace SkillCraft.Domain.Characters;

public class CharacterCannotLevelUpYetException : DomainException
{
  private const string ErrorMessage = "The specified character cannot level-up yet.";

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
  public int CurrentExperience
  {
    get => (int)Data[nameof(CurrentExperience)]!;
    private set => Data[nameof(CurrentExperience)] = value;
  }
  public int CurrentLevel
  {
    get => (int)Data[nameof(CurrentLevel)]!;
    private set => Data[nameof(CurrentLevel)] = value;
  }
  public int? RequiredExperience
  {
    get => (int?)Data[nameof(RequiredExperience)];
    private set => Data[nameof(RequiredExperience)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.AddData(nameof(CurrentExperience), CurrentExperience.ToString());
      error.AddData(nameof(CurrentLevel), CurrentLevel.ToString());
      error.AddData(nameof(RequiredExperience), RequiredExperience?.ToString());
      return error;
    }
  }

  public CharacterCannotLevelUpYetException(Character character) : base(BuildMessage(character))
  {
    WorldId = character.WorldId.ToGuid();
    CharacterId = character.EntityId;
    CurrentExperience = character.Experience;
    CurrentLevel = character.Level;

    if (character.Level < 20)
    {
      RequiredExperience = ExperienceTable.GetTotalExperience(character.Level + 1);
    }
  }

  private static string BuildMessage(Character character) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), character.WorldId.ToGuid())
    .AddData(nameof(CharacterId), character.EntityId)
    .AddData(nameof(CurrentExperience), character.Experience)
    .AddData(nameof(CurrentLevel), character.Level)
    .AddData(nameof(RequiredExperience), character.Level < 20 ? ExperienceTable.GetTotalExperience(character.Level + 1) : null, "<null>")
    .Build();
}
