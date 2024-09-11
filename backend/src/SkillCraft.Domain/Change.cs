namespace SkillCraft.Domain;

public record Change<T>
{
  public T? Value { get; }

  public Change(T? value)
  {
    Value = value;
  }
}
