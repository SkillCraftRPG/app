namespace SkillCraft.Application;

public abstract class ForbiddenException : ErrorException
{
  protected ForbiddenException(string? message = null, Exception? innerException = null) : base(message, innerException)
  {
  }
}
