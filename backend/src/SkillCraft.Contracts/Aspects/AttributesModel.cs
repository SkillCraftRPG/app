﻿namespace SkillCraft.Contracts.Aspects;

public record AttributesModel : IAttributes
{
  public Attribute? Mandatory1 { get; set; }
  public Attribute? Mandatory2 { get; set; }
  public Attribute? Optional1 { get; set; }
  public Attribute? Optional2 { get; set; }
}
