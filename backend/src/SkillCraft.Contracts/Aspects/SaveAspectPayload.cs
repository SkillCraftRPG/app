namespace SkillCraft.Contracts.Aspects;

public record SaveAspectPayload
{
  public string Name { get; set; }
  public string? Description { get; set; }

  public AttributeSelectionModel Attributes { get; set; }
  public SkillsModel Skills { get; set; }

  public SaveAspectPayload() : this(string.Empty)
  {
  }

  public SaveAspectPayload(string name)
  {
    Name = name;

    Attributes = new();
    Skills = new();
  }
}
