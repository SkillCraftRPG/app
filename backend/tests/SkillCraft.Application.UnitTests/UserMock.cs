using Bogus;
using Logitar;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Users;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Application;

internal class UserMock : User
{
  public UserMock(Faker? faker = null)
  {
    faker ??= new();
    Person person = faker.Person;

    DateTime now = DateTime.Now;

    Id = Guid.NewGuid();
    Version = 6;
    CreatedOn = now;
    UpdatedOn = now;

    UniqueName = person.UserName;
    HasPassword = true;
    PasswordChangedOn = now;

    Email = new(person.Email)
    {
      VerifiedOn = now,
      IsVerified = true
    };
    Phone = new("CA", "514-845-4636", extension: null, "+15148454636")
    {
      VerifiedOn = now,
      IsVerified = true
    };
    IsConfirmed = true;

    FirstName = person.FirstName;
    LastName = person.LastName;
    FullName = person.FullName;

    Birthdate = person.DateOfBirth;
    Gender = person.Gender.ToString().ToLowerInvariant();
    Locale = new("fr");
    TimeZone = "America/Montreal";

    Picture = person.Avatar;
    Website = $"https://www.{person.Website}";

    AuthenticatedOn = now;

    CustomAttributes.Add(new(nameof(MultiFactorAuthenticationMode), MultiFactorAuthenticationMode.Phone.ToString()));
    CustomAttributes.Add(new("ProfileCompletedOn", now.ToISOString()));
    CustomAttributes.Add(new(nameof(UserType), UserType.Gamemaster.ToString()));

    Realm = new RealmMock();

    Actor actor = new(this);
    CreatedBy = actor;
    UpdatedBy = actor;
    PasswordChangedBy = actor;
    Email.VerifiedBy = actor;
    Phone.VerifiedBy = actor;
  }
}
