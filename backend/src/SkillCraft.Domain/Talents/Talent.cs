﻿using Logitar.EventSourcing;
using MediatR;
using SkillCraft.Contracts;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Domain.Talents;

public class Talent : AggregateRoot
{
  private UpdatedEvent _updatedEvent = new();

  public new TalentId Id => new(base.Id);

  public WorldId WorldId { get; private set; }

  public int Tier { get; private set; }

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

  private bool _allowMultiplePurchases = false;
  public bool AllowMultiplePurchases
  {
    get => _allowMultiplePurchases;
    set
    {
      if (_allowMultiplePurchases != value)
      {
        _allowMultiplePurchases = value;
        _updatedEvent.AllowMultiplePurchases = value;
      }
    }
  }
  public TalentId? RequiredTalentId { get; private set; }

  public Talent() : base()
  {
  }

  public Talent(WorldId worldId, int tier, Name name, UserId userId, TalentId? id = null) : base(id?.AggregateId)
  {
    if (tier < 0 || tier > 3)
    {
      throw new ArgumentOutOfRangeException(nameof(tier));
    }

    Raise(new CreatedEvent(worldId, tier, name), userId.ActorId);
  }
  protected virtual void Apply(CreatedEvent @event)
  {
    WorldId = @event.WorldId;

    Tier = @event.Tier;

    _name = @event.Name;
  }

  public void SetRequiredTalent(Talent? requiredTalent)
  {
    if (requiredTalent != null)
    {
      if (requiredTalent.WorldId != WorldId)
      {
        throw new ArgumentException("The required talent does not reside in the same world as the requiring talent.", nameof(requiredTalent));
      }
      else if (requiredTalent.Tier > Tier)
      {
        throw new ArgumentException("The required talent tier must be inferior or equal to the requiring talent tier.", nameof(requiredTalent));
      }
    }

    if (RequiredTalentId != requiredTalent?.Id)
    {
      RequiredTalentId = requiredTalent?.Id;
      _updatedEvent.RequiredTalentId = new Change<TalentId?>(requiredTalent?.Id);
    }
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

    if (@event.AllowMultiplePurchases.HasValue)
    {
      _allowMultiplePurchases = @event.AllowMultiplePurchases.Value;
    }
    if (@event.RequiredTalentId != null)
    {
      RequiredTalentId = @event.RequiredTalentId.Value;
    }
  }

  public override string ToString() => $"{Name.Value} | {base.ToString()}";

  public class CreatedEvent : DomainEvent, INotification
  {
    public WorldId WorldId { get; }

    public int Tier { get; }

    public Name Name { get; }

    public CreatedEvent(WorldId worldId, int tier, Name name)
    {
      WorldId = worldId;

      Tier = tier;

      Name = name;
    }
  }

  public class UpdatedEvent : DomainEvent, INotification
  {
    public Name? Name { get; set; }
    public Change<Description>? Description { get; set; }

    public bool? AllowMultiplePurchases { get; set; }
    public Change<TalentId?>? RequiredTalentId { get; set; }

    public bool HasChanges => Name != null || Description != null || AllowMultiplePurchases.HasValue || RequiredTalentId != null;
  }
}
