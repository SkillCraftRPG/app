using Logitar.Portal.Contracts.Users;

namespace SkillCraft.Contracts.Accounts;

public record ProfilePayload
{
  public string Token { get; set; }

  public string FirstName { get; set; }
  public string LastName { get; set; }

  public string? Password { get; set; }
  public MultiFactorAuthenticationMode? MultiFactorAuthenticationMode { get; set; }
  public PhonePayload? Phone { get; set; }

  public DateTime? Birthdate { get; set; }
  public string? Gender { get; set; }
  public string Locale { get; set; }
  public string TimeZone { get; set; }

  public ProfilePayload() : this(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty)
  {
  }

  public ProfilePayload(string token, string firstName, string lastName, string locale, string timeZone)
  {
    Token = token;

    FirstName = firstName;
    LastName = lastName;

    Locale = locale;
    TimeZone = timeZone;
  }
}
