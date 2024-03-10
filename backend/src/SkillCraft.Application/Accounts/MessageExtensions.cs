using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Users;

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
    _ = sentMessages.Ids.Single();
    DateTime now = DateTime.UtcNow;

    StringBuilder number = new();
    number.Append("0000")
      .Append('-')
      .Append((now.Year % 100).ToString("D2"))
      .Append(now.Month.ToString("D2"))
      .Append(now.Day.ToString("D2"))
      .Append('-')
      .Append("00");
    return number.ToString(); // ISSUE #7: Generate Confirmation Number
  }

  public static SentMessage ToSentMessage(this SentMessages sentMessages, Email email)
  {
    return new SentMessage(sentMessages.GenerateConfirmationNumber(), ContactType.Email, email.Address);
  }
}
