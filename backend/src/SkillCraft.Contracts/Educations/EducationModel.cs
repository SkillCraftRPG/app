﻿using Logitar.Portal.Contracts;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Contracts.Educations;

public class EducationModel : Aggregate
{
  public WorldModel World { get; set; }

  public string Name { get; set; }
  public string? Description { get; set; }

  public Skill? Skill { get; set; }
  public double? WealthMultiplier { get; set; }

  public EducationModel() : this(new WorldModel(), string.Empty)
  {
  }

  public EducationModel(WorldModel world, string name)
  {
    World = world;

    Name = name;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
