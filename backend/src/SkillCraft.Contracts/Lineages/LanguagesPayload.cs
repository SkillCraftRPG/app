namespace SkillCraft.Contracts.Lineages;

public record LanguagesPayload
{
  public List<Guid> Ids { get; set; }
  public int Extra { get; set; }
  public string? Text { get; set; }

  public LanguagesPayload() : this([])
  {
  }

  public LanguagesPayload(IEnumerable<Guid> ids)
  {
    Ids = ids.ToList();
  }
}
