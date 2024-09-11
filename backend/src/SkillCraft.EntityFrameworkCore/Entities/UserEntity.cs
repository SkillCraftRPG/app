using Logitar.Portal.Contracts.Users;

namespace SkillCraft.EntityFrameworkCore.Entities;

internal class UserEntity
{
  public int UserId { get; private set; }
  public Guid Id { get; private set; }

  public bool IsDeleted { get; private set; }

  public string DisplayName { get; private set; } = string.Empty;
  public string? EmailAddress { get; private set; }
  public string? PictureUrl { get; private set; }

  public List<StorageDetailEntity> StorageDetails { get; private set; } = [];
  public StorageSummaryEntity? StorageSummary { get; private set; }
  public List<WorldEntity> Worlds { get; private set; } = [];

  public UserEntity(User user)
  {
    Id = user.Id;

    Update(user);
  }

  private UserEntity()
  {
  }

  public void Update(User user)
  {
    if (user.Id != Id)
    {
      throw new ArgumentException($"The user 'Id={user.Id}' was not expected. The expected user is 'Id={Id}'.", nameof(user));
    }

    DisplayName = user.FullName ?? user.UniqueName;
    EmailAddress = user.Email?.Address;
    PictureUrl = user.Picture;
  }
}
