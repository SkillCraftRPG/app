using Logitar.Portal.Contracts.Users;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain;

namespace SkillCraft.Application;

public abstract record Activity : IActivity
{
  [JsonIgnore]
  private ActivityContext? _context = null;
  [JsonIgnore]
  protected ActivityContext Context => _context ?? throw new InvalidOperationException($"The activity has not been contextualized. You must call the '{nameof(Contextualize)}' method once.");

  public virtual IActivity Anonymize()
  {
    return this;
  }

  public void Contextualize(ActivityContext context)
  {
    if (_context != null)
    {
      throw new InvalidOperationException($"The activity has already been contextualized. You may only call the '{nameof(Contextualize)}' method once.");
    }

    _context = context;
  }

  public User GetUser() => TryGetUser() ?? throw new InvalidOperationException("An authenticated user is required.");
  public UserId GetUserId() => new(GetUser().Id);
  public User? TryGetUser() => Context.User;

  public WorldModel GetWorld() => TryGetWorld() ?? throw new InvalidOperationException("The world is required.");
  public WorldModel? TryGetWorld() => Context.World;
}
