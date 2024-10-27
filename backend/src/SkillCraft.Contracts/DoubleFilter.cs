namespace SkillCraft.Contracts;

public record DoubleFilter
{
  public List<double> Values { get; set; }
  public string Operator { get; set; }

  public DoubleFilter() : this(string.Empty, [])
  {
  }

  public DoubleFilter(string @operator, IEnumerable<double> values)
  {
    Operator = @operator;
    Values = values.ToList();
  }
}
