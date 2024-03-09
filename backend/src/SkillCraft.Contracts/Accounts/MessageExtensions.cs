using Logitar.Portal.Contracts.Messages;

namespace SkillCraft.Contracts.Accounts;

public static class MessageExtensions
{
  public static string GenerateConfirmationNumber(this SentMessages sentMessages)
  {
    if (sentMessages.Ids.Count == 0)
    {
      throw new ArgumentException("No message has been sent.", nameof(sentMessages));
    }
    else if (sentMessages.Ids.Count > 1)
    {
      throw new ArgumentException("More than one message have been sent.", nameof(sentMessages));
    }
    Guid id = sentMessages.Ids.Single();

    throw new NotImplementedException(); // TODO(fpion): implement
  }
}
