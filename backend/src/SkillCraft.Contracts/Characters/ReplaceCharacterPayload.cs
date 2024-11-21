﻿namespace SkillCraft.Contracts.Characters;

public record ReplaceCharacterPayload
{
  public string Name { get; set; }
  public string? Player { get; set; }

  public double Height { get; set; }
  public double Weight { get; set; }
  public int Age { get; set; }

  public int Experience { get; set; }
  public int Vitality { get; set; }
  public int Stamina { get; set; }
  public int BloodAlcoholContent { get; set; }
  public int Intoxication { get; set; }

  public ReplaceCharacterPayload() : this(string.Empty)
  {
  }

  public ReplaceCharacterPayload(string name)
  {
    Name = name;
  }
}