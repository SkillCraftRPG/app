using FluentValidation;
using MediatR;
using SkillCraft.Application.Castes.Validators;
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
    new ReplaceCasteValidator().ValidateAndThrow(payload);

    CasteId id = new(command.Id);
    Caste? caste = await _casteRepository.LoadAsync(id, cancellationToken);
    if (caste == null)
    {
      return null;
    }

    await _permissionService.EnsureCanUpdateAsync(command, caste.GetMetadata(), cancellationToken);

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

    SetTraits(caste, reference, payload);

    caste.Update(command.GetUserId());

    await _sender.Send(new SaveCasteCommand(caste), cancellationToken);

    return await _casteQuerier.ReadAsync(caste, cancellationToken);
  }

  private static void SetTraits(Caste caste, Caste reference, ReplaceCastePayload payload)
  {
    foreach (TraitPayload traitPayload in payload.Traits)
    {
      Trait trait = new(new Name(traitPayload.Name), Description.TryCreate(traitPayload.Description));
      if (traitPayload.Id.HasValue)
      {
        if (!reference.Traits.TryGetValue(traitPayload.Id.Value, out Trait? existingTrait) || existingTrait != trait)
        {
          caste.SetTrait(traitPayload.Id.Value, trait);
        }
      }
      else
      {
        caste.AddTrait(trait);
      }
    }

    HashSet<Guid> traitIds = payload.Traits.Where(x => x.Id.HasValue).Select(x => x.Id!.Value).ToHashSet();
    foreach (Guid traitId in reference.Traits.Keys)
    {
      if (!traitIds.Contains(traitId))
      {
        caste.RemoveTrait(traitId);
      }
    }
  }
}
