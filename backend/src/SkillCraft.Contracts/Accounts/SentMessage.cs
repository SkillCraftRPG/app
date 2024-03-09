using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Users;

namespace SkillCraft.Contracts.Accounts;

public record SentMessage
{
  public string ConfirmationNumber { get; }
  public ContactType ContactType { get; }
  public string MaskedContact { get; }

  public SentMessage(SentMessages sentMessages, Email email)
  {
    ConfirmationNumber = sentMessages.GenerateConfirmationNumber();
    ContactType = ContactType.Email;
    MaskedContact = email.Address;
  }
}
