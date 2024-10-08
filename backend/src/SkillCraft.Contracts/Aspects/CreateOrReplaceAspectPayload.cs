﻿namespace SkillCraft.Contracts.Aspects;

public record CreateOrReplaceAspectPayload
{
  public string Name { get; set; }
  public string? Description { get; set; }

  public AttributeSelectionModel Attributes { get; set; }
  public SkillsModel Skills { get; set; }

  public CreateOrReplaceAspectPayload() : this(string.Empty)
  {
  }

  public CreateOrReplaceAspectPayload(string name)
  {
    Name = name;

    Attributes = new();
    Skills = new();
  }
}
