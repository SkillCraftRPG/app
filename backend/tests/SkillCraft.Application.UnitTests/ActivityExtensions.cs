using Bogus;
using Logitar;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Users;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application;

internal static class ActivityExtensions
{
  private static readonly Faker _faker = new();

  public static void Contextualize(this Activity activity) => activity.Contextualize(new UserMock());

  public static void Contextualize(this Activity activity, User user)
  {
    ActivityContext context = new(ApiKey: null, Session: null, user, World: null);
    activity.Contextualize(context);
  }

  public static void Contextualize(this Activity activity, User user, World? world)
  {
    WorldModel? model = ToModel(user, world);
    activity.Contextualize(user, model);
  }
  private static WorldModel? ToModel(User user, World? world)
  {
    if (world == null)
    {
      return null;
    }

    Dictionary<ActorId, Actor> actors = [];
    actors[new ActorId(user.Id)] = new Actor(user);

    if (world.OwnerId.ToGuid() != user.Id)
    {
      ActorId id = world.OwnerId.ActorId;
      actors[id] = GenerateActor(id);
    }
    if (world.CreatedBy.ToGuid() != user.Id)
    {
      ActorId id = world.CreatedBy;
      actors[id] = GenerateActor(id);
    }
    if (world.UpdatedBy.ToGuid() != user.Id)
    {
      ActorId id = world.UpdatedBy;
      actors[id] = GenerateActor(id);
    }

    return new WorldModel
    {
      CreatedBy = actors[world.CreatedBy],
      CreatedOn = world.CreatedOn.AsUniversalTime(),
      Description = world.Description?.Value,
      Id = world.Id.ToGuid(),
      Name = world.Name?.Value,
      Owner = actors[world.OwnerId.ActorId],
      Slug = world.Slug.Value,
      UpdatedBy = actors[world.UpdatedBy],
      UpdatedOn = world.UpdatedOn.AsUniversalTime(),
      Version = world.Version
    };
  }
  private static Actor GenerateActor(ActorId id) => new(_faker.Name.FullName())
  {
    Id = id.ToGuid(),
    Type = ActorType.User,
    EmailAddress = _faker.Internet.Email(),
    PictureUrl = _faker.Internet.Avatar()
  };

  public static void Contextualize(this Activity activity, User user, WorldModel? world)
  {
    ActivityContext context = new(ApiKey: null, Session: null, user, world);
    activity.Contextualize(context);
  }
}
