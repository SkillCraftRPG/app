using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;

namespace SkillCraft.Application.Accounts;

internal class RealmMock : Realm
{
  public RealmMock() : base("skillcraftrpg", "JXp6/&%#F.KAQP!S3)qCevfNH2t?T,(d")
  {
    CreatedOn = UpdatedOn = DateTime.Now;

    DisplayName = "SkillCraftRPG";
    Description = "This is the realm of the SkillCraft Tabletop Role-Playing Game.";

    DefaultLocale = new Locale("fr");
    Url = "https://www.skillcraftrpg.ca";
  }
}
