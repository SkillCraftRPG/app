namespace SkillCraft.Contracts.Characters;

public record CreateCharacterPayload
{
  public string Name { get; set; }
  public string? Player { get; set; }

  public Guid LineageId { get; set; }
  public double Height { get; set; }
  public double Weight { get; set; }
  public int Age { get; set; }

  // TODO(fpion): Lineage.Attributes.Extra

  // TODO(fpion): Lineage.Languages.Extra

  public Guid PersonalityId { get; set; }
  public List<Guid> CustomizationIds { get; set; }

  public List<Guid> AspectIds { get; set; }

  public CreateCharacterPayload() : this(string.Empty)
  {
  }

  public CreateCharacterPayload(string name)
  {
    Name = name;

    CustomizationIds = [];
    AspectIds = [];
  }
}
