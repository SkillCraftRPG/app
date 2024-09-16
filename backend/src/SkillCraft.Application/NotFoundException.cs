namespace SkillCraft.Application;

public abstract class NotFoundException : ErrorException
{
  protected NotFoundException(string? message = null, Exception? innerException = null) : base(message, innerException)
  {
  }
}
