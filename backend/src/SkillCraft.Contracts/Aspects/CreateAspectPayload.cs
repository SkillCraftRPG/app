namespace SkillCraft.Contracts.Aspects;

public record CreateAspectPayload
{
  public string Name { get; set; }
  public string? Description { get; set; }

  public AttributesModel Attributes { get; set; }
  public SkillsModel Skills { get; set; }

  public CreateAspectPayload() : this(string.Empty)
  {
  }

  public CreateAspectPayload(string name)
  {
    Name = name;

    Attributes = new();
    Skills = new();
  }
}
