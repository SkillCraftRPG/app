using SkillCraft.Contracts.Languages;

namespace SkillCraft.Contracts.Characters;

public class CharacterLanguageModel
{
  public LanguageModel Language { get; set; }

  public string? Notes { get; set; }

  public CharacterLanguageModel() : this(new LanguageModel())
  {
  }

  public CharacterLanguageModel(LanguageModel language)
  {
    Language = language;
  }

  public override bool Equals(object? obj) => obj is CharacterLanguageModel other && other.Language.Equals(Language);
  public override int GetHashCode() => Language.GetHashCode();
  public override string ToString() => Language.ToString();
}
