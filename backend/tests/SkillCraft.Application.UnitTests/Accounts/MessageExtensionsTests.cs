using Bogus;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Users;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Application.Accounts;

[Trait(Traits.Category, Categories.Unit)]
public class MessageExtensionsTests
{
  private readonly Faker _faker = new();

  [Fact(DisplayName = "GenerateConfirmationNumber: it should generate the correct confirmation number")]
  public void GenerateConfirmationNumber_it_should_generate_the_correct_confirmation_number()
  {
    SentMessages sentMessages = new([Guid.NewGuid()]);
    string confirmationNumber = sentMessages.GenerateConfirmationNumber();

    DateTime now = DateTime.UtcNow;
    string expected = $"0000-{now:yyMMdd}-00";
    Assert.Equal(expected, confirmationNumber);
  }

  [Fact(DisplayName = "GenerateConfirmationNumber: it should throw ArgumentException when multiple messages were sent.")]
  public void GenerateConfirmationNumber_it_should_throw_ArgumentException_when_multiple_messages_were_sent()
  {
    SentMessages sentMessages = new([Guid.NewGuid(), Guid.NewGuid()]);
    var exception = Assert.Throws<ArgumentException>(sentMessages.GenerateConfirmationNumber);
    Assert.StartsWith("More than one message have been sent.", exception.Message);
    Assert.Equal("sentMessages", exception.ParamName);
  }

  [Fact(DisplayName = "GenerateConfirmationNumber: it should throw ArgumentException when no message was sent.")]
  public void GenerateConfirmationNumber_it_should_throw_ArgumentException_when_no_message_was_sent()
  {
    SentMessages sentMessages = new([]);
    var exception = Assert.Throws<ArgumentException>(sentMessages.GenerateConfirmationNumber);
    Assert.StartsWith("No message has been sent.", exception.Message);
    Assert.Equal("sentMessages", exception.ParamName);
  }

  [Fact(DisplayName = "ToSentMessage: it should return the correct sent email message.")]
  public void ToSentMessage_it_should_return_the_correct_sent_email_message()
  {
    SentMessages sentMessages = new([Guid.NewGuid()]);
    Email email = new(_faker.Person.Email);
    SentMessage sentMessage = sentMessages.ToSentMessage(email);

    string confirmationNumber = sentMessages.GenerateConfirmationNumber();
    Assert.Equal(confirmationNumber, sentMessage.ConfirmationNumber);
    Assert.Equal(ContactType.Email, sentMessage.ContactType);
    Assert.Equal(email.Address, sentMessage.MaskedContact);
  }
}
