using SkillCraft.Application.Storages;
using SkillCraft.Domain.Parties;

namespace SkillCraft.Application.Parties.Commands;

internal abstract class PartyCommandHandler
{
  private readonly IPartyRepository _partyRepository;
  private readonly IStorageService _storageService;

  public PartyCommandHandler(IPartyRepository partyRepository, IStorageService storageService)
  {
    _partyRepository = partyRepository;
    _storageService = storageService;
  }

  protected async Task SaveAsync(Party party, CancellationToken cancellationToken)
  {
    EntityMetadata entity = party.GetMetadata();
    await _storageService.EnsureAvailableAsync(entity, cancellationToken);

    await _partyRepository.SaveAsync(party, cancellationToken);

    await _storageService.UpdateAsync(entity, cancellationToken);
  }
}
