using Logitar.EventSourcing;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.EntityFrameworkCore.Entities;

internal class WorldEntity : AggregateEntity
{
  public int WorldId { get; private set; }
  public Guid Id { get; private set; }

  public UserEntity? Owner { get; private set; }
  public int UserId { get; private set; }
  public Guid OwnerId { get; private set; }

  public string Slug { get; private set; } = string.Empty;
  public string SlugNormalized
  {
    get => SkillCraftDb.Helper.Normalize(Slug);
    private set { }
  }
  public string? Name { get; private set; }
  public string? Description { get; private set; }

  public WorldEntity(UserEntity owner, World.CreatedEvent @event) : base(@event)
  {
    Id = @event.AggregateId.ToGuid();

    Owner = owner;
    UserId = owner.UserId;
    OwnerId = owner.Id;

    Slug = @event.Slug.Value;
  }

  private WorldEntity() : base()
  {
  }

  public override IEnumerable<ActorId> GetActorIds() => base.GetActorIds().Concat([new(OwnerId)]);

  public void Update(World.UpdatedEvent @event)
  {
    if (@event.Slug != null)
    {
      Slug = @event.Slug.Value;
    }
    if (@event.Name != null)
    {
      Name = @event.Name.Value?.Value;
    }
    if (@event.Description != null)
    {
      Description = @event.Description.Value?.Value;
    }
  }
}
