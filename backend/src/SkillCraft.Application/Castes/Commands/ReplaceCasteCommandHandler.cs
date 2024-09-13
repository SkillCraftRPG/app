using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Castes;
using SkillCraft.Domain;
using SkillCraft.Domain.Castes;

namespace SkillCraft.Application.Castes.Commands;

public record ReplaceCasteCommand(Guid Id, ReplaceCastePayload Payload, long? Version) : Activity, IRequest<CasteModel?>;

internal class ReplaceCasteCommandHandler : IRequestHandler<ReplaceCasteCommand, CasteModel?>
{
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;
  private readonly ICasteQuerier _casteQuerier;
  private readonly ICasteRepository _casteRepository;

  public ReplaceCasteCommandHandler(
    ICasteQuerier casteQuerier,
    ICasteRepository casteRepository,
    IPermissionService permissionService,
    ISender sender)
  {
    _permissionService = permissionService;
    _sender = sender;
    _casteQuerier = casteQuerier;
    _casteRepository = casteRepository;
  }

  public async Task<CasteModel?> Handle(ReplaceCasteCommand command, CancellationToken cancellationToken)
  {
    ReplaceCastePayload payload = command.Payload;
    //new ReplaceCasteValidator().ValidateAndThrow(payload); // TODO(fpion): implement

    CasteId id = new(command.Id);
    Caste? caste = await _casteRepository.LoadAsync(id, cancellationToken);
    if (caste == null)
    {
      return null;
    }

    await _permissionService.EnsureCanUpdateAsync(command, EntityMetadata.From(caste), cancellationToken);

    Caste? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _casteRepository.LoadAsync(id, command.Version.Value, cancellationToken);
    }
    reference ??= caste;

    Name name = new(payload.Name);
    if (name != reference.Name)
    {
      caste.Name = name;
    }
    Description? description = Description.TryCreate(payload.Description);
    if (description != reference.Description)
    {
      caste.Description = description;
    }

    if (payload.Skill != reference.Skill)
    {
      caste.Skill = payload.Skill;
    }
    Roll? wealthRoll = Roll.TryCreate(payload.WealthRoll);
    if (wealthRoll != reference.WealthRoll)
    {
      caste.WealthRoll = wealthRoll;
    }

    // TODO(fpion): Traits
    caste.Update(command.GetUserId());

    await _sender.Send(new SaveCasteCommand(caste), cancellationToken);

    return await _casteQuerier.ReadAsync(caste, cancellationToken);
  }
}
