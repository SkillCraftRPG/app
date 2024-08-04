using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Realms;

namespace SkillCraft.Application;

internal class RealmMock : Realm
{
  public RealmMock() : base("skillcraftrpg", "JXp6/&%#F.KAQP!S3)qCevfNH2t?T,(d")
  {
    Actor actor = Actor.System;
    DateTime now = DateTime.UtcNow;

    Id = Guid.NewGuid();
    Version = 2;
    CreatedBy = actor;
    CreatedOn = now;
    UpdatedBy = actor;
    UpdatedOn = now;

    DisplayName = "SkillCraftRPG";
    Description = "This is the realm of the SkillCraft Tabletop Role-Playing Game.";

    DefaultLocale = new Locale("fr");
    Url = "https://www.skillcraftrpg.ca";
  }
}
