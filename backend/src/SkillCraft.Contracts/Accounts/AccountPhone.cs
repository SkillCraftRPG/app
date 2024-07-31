using Logitar.Identity.Contracts.Users;

namespace SkillCraft.Contracts.Accounts;

public record AccountPhone
{
  private const string DefaultCountryCode = "CA";

  public string CountryCode { get; set; }
  public string Number { get; set; }

  public AccountPhone() : this(string.Empty, string.Empty)
  {
  }

  public AccountPhone(IPhone phone) : this(phone.CountryCode ?? DefaultCountryCode, phone.Number)
  {
  }

  public AccountPhone(string countryCode, string number)
  {
    CountryCode = countryCode;
    Number = number;
  }

  public static AccountPhone? TryCreate(IPhone? phone) => phone == null ? null : new(phone);
}
