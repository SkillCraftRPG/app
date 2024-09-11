using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Application.Worlds.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class ReadWorldQueryTests : IntegrationTests
{
  public ReadWorldQueryTests() : base()
  {
  }

  [Fact(DisplayName = "It should return the correct world.")]
  public async Task It_should_return_the_correct_world()
  {
    ReadWorldQuery query = new(Id: null, "UngAr");

    WorldModel? world = await Pipeline.ExecuteAsync(query);
    Assert.NotNull(world);
    Assert.Equal("ungar", world.Slug, ignoreCase: true);
  }
}
