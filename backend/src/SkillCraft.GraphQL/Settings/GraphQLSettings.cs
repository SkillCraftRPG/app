namespace SkillCraft.GraphQL.Settings;

public record GraphQLSettings
{
  public const string SectionKey = "GraphQL";

  public bool EnableMetrics { get; set; }
  public bool ExposeExceptionDetails { get; set; }
}
