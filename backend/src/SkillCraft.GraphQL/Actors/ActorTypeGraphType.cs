using GraphQL.Types;
using Logitar.Portal.Contracts.Actors;

namespace SkillCraft.GraphQL.Actors;

internal class ActorTypeGraphType : EnumerationGraphType<ActorType>
{
  public ActorTypeGraphType()
  {
    Name = nameof(ActorType);
    Description = "The possible actor types.";

    AddValue(ActorType.ApiKey, "The actor is an API key.");
    AddValue(ActorType.System, "The actor is the system.");
    AddValue(ActorType.User, "The actor is an user.");
  }
  private void AddValue(ActorType value, string description)
  {
    Add(value.ToString(), value, description);
  }
}
