namespace SkillCraft.Contracts;

public record TierFilter
{
  public List<int> Values { get; set; }
  public string Operator { get; set; }

  public TierFilter() : this(string.Empty, [])
  {
  }

  public TierFilter(string @operator, IEnumerable<int> values)
  {
    Operator = @operator;
    Values = values.ToList();
  }
}
