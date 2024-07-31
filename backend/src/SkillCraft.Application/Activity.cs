using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Users;

namespace SkillCraft.Application;

public abstract record Activity : IActivity
{
  [JsonIgnore]
  private ActivityContext? _context = null;
  [JsonIgnore]
  protected ActivityContext Context => _context ?? throw new InvalidOperationException($"The activity has not been contextualized. You must call the '{nameof(Contextualize)}' method once.");

  [JsonIgnore]
  public Actor Actor
  {
    get
    {
      if (Context.User != null)
      {
        return new Actor(Context.User);
      }
      else if (Context.ApiKey != null)
      {
        return new Actor(Context.ApiKey);
      }

      return Actor.System;
    }
  }
  [JsonIgnore]
  public ActorId ActorId => new(Actor.Id);

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
  public User? TryGetUser() => Context.User;
}
