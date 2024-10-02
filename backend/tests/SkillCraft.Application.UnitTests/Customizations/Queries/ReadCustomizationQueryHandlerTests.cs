using Logitar.Portal.Contracts.Actors;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Application.Customizations.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadCustomizationQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICustomizationQuerier> _customizationQuerier = new();
  private readonly Mock<IPermissionService> _permissionService = new();

  private readonly ReadCustomizationQueryHandler _handler;

  private readonly UserMock _user = new();
  private readonly WorldMock _world;
  private readonly CustomizationModel _customization;

  public ReadCustomizationQueryHandlerTests()
  {
    _handler = new(_customizationQuerier.Object, _permissionService.Object);

    _world = new(_user);
    WorldModel worldModel = new(new Actor(_user), _world.Slug.Value)
    {
      Id = _world.Id.ToGuid()
    };
    _customization = new(worldModel, "Abruti")
    {
      Id = Guid.NewGuid()
    };
    _customizationQuerier.Setup(x => x.ReadAsync(_world.Id, _customization.Id, _cancellationToken)).ReturnsAsync(_customization);
  }

  [Fact(DisplayName = "It should return null when no customization is found.")]
  public async Task It_should_return_null_when_no_customization_is_found()
  {
    ReadCustomizationQuery query = new(Guid.Empty);
    query.Contextualize(_world);

    Assert.Null(await _handler.Handle(query, _cancellationToken));

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(query, EntityType.Customization, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return the customization found by ID.")]
  public async Task It_should_return_the_customization_found_by_Id()
  {
    ReadCustomizationQuery query = new(_customization.Id);
    query.Contextualize(_world);

    CustomizationModel? customization = await _handler.Handle(query, _cancellationToken);
    Assert.Same(_customization, customization);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(query, EntityType.Customization, _cancellationToken), Times.Once);
  }
}
