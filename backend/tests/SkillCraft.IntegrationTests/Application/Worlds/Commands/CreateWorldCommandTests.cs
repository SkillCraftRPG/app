using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Application.Settings;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Worlds;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.Application.Worlds.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class CreateWorldCommandTests : IntegrationTests
{
  private readonly AccountSettings _accountSettings;

  public CreateWorldCommandTests() : base()
  {
    _accountSettings = ServiceProvider.GetRequiredService<AccountSettings>();
  }

  [Fact(DisplayName = "It should create a new world.")]
  public async Task It_should_create_a_new_world()
  {
    CreateWorldPayload payload = new("new-world")
    {
      Name = " New World ",
      Description = "    "
    };
    CreateWorldCommand command = new(payload);

    WorldModel model = await Pipeline.ExecuteAsync(command, CancellationToken);

    Assert.NotEqual(Guid.Empty, model.Id);
    Assert.Equal(2, model.Version);
    Assert.Equal(Actor, model.CreatedBy);
    Assert.Equal(Actor, model.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, model.CreatedOn, TimeSpan.FromSeconds(1));
    Assert.True(model.CreatedOn < model.UpdatedOn);

    Assert.Equal(payload.Slug, model.Slug);
    Assert.Equal(payload.Name.Trim(), model.Name);
    Assert.Null(model.Description);
    Assert.Equal(Actor, model.Owner);

    StorageSummaryEntity? summary = await SkillCraftContext.StorageSummaries.AsNoTracking().SingleOrDefaultAsync();
    Assert.NotNull(summary);
    Assert.Equal(model.Owner.Id, summary.OwnerId);
    Assert.Equal(_accountSettings.AllocatedBytes, summary.AllocatedBytes);
    Assert.NotNull(model.Name);
    long size = model.Slug.Length + model.Name.Length;
    Assert.Equal(size, summary.UsedBytes);
    long availableBytes = _accountSettings.AllocatedBytes - size;
    Assert.Equal(availableBytes, summary.AvailableBytes);

    StorageDetailEntity? detail = await SkillCraftContext.StorageDetails.AsNoTracking()
      .Include(x => x.World)
      .SingleOrDefaultAsync();
    Assert.NotNull(detail);
    Assert.NotNull(detail.World);
    Assert.Equal(model.Owner.Id, detail.OwnerId);
    Assert.Equal(model.Id, detail.World.Id);
    Assert.Equal(EntityType.World, detail.EntityType);
    Assert.Equal(model.Id, detail.EntityId);
    Assert.Equal(size, detail.Size);
  }
}
