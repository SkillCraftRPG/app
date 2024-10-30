namespace SkillCraft.Contracts.Characters;

public record StartingWealthPayload
{
  public Guid ItemId { get; set; }
  public int Quantity { get; set; }
}
