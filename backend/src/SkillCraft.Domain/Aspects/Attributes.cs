﻿using FluentValidation;
using SkillCraft.Contracts.Aspects;
using SkillCraft.Domain.Aspects.Validators;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Domain.Aspects;

public record Attributes : IAttributes
{
  public Attribute? Mandatory1 { get; }
  public Attribute? Mandatory2 { get; }
  public Attribute? Optional1 { get; }
  public Attribute? Optional2 { get; }

  public Attributes() : this(mandatory1: null, mandatory2: null, optional1: null, optional2: null)
  {
  }

  public Attributes(IAttributes attributes) : this(attributes.Mandatory1, attributes.Mandatory2, attributes.Optional1, attributes.Optional2)
  {
  }

  [JsonConstructor]
  public Attributes(Attribute? mandatory1, Attribute? mandatory2, Attribute? optional1, Attribute? optional2)
  {
    Mandatory1 = mandatory1;
    Mandatory2 = mandatory2;
    Optional1 = optional1;
    Optional2 = optional2;
    new AttributesValidator().ValidateAndThrow(this);
  }
}
