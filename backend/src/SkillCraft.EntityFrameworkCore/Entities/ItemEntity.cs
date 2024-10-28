using Logitar.EventSourcing;
using SkillCraft.Contracts.Items;
using SkillCraft.Contracts.Items.Properties;
using SkillCraft.Domain.Items;

namespace SkillCraft.EntityFrameworkCore.Entities;

internal class ItemEntity : AggregateEntity
{
  private const char TraitSeparator = ',';

  public int ItemId { get; private set; }
  public Guid Id { get; private set; }

  public WorldEntity? World { get; private set; }
  public int WorldId { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? Description { get; private set; }

  public double? Value { get; private set; }
  public double? Weight { get; private set; }

  public bool IsAttunementRequired { get; private set; }

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
    IReadOnlyDictionary<string, string> properties = DeserializeProperties();
    return new ConsumablePropertiesModel
    {
      Charges = properties.TryGetValue(nameof(IConsumableProperties.Charges), out string? charges) ? int.Parse(charges) : null,
      RemoveWhenEmpty = bool.Parse(properties[nameof(IConsumableProperties.RemoveWhenEmpty)]),
      ReplaceWithItemWhenEmptyId = properties.TryGetValue(nameof(IConsumableProperties.ReplaceWithItemWhenEmptyId), out string? replaceWithItemWhenEmptyId) ? Guid.Parse(replaceWithItemWhenEmptyId) : null
    };
  }
  public ContainerPropertiesModel GetContainerProperties()
  {
    IReadOnlyDictionary<string, string> properties = DeserializeProperties();
    return new ContainerPropertiesModel
    {
      Capacity = properties.TryGetValue(nameof(IContainerProperties.Capacity), out string? capacity) ? double.Parse(capacity) : null,
      Volume = properties.TryGetValue(nameof(IContainerProperties.Volume), out string? volume) ? double.Parse(volume) : null
    };
  }
  public DevicePropertiesModel GetDeviceProperties()
  {
    return new DevicePropertiesModel();
  }
  public EquipmentPropertiesModel GetEquipmentProperties()
  {
    IReadOnlyDictionary<string, string> properties = DeserializeProperties();
    return new EquipmentPropertiesModel
    {
      Defense = int.Parse(properties[nameof(IEquipmentProperties.Defense)]),
      Resistance = properties.TryGetValue(nameof(IEquipmentProperties.Resistance), out string? resistance) ? int.Parse(resistance) : null,
      Traits = properties.TryGetValue(nameof(IEquipmentProperties.Traits), out string? traits) ? traits.Split(TraitSeparator).Select(Enum.Parse<EquipmentTrait>).ToArray() : []
    };
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
    return new WeaponPropertiesModel(); // TODO(fpion): implement
  }
  private IReadOnlyDictionary<string, string> DeserializeProperties()
  {
    Dictionary<string, string>? properties = null;
    if (Properties != null)
    {
      properties = JsonSerializer.Deserialize<Dictionary<string, string>>(Properties);
    }
    return (properties ?? []).AsReadOnly();
  }

  public void SetProperties(Item.ConsumablePropertiesUpdatedEvent @event)
  {
    base.Update(@event);

    Dictionary<string, string> properties = new(capacity: 3);
    if (@event.Properties.Charges.HasValue)
    {
      properties[nameof(IConsumableProperties.Charges)] = @event.Properties.Charges.Value.ToString();
    }
    properties[nameof(IConsumableProperties.RemoveWhenEmpty)] = @event.Properties.RemoveWhenEmpty.ToString();
    if (@event.Properties.ReplaceWithItemWhenEmptyId.HasValue)
    {
      properties[nameof(IConsumableProperties.ReplaceWithItemWhenEmptyId)] = @event.Properties.ReplaceWithItemWhenEmptyId.Value.ToString();
    }
    Properties = SerializeProperties(properties);
  }
  public void SetProperties(Item.ContainerPropertiesUpdatedEvent @event)
  {
    base.Update(@event);

    Dictionary<string, string> properties = new(capacity: 2);
    if (@event.Properties.Capacity.HasValue)
    {
      properties[nameof(IContainerProperties.Capacity)] = @event.Properties.Capacity.Value.ToString();
    }
    if (@event.Properties.Volume.HasValue)
    {
      properties[nameof(IContainerProperties.Volume)] = @event.Properties.Volume.Value.ToString();
    }
    Properties = SerializeProperties(properties);
  }
  public void SetProperties(Item.DevicePropertiesUpdatedEvent @event)
  {
    base.Update(@event);

    Properties = null;
  }
  public void SetProperties(Item.EquipmentPropertiesUpdatedEvent @event)
  {
    base.Update(@event);

    Dictionary<string, string> properties = new(capacity: 3)
    {
      [nameof(IEquipmentProperties.Defense)] = @event.Properties.Defense.ToString()
    };
    if (@event.Properties.Resistance.HasValue)
    {
      properties[nameof(IEquipmentProperties.Resistance)] = @event.Properties.Resistance.Value.ToString();
    }
    if (@event.Properties.Traits.Length > 0)
    {
      properties[nameof(IEquipmentProperties.Traits)] = string.Join(TraitSeparator, @event.Properties.Traits);
    }
    Properties = SerializeProperties(properties);
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
  private string? SerializeProperties(IReadOnlyDictionary<string, string> properties)
  {
    return properties.Count == 0 ? null : JsonSerializer.Serialize(properties);
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

    if (@event.IsAttunementRequired.HasValue)
    {
      IsAttunementRequired = @event.IsAttunementRequired.Value;
    }
  }
}
