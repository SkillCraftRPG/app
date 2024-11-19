using FluentValidation;
using MediatR;
using SkillCraft.Application.Castes.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Storages;
using SkillCraft.Contracts.Castes;
using SkillCraft.Domain;
using SkillCraft.Domain.Castes;

namespace SkillCraft.Application.Castes.Commands;

public record UpdateCasteCommand(Guid Id, UpdateCastePayload Payload) : Activity, IRequest<CasteModel?>;

/// <exception cref="NotEnoughAvailableStorageException"></exception>
/// <exception cref="PermissionDeniedException"></exception>
/// <exception cref="ValidationException"></exception>
internal class UpdateCasteCommandHandler : IRequestHandler<UpdateCasteCommand, CasteModel?>
{
  private readonly ICasteQuerier _casteQuerier;
  private readonly ICasteRepository _casteRepository;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;

  public UpdateCasteCommandHandler(
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

  public async Task<CasteModel?> Handle(UpdateCasteCommand command, CancellationToken cancellationToken)
  {
    UpdateCastePayload payload = command.Payload;
    new UpdateCasteValidator().ValidateAndThrow(payload);

    CasteId id = new(command.GetWorldId(), command.Id);
    Caste? caste = await _casteRepository.LoadAsync(id, cancellationToken);
    if (caste == null)
    {
      return null;
    }

    await _permissionService.EnsureCanUpdateAsync(command, caste.GetMetadata(), cancellationToken);

    if (!string.IsNullOrWhiteSpace(payload.Name))
    {
      caste.Name = new Name(payload.Name);
    }
    if (payload.Description != null)
    {
      caste.Description = Description.TryCreate(payload.Description.Value);
    }

    if (payload.Skill != null)
    {
      caste.Skill = payload.Skill.Value;
    }
    if (payload.WealthRoll != null)
    {
      caste.WealthRoll = Roll.TryCreate(payload.WealthRoll.Value);
    }

    SetFeatures(caste, payload);

    caste.Update(command.GetUserId());

    await _sender.Send(new SaveCasteCommand(caste), cancellationToken);

    return await _casteQuerier.ReadAsync(caste, cancellationToken);
  }

  private static void SetFeatures(Caste caste, UpdateCastePayload payload)
  {
    foreach (UpdateFeaturePayload featurePayload in payload.Features)
    {
      if (featurePayload.Remove)
      {
        if (featurePayload.Id.HasValue)
        {
          caste.RemoveFeature(featurePayload.Id.Value);
        }
      }
      else
      {
        Feature feature = new(new Name(featurePayload.Name), Description.TryCreate(featurePayload.Description));
        if (featurePayload.Id.HasValue)
        {
          caste.SetFeature(featurePayload.Id.Value, feature);
        }
        else
        {
          caste.AddFeature(feature);
        }
      }
    }
  }
}
