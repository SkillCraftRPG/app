using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;

namespace SkillCraft.GraphQL;

public class SkillCraftSchema : Schema
{
  public SkillCraftSchema(IServiceProvider serviceProvider) : base(serviceProvider)
  {
    Query = serviceProvider.GetRequiredService<RootQuery>();
  }
}
