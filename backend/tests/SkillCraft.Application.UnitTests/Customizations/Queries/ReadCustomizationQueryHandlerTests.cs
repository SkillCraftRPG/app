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

  private readonly CustomizationModel _customization = new(new WorldModel(), "Abruti") { Id = Guid.NewGuid() };

  public ReadCustomizationQueryHandlerTests()
  {
    _handler = new(_customizationQuerier.Object, _permissionService.Object);

    _customizationQuerier.Setup(x => x.ReadAsync(_customization.Id, _cancellationToken)).ReturnsAsync(_customization);
  }

  [Fact(DisplayName = "It should return null when no customization is found.")]
  public async Task It_should_return_null_when_no_customization_is_found()
  {
    ReadCustomizationQuery query = new(Guid.Empty);
    Assert.Null(await _handler.Handle(query, _cancellationToken));

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(It.IsAny<ReadCustomizationQuery>(), It.IsAny<EntityMetadata>(), It.IsAny<CancellationToken>()), Times.Never);
  }

  [Fact(DisplayName = "It should return the customization found by ID.")]
  public async Task It_should_return_the_customization_found_by_Id()
  {
    ReadCustomizationQuery query = new(_customization.Id);
    CustomizationModel? customization = await _handler.Handle(query, _cancellationToken);
    Assert.Same(_customization, customization);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(
      query,
      It.Is<EntityMetadata>(y => y.WorldId.ToGuid() == _customization.World.Id && y.Key.Type == EntityType.Customization && y.Key.Id == _customization.Id && y.Size > 0),
      _cancellationToken), Times.Once);
  }
}
