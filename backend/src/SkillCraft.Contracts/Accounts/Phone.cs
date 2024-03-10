namespace SkillCraft.Contracts.Accounts;

public record Phone
{
  public string? CountryCode { get; set; }
  public string Number { get; set; }

  public Phone() : this(string.Empty)
  {
  }

  public Phone(string number) : this(countryCode: null, number)
  {
  }

  public Phone(string? countryCode, string number)
  {
    CountryCode = countryCode;
    Number = number;
  }
}
