using Moq;
using SkillCraft.Application.Storages;
using SkillCraft.Domain;
using SkillCraft.Domain.Parties;

namespace SkillCraft.Application.Parties.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SavePartyCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IPartyRepository> _partyRepository = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly SavePartyCommandHandler _handler;

  public SavePartyCommandHandlerTests()
  {
    _handler = new(_partyRepository.Object, _storageService.Object);
  }

  [Fact(DisplayName = "It should save the party.")]
  public async Task It_should_save_the_party()
  {
    WorldMock world = new();
    Party party = new(world.Id, new Name("Ascension"), world.OwnerId);

    SavePartyCommand command = new(party);
    await _handler.Handle(command, _cancellationToken);

    _partyRepository.Verify(x => x.SaveAsync(party, _cancellationToken), Times.Once);

    EntityMetadata entity = party.GetMetadata();
    _storageService.Verify(x => x.EnsureAvailableAsync(entity, _cancellationToken), Times.Once);
    _storageService.Verify(x => x.UpdateAsync(entity, _cancellationToken), Times.Once);
  }
}
