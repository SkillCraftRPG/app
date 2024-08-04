namespace SkillCraft.Application;

public abstract class ConflictException : ErrorException
{
  protected ConflictException(string? message = null, Exception? innerException = null) : base(message, innerException)
  {
  }
}
