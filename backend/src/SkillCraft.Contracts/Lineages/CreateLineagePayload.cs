namespace SkillCraft.Contracts.Lineages;

public record CreateLineagePayload
{
  public Guid? ParentId { get; set; }

  public string Name { get; set; }
  public string? Description { get; set; }

  public AttributesModel Attributes { get; set; }
  public List<TraitPayload> Traits { get; set; }

  public CreateLineagePayload() : this(string.Empty)
  {
  }

  public CreateLineagePayload(string name)
  {
    Name = name;

    Attributes = new();
    Traits = [];
  }
}
