namespace SkillCraft.GraphQL;

public interface IGraphQLSettings
{
  bool EnableMetrics { get; }
  bool ExposeExceptionDetails { get; }
}
