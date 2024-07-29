namespace SkillCraft.Contracts.Accounts;

public record SentMessage
{
  public ContactType ContactType { get; set; }
  public string MaskedContact { get; set; }
  public string ConfirmationNumber { get; set; }

  public SentMessage() : this(default, string.Empty, string.Empty)
  {
  }

  public SentMessage(ContactType contactType, string maskedContact, string confirmationNumber)
  {
    ContactType = contactType;
    MaskedContact = maskedContact;
    ConfirmationNumber = confirmationNumber;
  }
}
