using Logitar.EventSourcing;
using MediatR;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Items;
using SkillCraft.Domain.Items.Properties;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Domain.Items;

public class Item : AggregateRoot
{
  private UpdatedEvent _updatedEvent = new();

  public new ItemId Id => new(base.Id);
  public WorldId WorldId => Id.WorldId;
  public Guid EntityId => Id.EntityId;

  private Name? _name = null;
  public Name Name
  {
    get => _name ?? throw new InvalidOperationException($"The {nameof(Name)} has not been initialized yet.");
    set
    {
      if (_name != value)
      {
        _name = value;
        _updatedEvent.Name = value;
      }
    }
  }
  private Description? _description = null;
  public Description? Description
  {
    get => _description;
    set
    {
      if (_description != value)
      {
        _description = value;
        _updatedEvent.Description = new Change<Description>(value);
      }
    }
  }

  private double? _value = null;
  public double? Value
  {
    get => _value;
    set
    {
      if (value.HasValue && value.Value < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(Value));
      }

      if (_value != value)
      {
        _value = value;
        _updatedEvent.Value = new Change<double?>(value);
      }
    }
  }
  private double? _weight = null;
  public double? Weight
  {
    get => _weight;
    set
    {
      if (value.HasValue && value.Value < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(Weight));
      }

      if (_weight != value)
      {
        _weight = value;
        _updatedEvent.Weight = new Change<double?>(value);
      }
    }
  }

  private bool _isAttunementRequired = false;
  public bool IsAttunementRequired
  {
    get => _isAttunementRequired;
    set
    {
      if (_isAttunementRequired != value)
      {
        _isAttunementRequired = value;
        _updatedEvent.IsAttunementRequired = value;
      }
    }
  }

  public ItemCategory Category { get; private set; }
  private PropertiesBase? _properties = null;
  public PropertiesBase Properties => _properties ?? throw new InvalidOperationException($"The {nameof(Properties)} have not been initialized yet.");

  public Item() : base()
  {
  }

  public Item(WorldId worldId, Name name, PropertiesBase properties, UserId userId, Guid? entityId = null) : base(new ItemId(worldId, entityId).AggregateId)
  {
    if (!Enum.IsDefined(properties.Category))
    {
      throw new ArgumentException($"The category '{properties.Category}' is not defined.", nameof(properties));
    }

    Raise(new CreatedEvent(name, properties.Category), userId.ActorId);

    switch (Category)
    {
      case ItemCategory.Consumable:
        SetProperties((ConsumableProperties)properties, userId);
        break;
      case ItemCategory.Container:
        SetProperties((ContainerProperties)properties, userId);
        break;
      case ItemCategory.Device:
        SetProperties((DeviceProperties)properties, userId);
        break;
      case ItemCategory.Equipment:
        SetProperties((EquipmentProperties)properties, userId);
        break;
      case ItemCategory.Miscellaneous:
        SetProperties((MiscellaneousProperties)properties, userId);
        break;
      case ItemCategory.Money:
        SetProperties((MoneyProperties)properties, userId);
        break;
      case ItemCategory.Weapon:
        SetProperties((WeaponProperties)properties, userId);
        break;
      default:
        throw new ItemCategoryNotSupportedException(Category);
    }
  }
  protected virtual void Apply(CreatedEvent @event)
  {
    _name = @event.Name;

    Category = @event.Category;
  }

  public void SetProperties(ConsumableProperties properties, UserId userId)
  {
    if (Category != ItemCategory.Consumable)
    {
      throw new ItemCategoryMismatchException(this, properties.Category);
    }
    else if (_properties != properties)
    {
      Raise(new ConsumablePropertiesUpdatedEvent(properties), userId.ActorId);
    }
  }
  protected virtual void Apply(ConsumablePropertiesUpdatedEvent @event)
  {
    _properties = @event.Properties;
  }

  public void SetProperties(ContainerProperties properties, UserId userId)
  {
    if (Category != ItemCategory.Container)
    {
      throw new ItemCategoryMismatchException(this, properties.Category);
    }
    else if (_properties != properties)
    {
      Raise(new ContainerPropertiesUpdatedEvent(properties), userId.ActorId);
    }
  }
  protected virtual void Apply(ContainerPropertiesUpdatedEvent @event)
  {
    _properties = @event.Properties;
  }

  public void SetProperties(DeviceProperties properties, UserId userId)
  {
    if (Category != ItemCategory.Device)
    {
      throw new ItemCategoryMismatchException(this, properties.Category);
    }
    else if (_properties != properties)
    {
      Raise(new DevicePropertiesUpdatedEvent(properties), userId.ActorId);
    }
  }
  protected virtual void Apply(DevicePropertiesUpdatedEvent @event)
  {
    _properties = @event.Properties;
  }

  public void SetProperties(EquipmentProperties properties, UserId userId)
  {
    if (Category != ItemCategory.Equipment)
    {
      throw new ItemCategoryMismatchException(this, properties.Category);
    }
    else if (_properties != properties)
    {
      Raise(new EquipmentPropertiesUpdatedEvent(properties), userId.ActorId);
    }
  }
  protected virtual void Apply(EquipmentPropertiesUpdatedEvent @event)
  {
    _properties = @event.Properties;
  }

  public void SetProperties(MiscellaneousProperties properties, UserId userId)
  {
    if (Category != ItemCategory.Miscellaneous)
    {
      throw new ItemCategoryMismatchException(this, properties.Category);
    }
    else if (_properties != properties)
    {
      Raise(new MiscellaneousPropertiesUpdatedEvent(properties), userId.ActorId);
    }
  }
  protected virtual void Apply(MiscellaneousPropertiesUpdatedEvent @event)
  {
    _properties = @event.Properties;
  }

  public void SetProperties(MoneyProperties properties, UserId userId)
  {
    if (Category != ItemCategory.Money)
    {
      throw new ItemCategoryMismatchException(this, properties.Category);
    }
    else if (_properties != properties)
    {
      Raise(new MoneyPropertiesUpdatedEvent(properties), userId.ActorId);
    }
  }
  protected virtual void Apply(MoneyPropertiesUpdatedEvent @event)
  {
    _properties = @event.Properties;
  }

  public void SetProperties(WeaponProperties properties, UserId userId)
  {
    if (Category != ItemCategory.Weapon)
    {
      throw new ItemCategoryMismatchException(this, properties.Category);
    }
    else if (_properties != properties)
    {
      Raise(new WeaponPropertiesUpdatedEvent(properties), userId.ActorId);
    }
  }
  protected virtual void Apply(WeaponPropertiesUpdatedEvent @event)
  {
    _properties = @event.Properties;
  }

  public void Update(UserId userId)
  {
    if (_updatedEvent.HasChanges)
    {
      Raise(_updatedEvent, userId.ActorId, DateTime.Now);
      _updatedEvent = new();
    }
  }
  protected virtual void Apply(UpdatedEvent @event)
  {
    if (@event.Name != null)
    {
      _name = @event.Name;
    }
    if (@event.Description != null)
    {
      _description = @event.Description.Value;
    }

    if (@event.Value != null)
    {
      _value = @event.Value.Value;
    }
    if (@event.Weight != null)
    {
      _weight = @event.Weight.Value;
    }

    if (@event.IsAttunementRequired.HasValue)
    {
      _isAttunementRequired = @event.IsAttunementRequired.Value;
    }
  }

  public override string ToString() => $"{Name.Value} | {base.ToString()}";

  public class ConsumablePropertiesUpdatedEvent : DomainEvent, INotification
  {
    public ConsumableProperties Properties { get; }

    public ConsumablePropertiesUpdatedEvent(ConsumableProperties properties)
    {
      Properties = properties;
    }
  }

  public class ContainerPropertiesUpdatedEvent : DomainEvent, INotification
  {
    public ContainerProperties Properties { get; }

    public ContainerPropertiesUpdatedEvent(ContainerProperties properties)
    {
      Properties = properties;
    }
  }

  public class CreatedEvent : DomainEvent, INotification
  {
    public Name Name { get; }

    public ItemCategory Category { get; }

    public CreatedEvent(Name name, ItemCategory category)
    {
      Name = name;

      Category = category;
    }
  }

  public class DevicePropertiesUpdatedEvent : DomainEvent, INotification
  {
    public DeviceProperties Properties { get; }

    public DevicePropertiesUpdatedEvent(DeviceProperties properties)
    {
      Properties = properties;
    }
  }

  public class EquipmentPropertiesUpdatedEvent : DomainEvent, INotification
  {
    public EquipmentProperties Properties { get; }

    public EquipmentPropertiesUpdatedEvent(EquipmentProperties properties)
    {
      Properties = properties;
    }
  }

  public class MiscellaneousPropertiesUpdatedEvent : DomainEvent, INotification
  {
    public MiscellaneousProperties Properties { get; }

    public MiscellaneousPropertiesUpdatedEvent(MiscellaneousProperties properties)
    {
      Properties = properties;
    }
  }

  public class MoneyPropertiesUpdatedEvent : DomainEvent, INotification
  {
    public MoneyProperties Properties { get; }

    public MoneyPropertiesUpdatedEvent(MoneyProperties properties)
    {
      Properties = properties;
    }
  }

  public class UpdatedEvent : DomainEvent, INotification
  {
    public Name? Name { get; set; }
    public Change<Description>? Description { get; set; }

    public Change<double?>? Value { get; set; }
    public Change<double?>? Weight { get; set; }

    public bool? IsAttunementRequired { get; set; }

    public bool HasChanges => Name != null || Description != null || Value != null || Weight != null || IsAttunementRequired != null;
  }

  public class WeaponPropertiesUpdatedEvent : DomainEvent, INotification
  {
    public WeaponProperties Properties { get; }

    public WeaponPropertiesUpdatedEvent(WeaponProperties properties)
    {
      Properties = properties;
    }
  }
}
