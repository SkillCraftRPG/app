namespace SkillCraft.Application;

public static class DateTimeExtensions // ISSUE: https://github.com/SkillCraftRPG/app/issues/4
{
  public static DateTime AsUniversalTime(this DateTime value)
  {
    return value.Kind switch
    {
      DateTimeKind.Local => value.ToUniversalTime(),
      DateTimeKind.Unspecified => DateTime.SpecifyKind(value, DateTimeKind.Utc),
      DateTimeKind.Utc => value,
      _ => throw new ArgumentException($"The date time kind '{value.Kind}' is not supported.", nameof(value)),
    };
  }

  public static string ToISOString(this DateTime value) => value.AsUniversalTime().ToString("O", CultureInfo.InvariantCulture);
}
