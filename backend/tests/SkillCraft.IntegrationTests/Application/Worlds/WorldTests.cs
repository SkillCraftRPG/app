﻿using Logitar;
using Logitar.Portal.Contracts.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Application.Worlds.Commands;
using SkillCraft.Application.Worlds.Queries;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Worlds;

[Trait(Traits.Category, Categories.Integration)]
public class WorldTests : IntegrationTests
{
  private readonly IWorldRepository _worldRepository;

  private readonly World _hyrule;
  private readonly World _lorule;
  private readonly World _newWorld;

  public WorldTests() : base()
  {
    _worldRepository = ServiceProvider.GetRequiredService<IWorldRepository>();

    _hyrule = new(new Slug("hyrule"), UserId);
    _lorule = new(new Slug("lorule"), UserId);
    _newWorld = new(new Slug("new-world"), UserId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _worldRepository.SaveAsync([_hyrule, _lorule, _newWorld]);
  }

  [Fact(DisplayName = "It should create a new world.")]
  public async Task It_should_create_a_new_world()
  {
    CreateOrReplaceWorldPayload payload = new("old-world")
    {
      Name = " The Old World ",
      Description = "    "
    };

    CreateOrReplaceWorldCommand command = new(Guid.NewGuid(), payload, Version: null);
    CreateOrReplaceWorldResult result = await Pipeline.ExecuteAsync(command);
    Assert.True(result.Created);

    WorldModel? world = result.World;
    Assert.NotNull(world);
    Assert.Equal(command.Id, world.Id);
    Assert.Equal(2, world.Version);
    Assert.Equal(DateTime.UtcNow, world.UpdatedOn, TimeSpan.FromSeconds(1));
    Assert.True(world.CreatedOn < world.UpdatedOn);
    Assert.Equal(Actor, world.CreatedBy);
    Assert.Equal(world.CreatedBy, world.UpdatedBy);

    Assert.Equal(Actor, world.Owner);
    Assert.Equal(payload.Slug, world.Slug);
    Assert.Equal(payload.Name?.CleanTrim(), world.Name);
    Assert.Equal(payload.Description?.CleanTrim(), world.Description);

    Assert.NotNull(await SkillCraftContext.Worlds.AsNoTracking().SingleOrDefaultAsync(x => x.Id == world.Id));
  }

  [Fact(DisplayName = "It should replace an existing world.")]
  public async Task It_should_replace_an_existing_world()
  {
    long version = World.Version;

    Description description = new("This is the world of Ungar.");
    World.Description = description;
    World.Update(UserId);
    await _worldRepository.SaveAsync(World);

    CreateOrReplaceWorldPayload payload = new("ungar")
    {
      Name = " Ungar ",
      Description = "    "
    };

    CreateOrReplaceWorldCommand command = new(World.EntityId, payload, version);
    CreateOrReplaceWorldResult result = await Pipeline.ExecuteAsync(command);
    Assert.False(result.Created);

    WorldModel? world = result.World;
    Assert.NotNull(world);
    Assert.Equal(command.Id, world.Id);
    Assert.Equal(World.Version + 1, world.Version);
    Assert.Equal(DateTime.UtcNow, world.UpdatedOn, TimeSpan.FromSeconds(1));
    Assert.Equal(Actor, world.UpdatedBy);

    Assert.Equal(Actor, world.Owner);
    Assert.Equal(payload.Slug, world.Slug);
    Assert.Equal(payload.Name.CleanTrim(), world.Name);
    Assert.Equal(description.Value, world.Description);
  }

  [Fact(DisplayName = "It should return empty search results.")]
  public async Task It_should_return_empty_search_results()
  {
    SearchWorldsPayload payload = new();
    payload.Search.Terms.Add(new SearchTerm("test"));

    SearchWorldsQuery query = new(payload);
    SearchResults<WorldModel> results = await Pipeline.ExecuteAsync(query);

    Assert.Equal(0, results.Total);
    Assert.Empty(results.Items);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    SearchWorldsPayload payload = new()
    {
      Skip = 1,
      Limit = 1
    };
    payload.Search.Operator = SearchOperator.Or;
    payload.Search.Terms.Add(new SearchTerm("%rule"));
    payload.Search.Terms.Add(new SearchTerm("ungar"));
    payload.Sort.Add(new WorldSortOption(WorldSort.Slug, isDescending: true));

    payload.Ids.Add(Guid.Empty);
    payload.Ids.AddRange((await _worldRepository.LoadAsync()).Select(world => world.EntityId));
    payload.Ids.Remove(World.EntityId);

    SearchWorldsQuery query = new(payload);
    SearchResults<WorldModel> results = await Pipeline.ExecuteAsync(query);

    Assert.Equal(2, results.Total);
    WorldModel world = Assert.Single(results.Items);
    Assert.Equal(_hyrule.EntityId, world.Id);
  }

  [Fact(DisplayName = "It should return the world found by ID.")]
  public async Task It_should_return_the_world_found_by_Id()
  {
    ReadWorldQuery query = new(World.EntityId, Slug: null);
    WorldModel? world = await Pipeline.ExecuteAsync(query);
    Assert.NotNull(world);
    Assert.Equal(World.EntityId, world.Id);
  }

  [Fact(DisplayName = "It should throw SlugAlreadyUsedException when the slug is already used.")]
  public async Task It_should_throw_SlugAlreadyUsedException_when_the_slug_is_already_used()
  {
    CreateOrReplaceWorldPayload payload = new(World.Slug.Value);
    CreateOrReplaceWorldCommand command = new(Id: null, payload, Version: null);
    var exception = await Assert.ThrowsAsync<SlugAlreadyUsedException>(async () => await Pipeline.ExecuteAsync(command));
    Assert.Contains(World.EntityId, exception.ConflictingIds);
    Assert.Equal(payload.Slug, exception.Slug);
    Assert.Equal("Slug", exception.PropertyName);
  }

  [Fact(DisplayName = "It should update an existing world.")]
  public async Task It_should_update_an_existing_world()
  {
    UpdateWorldPayload payload = new()
    {
      Name = new Change<string>("Ungar")
    };

    UpdateWorldCommand command = new(World.EntityId, payload);
    WorldModel? world = await Pipeline.ExecuteAsync(command);

    Assert.NotNull(world);
    Assert.Equal(command.Id, world.Id);
    Assert.Equal(2, world.Version);
    Assert.Equal(DateTime.UtcNow, world.UpdatedOn, TimeSpan.FromSeconds(1));
    Assert.Equal(Actor, world.UpdatedBy);

    Assert.Equal(Actor, world.Owner);
    Assert.Equal(World.Slug.Value, world.Slug);
    Assert.Equal(payload.Name.Value?.CleanTrim(), world.Name);
    Assert.Null(world.Description);
  }
}
