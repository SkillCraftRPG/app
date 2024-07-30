using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Users;

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

  public ActorEntity(User user)
  {
    Id = user.Id;
    Type = ActorType.User;

    Update(user);
  }

  private ActorEntity()
  {
  }

  public void Update(User user)
  {
    if (user.Id != Id || Type != ActorType.User)
    {
      throw new ArgumentException($"The actor '{ActorType.User}.Id={user.Id}' was not expected. The expected actor is '{Type}.Id={Id}'.", nameof(user));
    }

    DisplayName = user.FullName ?? user.UniqueName;
    EmailAddress = user.Email?.Address;
    PictureUrl = user.Picture;
  }
}
