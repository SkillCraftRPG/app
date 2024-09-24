using SkillCraft.Domain;

namespace SkillCraft.Application;

[Trait(Traits.Category, Categories.Unit)]
public class EntityExtensionsTests
{
  [Theory(DisplayName = "IsGameEntity: it should return false when the entity is not a game entity.")]
  [InlineData(EntityType.Comment)]
  public void IsGameEntity_it_should_return_false_when_the_entity_is_not_a_game_entity(EntityType type)
  {
    Assert.False(type.IsGameEntity());
  }

  [Theory(DisplayName = "IsGameEntity: it should return true when the entity is not a game entity.")]
  [InlineData(EntityType.Aspect)]
  [InlineData(EntityType.Caste)]
  [InlineData(EntityType.Character)]
  [InlineData(EntityType.Customization)]
  [InlineData(EntityType.Education)]
  [InlineData(EntityType.Language)]
  [InlineData(EntityType.Lineage)]
  [InlineData(EntityType.Party)]
  [InlineData(EntityType.Personality)]
  [InlineData(EntityType.Talent)]
  [InlineData(EntityType.World)]
  public void IsGameEntity_it_should_return_true_when_the_entity_is_not_a_game_entity(EntityType type)
  {
    Assert.True(type.IsGameEntity());
  }
}
