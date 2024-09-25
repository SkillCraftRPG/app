using Logitar.Portal.Contracts;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Contracts.Comments;

public class CommentModel : Aggregate
{
  public string Text { get; set; }

  public string EntityType { get; set; } = string.Empty; // TODO(fpion): enum
  public Guid EntityId { get; set; }

  public WorldModel World { get; set; }

  public CommentModel() : this(new WorldModel(), string.Empty)
  {
  }

  public CommentModel(WorldModel world, string text)
  {
    Text = text;

    World = world;
  }
}
