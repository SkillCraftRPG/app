using Microsoft.Extensions.Primitives;

namespace SkillCraft.Extensions;

internal static class StringValuesExtensions
{
  public static IReadOnlyCollection<string> GetNotNullOrWhiteSpace(this StringValues values)
  {
    List<string> cleanedValues = new(capacity: values.Count);

    foreach (string? value in values)
    {
      if (!string.IsNullOrWhiteSpace(value))
      {
        cleanedValues.Add(value.Trim());
      }
    }

    return cleanedValues.AsReadOnly();
  }
}
