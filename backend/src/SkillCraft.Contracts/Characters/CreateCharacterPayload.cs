namespace SkillCraft.Contracts.Characters;

public record CreateCharacterPayload
{
  public string Name { get; set; }
  public string? Player { get; set; }

  public Guid LineageId { get; set; }
  public double Height { get; set; }
  public double Weight { get; set; }
  public int Age { get; set; }
  public List<Guid> LanguageIds { get; set; }

  public Guid PersonalityId { get; set; }
  public List<Guid> CustomizationIds { get; set; }

  public List<Guid> AspectIds { get; set; }

  public BaseAttributesPayload Attributes { get; set; }

  public Guid CasteId { get; set; }
  public Guid EducationId { get; set; }

  public List<Guid> TalentIds { get; set; }

  public CreateCharacterPayload() : this(string.Empty)
  {
  }

  public CreateCharacterPayload(string name)
  {
    Name = name;

    LanguageIds = [];
    CustomizationIds = [];
    AspectIds = [];
    Attributes = new();
    TalentIds = [];
  }
}
