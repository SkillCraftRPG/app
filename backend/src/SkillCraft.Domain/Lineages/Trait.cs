namespace SkillCraft.Domain.Lineages;

public record Trait(Name Name, Description? Description)
{
  [JsonIgnore]
  public int Size => Name.Size + (Description?.Size ?? 0);
};
