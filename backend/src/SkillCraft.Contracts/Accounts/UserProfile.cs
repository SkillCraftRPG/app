using Logitar.Portal.Contracts;

namespace SkillCraft.Contracts.Accounts;

public record UserProfile
{
  public DateTime RegisteredOn { get; set; }
  public DateTime CompletedOn { get; set; }

  public string EmailAddress { get; set; }
  public Phone? Phone { get; set; }

  public DateTime? PasswordChangedOn { get; set; }
  public MultiFactorAuthenticationMode MultiFactorAuthenticationMode { get; set; }

  public string FirstName { get; set; }
  public string LastName { get; set; }

  public DateTime? Birthdate { get; set; }
  public string? Gender { get; set; }
  public Locale Locale { get; set; }
  public string TimeZone { get; set; }

  public UserProfile() : this(string.Empty, string.Empty, string.Empty, new Locale(), string.Empty)
  {
  }

  public UserProfile(string emailAddress, string firstName, string lastName, Locale locale, string timeZone)
  {
    EmailAddress = emailAddress;

    FirstName = firstName;
    LastName = lastName;

    Locale = locale;
    TimeZone = timeZone;
  }
}
