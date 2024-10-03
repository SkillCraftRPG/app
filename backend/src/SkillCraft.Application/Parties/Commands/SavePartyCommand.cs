using MediatR;
using SkillCraft.Application.Storages;
using SkillCraft.Domain.Parties;

namespace SkillCraft.Application.Parties.Commands;

internal record SavePartyCommand(Party Party) : IRequest;

internal class SavePartyCommandHandler : IRequestHandler<SavePartyCommand>
{
  private readonly IPartyRepository _partyRepository;
  private readonly IStorageService _storageService;

  public SavePartyCommandHandler(IPartyRepository partyRepository, IStorageService storageService)
  {
    _partyRepository = partyRepository;
    _storageService = storageService;
  }

  public async Task Handle(SavePartyCommand command, CancellationToken cancellationToken)
  {
    Party party = command.Party;

    EntityMetadata entity = party.GetMetadata();
    await _storageService.EnsureAvailableAsync(entity, cancellationToken);

    await _partyRepository.SaveAsync(party, cancellationToken);

    await _storageService.UpdateAsync(entity, cancellationToken);
  }
}
