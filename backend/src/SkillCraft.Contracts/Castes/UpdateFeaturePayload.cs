﻿namespace SkillCraft.Contracts.Castes;

public record UpdateFeaturePayload
{
  public Guid? Id { get; set; }

  public string Name { get; set; }
  public string? Description { get; set; }

  public bool Remove { get; set; }

  public UpdateFeaturePayload() : this(string.Empty)
  {
  }

  public UpdateFeaturePayload(string name)
  {
    Name = name;
  }
}
