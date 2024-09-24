using Logitar.EventSourcing;

namespace SkillCraft.Domain.Comments;

public readonly struct CommentId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public CommentId(AggregateId actorId)
  {
    AggregateId = actorId;
  }
  public CommentId(Guid value)
  {
    AggregateId = new(value);
  }
  public CommentId(string value)
  {
    AggregateId = new(value);
  }

  public static CommentId NewId() => new(AggregateId.NewId());

  public Guid ToGuid() => AggregateId.ToGuid();

  public static bool operator ==(CommentId left, CommentId right) => left.Equals(right);
  public static bool operator !=(CommentId left, CommentId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is CommentId userId && userId.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
