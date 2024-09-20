﻿using GraphQL.Types;
using SkillCraft.Contracts.Lineages;
using SkillCraft.GraphQL.Worlds;

namespace SkillCraft.GraphQL.Lineages;

internal class LineageGraphType : AggregateGraphType<LineageModel>
{
  public LineageGraphType() : base("Represents a character lineage.")
  {
    Field(x => x.Name)
      .Description("The display name of the lineage.");
    Field(x => x.Description)
      .Description("The description of the lineage.");

    Field(x => x.Attributes, type: typeof(NonNullGraphType<AttributeBonusesGraphType>))
      .Description("The attribute bonuses granted by this lineage.");
    Field(x => x.Features, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<FeatureGraphType>>>))
      .Description("The features granted to characters in this lineage.");

    Field(x => x.Languages, type: typeof(NonNullGraphType<LanguagesGraphType>))
      .Description("The languages spoken by this lineage.");
    Field(x => x.Names, type: typeof(NonNullGraphType<NamesGraphType>))
      .Description("The names given to individuals in this lineage.");

    Field(x => x.Speeds, type: typeof(NonNullGraphType<SpeedsGraphType>))
      .Description("The movement speeds of the individuals in this lineage.");
    Field(x => x.Size, type: typeof(NonNullGraphType<SizeGraphType>))
      .Description("The size parameters of this lineage.");
    Field(x => x.Weight, type: typeof(NonNullGraphType<WeightGraphType>))
      .Description("The weight parameters of this lineage.");
    Field(x => x.Ages, type: typeof(NonNullGraphType<AgesGraphType>))
      .Description("The age parameters of this lineage.");

    Field(x => x.World, type: typeof(NonNullGraphType<WorldGraphType>))
      .Description("The world in which the lineage resides.");
  }
}