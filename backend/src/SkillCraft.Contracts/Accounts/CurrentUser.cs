using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;

namespace SkillCraft.Contracts.Accounts;

public record CurrentUser
{
  public UserType Type { get; set; }

  public string DisplayName { get; set; }
  public string? EmailAddress { get; set; }
  public string? PictureUrl { get; set; }

  public CurrentUser() : this(string.Empty)
  {
  }

  public CurrentUser(string displayName, string? emailAddress = null, string? pictureUrl = null)
  {
    DisplayName = displayName;
    EmailAddress = emailAddress;
    PictureUrl = pictureUrl;
  }

  public CurrentUser(Session session) : this(session.User)
  {
  }

  public CurrentUser(User user) : this(user.FullName ?? user.UniqueName, user.Email?.Address, user.Picture)
  {
    foreach (CustomAttribute customAttribute in user.CustomAttributes)
    {
      if (customAttribute.Key == nameof(UserType) && Enum.TryParse(customAttribute.Value, out UserType type))
      {
        Type = type;
      }
    }
  }
}
