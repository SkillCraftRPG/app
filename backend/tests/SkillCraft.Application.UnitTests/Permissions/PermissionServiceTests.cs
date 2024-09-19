using Moq;
using SkillCraft.Application.Settings;
using SkillCraft.Application.Worlds;

namespace SkillCraft.Application.Permissions;

[Trait(Traits.Category, Categories.Unit)]
public class PermissionServiceTests
{
  private readonly AccountSettings _accountSettings = new()
  {
    WorldLimit = 3
  };
  private readonly Mock<IPermissionQuerier> _permissionQuerier = new();
  private readonly Mock<IWorldQuerier> _worldQuerier = new();

  private readonly PermissionService _service;

  public PermissionServiceTests()
  {
    _service = new(_accountSettings, _permissionQuerier.Object, _worldQuerier.Object);
  }

  // TODO(fpion): implement
}
