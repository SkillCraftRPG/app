namespace SkillCraft.Contracts.Lineages;

public record UpdateLanguagesPayload
{
  public List<Guid>? Ids { get; set; }
  public int? Extra { get; set; }
  public Change<string>? Text { get; set; }
}
