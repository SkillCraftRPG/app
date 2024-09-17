using Logitar;

namespace SkillCraft.Domain.Lineages;

public record Names
{
  public string? Text { get; }
  public IReadOnlyCollection<string> Family { get; }
  public IReadOnlyCollection<string> Female { get; }
  public IReadOnlyCollection<string> Male { get; }
  public IReadOnlyCollection<string> Unisex { get; }
  public IReadOnlyDictionary<string, IReadOnlyCollection<string>> Custom { get; }

  public Names()
    : this(text: null, family: [], female: [], male: [], unisex: [], custom: new Dictionary<string, IReadOnlyCollection<string>>())
  {
  }

  [JsonConstructor]
  public Names(
    string? text,
    IReadOnlyCollection<string> family,
    IReadOnlyCollection<string> female,
    IReadOnlyCollection<string> male,
    IReadOnlyCollection<string> unisex,
    IReadOnlyDictionary<string, IReadOnlyCollection<string>> custom)
  {
    Text = text?.CleanTrim();
    Family = family.Distinct().OrderBy(x => x).ToArray().AsReadOnly();
    Female = female.Distinct().OrderBy(x => x).ToArray().AsReadOnly();
    Male = male.Distinct().OrderBy(x => x).ToArray().AsReadOnly();
    Unisex = unisex.Distinct().OrderBy(x => x).ToArray().AsReadOnly();

    Dictionary<string, List<string>> customNames = new(capacity: custom.Count);
    foreach (KeyValuePair<string, IReadOnlyCollection<string>> category in custom)
    {
      customNames[category.Key.Trim()] = [.. category.Value.Distinct().OrderBy(x => x)];
    }
    Custom = customNames.ToDictionary(x => x.Key, x => (IReadOnlyCollection<string>)x.Value.AsReadOnly()).AsReadOnly();
  }
}
