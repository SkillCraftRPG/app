namespace SkillCraft.Constants;

internal static class Schemes
{
  public const string Bearer = nameof(Bearer);
  public const string Session = nameof(Session);

  public static IReadOnlyCollection<string> All => [Bearer, Session];
}
