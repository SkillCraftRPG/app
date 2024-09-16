using SkillCraft.Contracts.Languages;

namespace SkillCraft.Contracts.Speciez;

public record LanguagesModel
{
  public List<LanguageModel> Languages { get; set; } = [];
  public int Extra { get; set; }
  public string? Text { get; set; }

  public LanguagesModel() : this([])
  {
  }

  public LanguagesModel(IEnumerable<LanguageModel> languages)
  {
    Languages = languages.ToList();
  }
}
