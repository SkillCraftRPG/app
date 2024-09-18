using SkillCraft.Contracts.Languages;

namespace SkillCraft.Contracts.Lineages;

public record LanguagesModel
{
  public List<LanguageModel> Items { get; set; }
  public int Extra { get; set; }
  public string? Text { get; set; }

  public LanguagesModel() : this([])
  {
  }

  public LanguagesModel(IEnumerable<LanguageModel> items)
  {
    Items = items.ToList();
  }
}
