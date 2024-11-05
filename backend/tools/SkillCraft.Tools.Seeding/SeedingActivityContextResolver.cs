using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Users;
using SkillCraft.Application;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain;
using SkillCraft.Domain.Worlds;
using ActivityContext = SkillCraft.Application.ActivityContext;

namespace SkillCraft.Tools.Seeding;

internal class SeedingActivityContextResolver : IActivityContextResolver
{
  private const string UserIdKey = nameof(UserId);
  private const string WorldIdKey = nameof(WorldId);

  private readonly ActivityContext _context;

  public SeedingActivityContextResolver(IConfiguration configuration)
  {
    string userId = configuration.GetValue<string>(UserIdKey) ?? throw new InvalidOperationException($"The configuration '{UserIdKey}' is required.");
    string worldId = configuration.GetValue<string>(WorldIdKey) ?? throw new InvalidOperationException($"The configuration '{WorldIdKey}' is required.");
    User user = new()
    {
      Id = Guid.Parse(userId)
    };
    WorldModel world = new()
    {
      Id = Guid.Parse(worldId),
      Owner = new Actor(user)
    };
    _context = new(ApiKey: null, Session: null, user, world);
  }

  public Task<ActivityContext> ResolveAsync(CancellationToken cancellationToken)
  {
    return Task.FromResult(_context);
  }
}
