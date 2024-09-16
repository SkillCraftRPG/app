namespace SkillCraft.Contracts.Lineages;

public record TraitPayload
{
  public Guid? Id { get; set; }

  public string Name { get; set; }
  public string? Description { get; set; }

  public TraitPayload() : this(string.Empty)
  {
  }

  public TraitPayload(string name)
  {
    Name = name;
  }
}
