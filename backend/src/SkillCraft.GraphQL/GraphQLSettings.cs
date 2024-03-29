﻿namespace SkillCraft.GraphQL;

public record GraphQLSettings : IGraphQLSettings
{
  public bool EnableMetrics { get; set; }
  public bool ExposeExceptionDetails { get; set; }
}
