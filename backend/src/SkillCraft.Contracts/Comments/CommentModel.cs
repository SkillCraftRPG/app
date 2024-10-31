using Logitar.Portal.Contracts;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Contracts.Comments;

public class CommentModel : Aggregate
{
  public WorldModel World { get; set; }

  public EntityType EntityType { get; set; }
  public Guid EntityId { get; set; }

  public string Text { get; set; }

  public CommentModel() : this(new WorldModel(), string.Empty)
  {
  }

  public CommentModel(WorldModel world, string text)
  {
    World = world;
    Text = text;
  }

  public override string ToString() => $"{Text} | {base.ToString()}";
}
