using GraphQL.Types;
using SkillCraft.Contracts;

namespace SkillCraft.GraphQL;

internal class EntityTypeGraphType : EnumerationGraphType<EntityType>
{
  public EntityTypeGraphType()
  {
    Name = nameof(EntityType);
    Description = "Represents the available entity types.";

    AddValue(EntityType.Aspect, "The entity is a character aspect.");
    AddValue(EntityType.Caste, "The entity is a character caste.");
    AddValue(EntityType.Character, "The entity is a character.");
    AddValue(EntityType.Comment, "The entity is a comment.");
    AddValue(EntityType.Customization, "The entity is a character customization.");
    AddValue(EntityType.Education, "The entity is a character education.");
    AddValue(EntityType.Item, "The entity is a character item.");
    AddValue(EntityType.Language, "The entity is a character language.");
    AddValue(EntityType.Lineage, "The entity is a character lineage.");
    AddValue(EntityType.Nature, "The entity is a character nature.");
    AddValue(EntityType.Party, "The entity is a character party.");
    AddValue(EntityType.Talent, "The entity is a character talent.");
    AddValue(EntityType.World, "The entity is a game world.");
  }
  private void AddValue(EntityType value, string description)
  {
    Add(value.ToString(), value, description);
  }
}
