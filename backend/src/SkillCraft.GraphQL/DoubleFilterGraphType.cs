using GraphQL.Types;
using SkillCraft.Contracts;

namespace SkillCraft.GraphQL;

internal class DoubleFilterGraphType : InputObjectGraphType<DoubleFilter>
{
  public DoubleFilterGraphType()
  {
    Name = nameof(DoubleFilter);
    Description = "Represents a filter to match data on a numeric (Double) property.";

    Field(x => x.Values, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<IntGraphType>>>))
      .Description("The values compared to the property value of the entities. Each value should only be included once, and only one value should be included when not using the IN or NOT IN operator.");
    Field(x => x.Operator)
      .Description("The comparison operator (case-insensitive). Expected operators are: eq (equal to) (DEFAULT), ne (not equal to), gt (greater than), gte (greater than or equal to), lt (less than), lte (less than or equal to), in (in Values), and nin (not in Values).");
  }
}
