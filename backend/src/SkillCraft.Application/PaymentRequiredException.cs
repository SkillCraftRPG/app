namespace SkillCraft.Application;

public abstract class PaymentRequiredException : ErrorException
{
  protected PaymentRequiredException(string? message = null, Exception? innerException = null) : base(message, innerException)
  {
  }
}
