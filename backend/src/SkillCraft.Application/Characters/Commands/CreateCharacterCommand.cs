using FluentValidation;
using MediatR;
using SkillCraft.Application.Aspects;
using SkillCraft.Application.Castes;
using SkillCraft.Application.Characters.Creation;
using SkillCraft.Application.Characters.Validators;
using SkillCraft.Application.Customizations;
using SkillCraft.Application.Educations;
using SkillCraft.Application.Languages;
using SkillCraft.Application.Lineages;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Personalities;
using SkillCraft.Application.Storages;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain;
using SkillCraft.Domain.Aspects;
using SkillCraft.Domain.Castes;
using SkillCraft.Domain.Characters;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Educations;
using SkillCraft.Domain.Languages;
using SkillCraft.Domain.Lineages;
using SkillCraft.Domain.Personalities;
using SkillCraft.Domain.Talents;

namespace SkillCraft.Application.Characters.Commands;

/// <exception cref="AspectsNotFoundException"></exception>
/// <exception cref="CasteNotFoundException"></exception>
/// <exception cref="CustomizationsCannotIncludePersonalityGiftException"></exception>
/// <exception cref="CustomizationsNotFoundException"></exception>
/// <exception cref="EducationNotFoundException"></exception>
/// <exception cref="InvalidAspectAttributeSelectionException"></exception>
/// <exception cref="InvalidCharacterCustomizationsException"></exception>
/// <exception cref="InvalidCharacterLineageException"></exception>
/// <exception cref="InvalidExtraAttributesException"></exception>
/// <exception cref="InvalidExtraLanguagesException"></exception>
/// <exception cref="LanguagesCannotIncludeLineageLanguageException"></exception>
/// <exception cref="LanguagesNotFoundException"></exception>
/// <exception cref="LineageNotFoundException"></exception>
/// <exception cref="NotEnoughAvailableStorageException"></exception>
/// <exception cref="PermissionDeniedException"></exception>
/// <exception cref="PersonalityNotFoundException"></exception>
/// <exception cref="ValidationException"></exception>
public record CreateCharacterCommand(CreateCharacterPayload Payload) : Activity, IRequest<CharacterModel>;

internal class CreateCharacterCommandHandler : IRequestHandler<CreateCharacterCommand, CharacterModel>
{
  private readonly ICharacterQuerier _characterQuerier;
  private readonly ILineageRepository _lineageRepository;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;

  public CreateCharacterCommandHandler(
    ICharacterQuerier characterQuerier,
    ILineageRepository lineageRepository,
    IPermissionService permissionService,
    ISender sender)
  {
    _characterQuerier = characterQuerier;
    _lineageRepository = lineageRepository;
    _permissionService = permissionService;
    _sender = sender;
  }

  public async Task<CharacterModel> Handle(CreateCharacterCommand command, CancellationToken cancellationToken)
  {
    CreateCharacterPayload payload = command.Payload;
    new CreateCharacterValidator().ValidateAndThrow(payload);

    await _permissionService.EnsureCanCreateAsync(command, EntityType.Character, cancellationToken);

    Lineage lineage = await _sender.Send(new ResolveLineageQuery(command, payload.LineageId), cancellationToken);
    Lineage? parent = null;
    if (lineage.ParentId.HasValue)
    {
      parent = await _lineageRepository.LoadAsync(lineage.ParentId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The lineage 'Id={lineage.ParentId}' could not be found.");
    }

    Personality personality = await _sender.Send(new ResolvePersonalityQuery(command, payload.PersonalityId), cancellationToken);
    IReadOnlyCollection<Customization> customizations = await _sender.Send(
      new ResolveCustomizationsQuery(command, personality, payload.CustomizationIds),
      cancellationToken);
    IReadOnlyCollection<Aspect> aspects = await _sender.Send(new ResolveAspectsQuery(command, payload.AspectIds), cancellationToken);
    BaseAttributes baseAttributes = await _sender.Send(new ResolveBaseAttributesQuery(payload.Attributes, aspects, lineage, parent), cancellationToken);
    Caste caste = await _sender.Send(new ResolveCasteQuery(command, payload.CasteId), cancellationToken);
    Education education = await _sender.Send(new ResolveEducationQuery(command, payload.EducationId), cancellationToken);

    UserId userId = command.GetUserId();
    Character character = new(
      command.GetWorldId(),
      new Name(payload.Name),
      PlayerName.TryCreate(payload.Player),
      lineage,
      payload.Height,
      payload.Weight,
      payload.Age,
      personality,
      customizations,
      aspects,
      baseAttributes,
      caste,
      education,
      userId);

    IReadOnlyCollection<Language> languages = await _sender.Send(new ResolveLanguagesQuery(command, lineage, parent, payload.LanguageIds), cancellationToken);
    foreach (Language language in languages)
    {
      character.AddLanguage(language, reason: "Lineage Extra Language", userId);
    }

    IReadOnlyCollection<Talent> talents = await _sender.Send(new ResolveTalentsQuery(command, caste, education, payload.TalentIds), cancellationToken);
    foreach (Talent talent in talents)
    {
      /* TODO(fpion): character.(Add/Set)Talent
       * - Cost (2 or 1 if Aspects Skill is the same)
       * - Reason: indicate rebate, caste and education talents
       */
    }

    await _sender.Send(new SaveCharacterCommand(character), cancellationToken);

    return await _characterQuerier.ReadAsync(character, cancellationToken);
  }
}
