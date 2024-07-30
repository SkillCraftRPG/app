using Logitar.Portal.Contracts.Errors;

namespace SkillCraft.Application;

public abstract class ErrorException : Exception
{
  public abstract Error Error { get; }

  protected ErrorException(string? message = null, Exception? innerException = null) : base(message, innerException)
  {
  }
}
