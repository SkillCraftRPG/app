using Moq;
using SkillCraft.Application.Storages;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Domain;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Customizations.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SaveCustomizationCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Customization _customization = new(WorldId.NewId(), CustomizationType.Gift, new Name("Aigrefin"), UserId.NewId());

  private readonly Mock<ICustomizationRepository> _customizationRepository = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly SaveCustomizationCommandHandler _handler;

  public SaveCustomizationCommandHandlerTests()
  {
    _handler = new(_customizationRepository.Object, _storageService.Object);
  }

  [Fact(DisplayName = "It should save the customization.")]
  public async Task It_should_save_the_customization()
  {
    SaveCustomizationCommand command = new(_customization);

    await _handler.Handle(command, _cancellationToken);

    _customizationRepository.Verify(x => x.SaveAsync(_customization, _cancellationToken), Times.Once);

    EntityMetadata entity = _customization.GetMetadata();
    _storageService.Verify(x => x.EnsureAvailableAsync(entity, _cancellationToken), Times.Once);
    _storageService.Verify(x => x.UpdateAsync(entity, _cancellationToken), Times.Once);
  }
}
