﻿namespace SkillCraft.Domain.Castes;

public record Trait(Name Name, Description? Description)
{
  [JsonIgnore]
  public int Size => Name.Size + (Description?.Size ?? 0);
}
