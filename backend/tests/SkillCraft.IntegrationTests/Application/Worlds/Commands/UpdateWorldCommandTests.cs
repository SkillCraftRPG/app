using Logitar;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Worlds.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class UpdateWorldCommandTests : IntegrationTests
{
  private readonly IWorldRepository _worldRepository;

  public UpdateWorldCommandTests() : base()
  {
    _worldRepository = ServiceProvider.GetRequiredService<IWorldRepository>();
  }

  [Fact(DisplayName = "It should update an existing world.")]
  public async Task It_should_update_an_existing_world()
  {
    World world = Assert.Single(await _worldRepository.LoadAsync());

    UpdateWorldPayload payload = new()
    {
      Slug = "hyrule",
      Name = new Change<string>(" New World "),
      Description = new Change<string>("    ")
    };
    UpdateWorldCommand command = new(world.Id.ToGuid(), payload);

    WorldModel? model = await Pipeline.ExecuteAsync(command, CancellationToken);
    Assert.NotNull(model);

    Assert.Equal(world.Id.ToGuid(), model.Id);
    Assert.Equal(world.Version + 1, model.Version);
    Assert.Equal(Actor, model.CreatedBy);
    Assert.Equal(Actor, model.UpdatedBy);
    Assert.Equal(world.CreatedOn.AsUniversalTime(), model.CreatedOn);
    Assert.Equal(DateTime.UtcNow, model.UpdatedOn, TimeSpan.FromSeconds(1));

    Assert.Equal(payload.Slug, model.Slug);
    Assert.NotNull(payload.Name.Value);
    Assert.Equal(payload.Name.Value.Trim(), model.Name);
    Assert.Null(model.Description);
    Assert.Equal(Actor, model.Owner);
  }
}
