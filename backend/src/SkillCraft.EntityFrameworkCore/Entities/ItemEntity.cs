using Logitar.EventSourcing;
using SkillCraft.Contracts.Items;
using SkillCraft.Contracts.Items.Properties;
using SkillCraft.Domain.Items;

namespace SkillCraft.EntityFrameworkCore.Entities;

internal class ItemEntity : AggregateEntity
{
  public int ItemId { get; private set; }
  public Guid Id { get; private set; }

  public WorldEntity? World { get; private set; }
  public int WorldId { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? Description { get; private set; }

  public double? Value { get; private set; }
  public double? Weight { get; private set; }

  public ItemCategory Category { get; private set; }
  public string? Properties { get; private set; }

  public ItemEntity(WorldEntity world, Item.CreatedEvent @event) : base(@event)
  {
    Id = new ItemId(@event.AggregateId).EntityId;

    World = world;
    WorldId = world.WorldId;

    Name = @event.Name.Value;

    Category = @event.Category;
  }

  private ItemEntity() : base()
  {
  }

  public override IEnumerable<ActorId> GetActorIds()
  {
    List<ActorId> actorIds = base.GetActorIds().ToList();
    if (World != null)
    {
      actorIds.AddRange(World.GetActorIds());
    }
    return actorIds.AsReadOnly();
  }

  public ConsumablePropertiesModel GetConsumableProperties()
  {
    return new ConsumablePropertiesModel();
  }
  public ContainerPropertiesModel GetContainerProperties()
  {
    return new ContainerPropertiesModel();
  }
  public DevicePropertiesModel GetDeviceProperties()
  {
    return new DevicePropertiesModel();
  }
  public EquipmentPropertiesModel GetEquipmentProperties()
  {
    return new EquipmentPropertiesModel();
  }
  public MiscellaneousPropertiesModel GetMiscellaneousProperties()
  {
    return new MiscellaneousPropertiesModel();
  }
  public MoneyPropertiesModel GetMoneyProperties()
  {
    return new MoneyPropertiesModel();
  }
  public WeaponPropertiesModel GetWeaponProperties()
  {
    return new WeaponPropertiesModel();
  }

  public void SetProperties(Item.ConsumablePropertiesUpdatedEvent @event)
  {
    base.Update(@event);

    Properties = null;
  }
  public void SetProperties(Item.ContainerPropertiesUpdatedEvent @event)
  {
    base.Update(@event);

    Properties = null;
  }
  public void SetProperties(Item.DevicePropertiesUpdatedEvent @event)
  {
    base.Update(@event);

    Properties = null;
  }
  public void SetProperties(Item.EquipmentPropertiesUpdatedEvent @event)
  {
    base.Update(@event);

    Properties = null;
  }
  public void SetProperties(Item.MiscellaneousPropertiesUpdatedEvent @event)
  {
    base.Update(@event);

    Properties = null;
  }
  public void SetProperties(Item.MoneyPropertiesUpdatedEvent @event)
  {
    base.Update(@event);

    Properties = null;
  }
  public void SetProperties(Item.WeaponPropertiesUpdatedEvent @event)
  {
    base.Update(@event);

    Properties = null;
  }

  public void Update(Item.UpdatedEvent @event)
  {
    base.Update(@event);

    if (@event.Name != null)
    {
      Name = @event.Name.Value;
    }
    if (@event.Description != null)
    {
      Description = @event.Description.Value?.Value;
    }

    if (@event.Value != null)
    {
      Value = @event.Value.Value;
    }
    if (@event.Weight != null)
    {
      Weight = @event.Weight.Value;
    }
  }
}
