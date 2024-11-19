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

public record CreateOrReplaceCasteResult(CasteModel? Caste = null, bool Created = false);

/// <exception cref="NotEnoughAvailableStorageException"></exception>
/// <exception cref="PermissionDeniedException"></exception>
/// <exception cref="ValidationException"></exception>
public record CreateOrReplaceCasteCommand(Guid? Id, CreateOrReplaceCastePayload Payload, long? Version) : Activity, IRequest<CreateOrReplaceCasteResult>;

internal class CreateOrReplaceCasteCommandHandler : IRequestHandler<CreateOrReplaceCasteCommand, CreateOrReplaceCasteResult>
{
  private readonly ICasteQuerier _casteQuerier;
  private readonly ICasteRepository _casteRepository;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;

  public CreateOrReplaceCasteCommandHandler(
    ICasteQuerier casteQuerier,
    ICasteRepository casteRepository,
    IPermissionService permissionService,
    ISender sender)
  {
    _casteQuerier = casteQuerier;
    _casteRepository = casteRepository;
    _permissionService = permissionService;
    _sender = sender;
  }

  public async Task<CreateOrReplaceCasteResult> Handle(CreateOrReplaceCasteCommand command, CancellationToken cancellationToken)
  {
    new CreateOrReplaceCasteValidator().ValidateAndThrow(command.Payload);

    Caste? caste = await FindAsync(command, cancellationToken);
    bool created = false;
    if (caste == null)
    {
      if (command.Version.HasValue)
      {
        return new CreateOrReplaceCasteResult();
      }

      caste = await CreateAsync(command, cancellationToken);
      created = true;
    }
    else
    {
      await ReplaceAsync(command, caste, cancellationToken);
    }

    await _sender.Send(new SaveCasteCommand(caste), cancellationToken);

    CasteModel model = await _casteQuerier.ReadAsync(caste, cancellationToken);
    return new CreateOrReplaceCasteResult(model, created);
  }

  private async Task<Caste?> FindAsync(CreateOrReplaceCasteCommand command, CancellationToken cancellationToken)
  {
    if (!command.Id.HasValue)
    {
      return null;
    }

    CasteId id = new(command.GetWorldId(), command.Id.Value);
    return await _casteRepository.LoadAsync(id, cancellationToken);
  }

  private async Task<Caste> CreateAsync(CreateOrReplaceCasteCommand command, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanCreateAsync(command, EntityType.Caste, cancellationToken);

    CreateOrReplaceCastePayload payload = command.Payload;
    UserId userId = command.GetUserId();

    Caste caste = new(command.GetWorldId(), new Name(payload.Name), userId, command.Id)
    {
      Description = Description.TryCreate(payload.Description),
      Skill = payload.Skill,
      WealthRoll = Roll.TryCreate(payload.WealthRoll)
    };

    SetFeatures(caste, caste, payload);

    caste.Update(userId);

    return caste;
  }

  private async Task ReplaceAsync(CreateOrReplaceCasteCommand command, Caste caste, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanUpdateAsync(command, caste.GetMetadata(), cancellationToken);

    CreateOrReplaceCastePayload payload = command.Payload;
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

    SetFeatures(caste, reference, payload);

    caste.Update(userId);
  }
  private static void SetFeatures(Caste caste, Caste reference, CreateOrReplaceCastePayload payload)
  {
    HashSet<Guid> featureIds = payload.Features.Where(x => x.Id.HasValue).Select(x => x.Id!.Value).ToHashSet();
    foreach (Guid featureId in reference.Features.Keys)
    {
      if (!featureIds.Contains(featureId))
      {
        caste.RemoveFeature(featureId);
      }
    }

    foreach (FeaturePayload featurePayload in payload.Features)
    {
      Feature feature = new(new Name(featurePayload.Name), Description.TryCreate(featurePayload.Description));
      if (featurePayload.Id.HasValue)
      {
        if (!reference.Features.TryGetValue(featurePayload.Id.Value, out Feature? existingFeature) || existingFeature != feature)
        {
          caste.SetFeature(featurePayload.Id.Value, feature);
        }
      }
      else
      {
        caste.AddFeature(feature);
      }
    }
  }
}
