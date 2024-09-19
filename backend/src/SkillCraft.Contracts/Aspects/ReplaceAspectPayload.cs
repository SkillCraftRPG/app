namespace SkillCraft.Contracts.Aspects;

public record ReplaceAspectPayload
{
  public string Name { get; set; }
  public string? Description { get; set; }

  public AttributeSelectionModel Attributes { get; set; }
  public SkillsModel Skills { get; set; }

  public ReplaceAspectPayload() : this(string.Empty)
  {
  }

  public ReplaceAspectPayload(string name)
  {
    Name = name;

    Attributes = new();
    Skills = new();
  }
}
