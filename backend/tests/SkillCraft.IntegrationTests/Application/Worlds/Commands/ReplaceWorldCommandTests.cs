using Logitar;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Worlds.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class ReplaceWorldCommandTests : IntegrationTests
{
  private readonly IWorldRepository _worldRepository;

  public ReplaceWorldCommandTests() : base()
  {
    _worldRepository = ServiceProvider.GetRequiredService<IWorldRepository>();
  }

  [Fact(DisplayName = "It should replace an existing world.")]
  public async Task It_should_replace_an_existingworld()
  {
    World world = Assert.Single(await _worldRepository.LoadAsync());
    long version = world.Version;

    Description description = new("This is the new world.");
    world.Description = description;
    world.Update(world.OwnerId);
    await _worldRepository.SaveAsync(world);

    ReplaceWorldPayload payload = new("hyrule")
    {
      Name = " New World ",
      Description = "    "
    };
    ReplaceWorldCommand command = new(world.Id.ToGuid(), payload, version);

    WorldModel? model = await Pipeline.ExecuteAsync(command, CancellationToken);
    Assert.NotNull(model);

    Assert.Equal(world.Id.ToGuid(), model.Id);
    Assert.Equal(world.Version + 1, model.Version);
    Assert.Equal(Actor, model.CreatedBy);
    Assert.Equal(Actor, model.UpdatedBy);
    Assert.Equal(world.CreatedOn.AsUniversalTime(), model.CreatedOn);
    Assert.Equal(DateTime.UtcNow, model.UpdatedOn, TimeSpan.FromSeconds(1));

    Assert.Equal(payload.Slug, model.Slug);
    Assert.Equal(payload.Name.Trim(), model.Name);
    Assert.Equal(description.Value, model.Description);
    Assert.Equal(Actor, model.Owner);
  }
}
