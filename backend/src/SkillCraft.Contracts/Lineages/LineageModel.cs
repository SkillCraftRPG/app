﻿using Logitar.Portal.Contracts;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Contracts.Lineages;

public class LineageModel : Aggregate
{
  public string Name { get; set; }
  public string? Description { get; set; }

  public LineageModel? Parent { get; set; }

  public WorldModel World { get; set; }

  public LineageModel() : this(new WorldModel(), string.Empty)
  {
  }

  public LineageModel(WorldModel world, string name)
  {
    Name = name;

    World = world;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
