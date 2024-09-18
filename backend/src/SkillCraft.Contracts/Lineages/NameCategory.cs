namespace SkillCraft.Contracts.Lineages;

public record NameCategory
{
  public string Key { get; set; }
  public List<string> Values { get; set; }

  public NameCategory() : this(string.Empty, [])
  {
  }

  public NameCategory(string key, IEnumerable<string> values)
  {
    Key = key;
    Values = values.ToList();
  }
}
