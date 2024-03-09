using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Users;

namespace SkillCraft.Contracts.Accounts;

public record SentMessage
{
  public string ConfirmationNumber { get; set; }
  public ContactType ContactType { get; set; }
  public string MaskedContact { get; set; }

  public SentMessage() : this(new SentMessages(), new Email())
  {
  }

  public SentMessage(SentMessages sentMessages, Email email)
  {
    ConfirmationNumber = sentMessages.GenerateConfirmationNumber();
    ContactType = ContactType.Email;
    MaskedContact = email.Mask();
  }
}
