namespace SkillCraft.Contracts.Languages;

public record CreateLanguagePayload
{
  public string Name { get; set; }
  public string? Description { get; set; }

  public string? Script { get; set; }
  public string? TypicalSpeakers { get; set; }

  public CreateLanguagePayload() : this(string.Empty)
  {
  }

  public CreateLanguagePayload(string name)
  {
    Name = name;
  }
}
