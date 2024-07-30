namespace SkillCraft.Application;

public abstract class BadRequestException : ErrorException
{
  protected BadRequestException(string? message = null, Exception? innerException = null) : base(message, innerException)
  {
  }
}
