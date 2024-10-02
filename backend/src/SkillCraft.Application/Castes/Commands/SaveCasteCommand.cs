using FluentValidation;
using MediatR;
using SkillCraft.Application.Castes.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Storages;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Castes;
using SkillCraft.Domain;
using SkillCraft.Domain.Castes;

namespace SkillCraft.Application.Castes.Commands;

public record SaveCasteResult(CasteModel? Caste = null, bool Created = false);

public record SaveCasteCommand(Guid? Id, SaveCastePayload Payload, long? Version) : Activity, IRequest<SaveCasteResult>;

internal class SaveCasteCommandHandler : CasteCommandHandler, IRequestHandler<SaveCasteCommand, SaveCasteResult>
{
  private readonly ICasteQuerier _casteQuerier;
  private readonly ICasteRepository _casteRepository;
  private readonly IPermissionService _permissionService;

  public SaveCasteCommandHandler(
    ICasteQuerier casteQuerier,
    ICasteRepository casteRepository,
    IPermissionService permissionService,
    IStorageService storageService)
    : base(casteRepository, storageService)
  {
    _casteQuerier = casteQuerier;
    _casteRepository = casteRepository;
    _permissionService = permissionService;
  }

  public async Task<SaveCasteResult> Handle(SaveCasteCommand command, CancellationToken cancellationToken)
  {
    new SaveCasteValidator().ValidateAndThrow(command.Payload);

    Caste? caste = await FindAsync(command, cancellationToken);
    bool created = false;
    if (caste == null)
    {
      if (command.Version.HasValue)
      {
        return new SaveCasteResult();
      }

      caste = await CreateAsync(command, cancellationToken);
      created = true;
    }
    else
    {
      await ReplaceAsync(command, caste, cancellationToken);
    }

    await SaveAsync(caste, cancellationToken);

    CasteModel model = await _casteQuerier.ReadAsync(caste, cancellationToken);
    return new SaveCasteResult(model, created);
  }

  private async Task<Caste?> FindAsync(SaveCasteCommand command, CancellationToken cancellationToken)
  {
    if (!command.Id.HasValue)
    {
      return null;
    }

    CasteId id = new(command.GetWorldId(), command.Id.Value);
    return await _casteRepository.LoadAsync(id, cancellationToken);
  }

  private async Task<Caste> CreateAsync(SaveCasteCommand command, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanCreateAsync(command, EntityType.Caste, cancellationToken);

    SaveCastePayload payload = command.Payload;
    UserId userId = command.GetUserId();

    Caste caste = new(command.GetWorldId(), new Name(payload.Name), userId, command.Id)
    {
      Description = Description.TryCreate(payload.Description),
      Skill = payload.Skill,
      WealthRoll = Roll.TryCreate(payload.WealthRoll)
    };

    SetTraits(caste, caste, payload);

    caste.Update(userId);

    return caste;
  }

  private async Task ReplaceAsync(SaveCasteCommand command, Caste caste, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanUpdateAsync(command, caste.GetMetadata(), cancellationToken);

    SaveCastePayload payload = command.Payload;
    UserId userId = command.GetUserId();

    Caste? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _casteRepository.LoadAsync(caste.Id, command.Version.Value, cancellationToken);
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

    caste.Update(userId);
  }
  private static void SetTraits(Caste caste, Caste reference, SaveCastePayload payload)
  {
    HashSet<Guid> traitIds = payload.Traits.Where(x => x.Id.HasValue).Select(x => x.Id!.Value).ToHashSet();
    foreach (Guid traitId in reference.Traits.Keys)
    {
      if (!traitIds.Contains(traitId))
      {
        caste.RemoveTrait(traitId);
      }
    }

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
  }
}
