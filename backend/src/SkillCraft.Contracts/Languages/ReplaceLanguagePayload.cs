namespace SkillCraft.Contracts.Languages;

public record ReplaceLanguagePayload
{
  public string Name { get; set; }
  public string? Description { get; set; }

  public string? Script { get; set; }
  public string? TypicalSpeakers { get; set; }

  public ReplaceLanguagePayload() : this(string.Empty)
  {
  }

  public ReplaceLanguagePayload(string name)
  {
    Name = name;
  }
}
