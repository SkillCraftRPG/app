namespace SkillCraft.Contracts.Languages;

public record UpdateLanguagePayload
{
  public string? Name { get; set; }
  public Change<string>? Description { get; set; }

  public Change<string>? Script { get; set; }
  public Change<string>? TypicalSpeakers { get; set; }
}
