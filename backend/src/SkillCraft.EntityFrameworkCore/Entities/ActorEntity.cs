using Logitar.Portal.Contracts.Actors;

namespace SkillCraft.EntityFrameworkCore.Entities;

internal class ActorEntity
{
  public int ActorId { get; private set; }

  public Guid Id { get; private set; }
  public ActorType Type { get; private set; }
  public bool IsDeleted { get; private set; }

  public string DisplayName { get; private set; } = string.Empty;
  public string? EmailAddress { get; private set; }
  public string? PictureUrl { get; private set; }

  public ActorEntity(Actor actor)
  {
    Id = actor.Id;
    Type = actor.Type;

    Update(actor);
  }

  private ActorEntity()
  {
  }

  public void Update(Actor actor)
  {
    if (actor.Id != Id || actor.Type != Type)
    {
      throw new ArgumentException($"The actor '{actor.Type}.Id={actor.Id}' was not expected. The expected actor is '{Type}.Id={Id}'.", nameof(actor));
    }

    DisplayName = actor.DisplayName;
    EmailAddress = actor.EmailAddress;
    PictureUrl = actor.PictureUrl;
  }
}
