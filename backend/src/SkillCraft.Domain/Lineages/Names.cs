using FluentValidation;
using Logitar;
using SkillCraft.Domain.Lineages.Validators;

namespace SkillCraft.Domain.Lineages;

public record Names
{
  public const int MaximumLength = byte.MaxValue;

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
    Family = Clean(family);
    Female = Clean(female);
    Male = Clean(male);
    Unisex = Clean(unisex);

    Dictionary<string, List<string>> customNames = new(capacity: custom.Count);
    foreach (KeyValuePair<string, IReadOnlyCollection<string>> category in custom)
    {
      IReadOnlyCollection<string> values = Clean(category.Value);
      if (values.Count > 0)
      {
        customNames[category.Key.Trim()] = [.. values];
      }
    }
    Custom = customNames.ToDictionary(x => x.Key, x => (IReadOnlyCollection<string>)x.Value.AsReadOnly()).AsReadOnly();

    new NamesValidator().ValidateAndThrow(this);
  }

  private static IReadOnlyCollection<string> Clean(IEnumerable<string> names) => names
    .Where(name => !string.IsNullOrWhiteSpace(name))
    .Select(name => name.Trim())
    .Distinct()
    .OrderBy(name => name)
    .ToArray()
    .AsReadOnly();
}
