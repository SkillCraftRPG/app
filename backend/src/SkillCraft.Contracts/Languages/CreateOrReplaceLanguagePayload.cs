namespace SkillCraft.Contracts.Languages;

public record CreateOrReplaceLanguagePayload
{
  public string Name { get; set; }
  public string? Description { get; set; }

  public string? Script { get; set; }
  public string? TypicalSpeakers { get; set; }

  public CreateOrReplaceLanguagePayload() : this(string.Empty)
  {
  }

  public CreateOrReplaceLanguagePayload(string name)
  {
    Name = name;
  }
}
