﻿using FluentValidation;
using MediatR;
using SkillCraft.Application.Aspects;
using SkillCraft.Application.Castes;
using SkillCraft.Application.Characters.Creation;
using SkillCraft.Application.Characters.Validators;
using SkillCraft.Application.Customizations;
using SkillCraft.Application.Educations;
using SkillCraft.Application.Items;
using SkillCraft.Application.Languages;
using SkillCraft.Application.Lineages;
using SkillCraft.Application.Natures;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Storages;
using SkillCraft.Application.Talents;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain;
using SkillCraft.Domain.Aspects;
using SkillCraft.Domain.Castes;
using SkillCraft.Domain.Characters;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Educations;
using SkillCraft.Domain.Items;
using SkillCraft.Domain.Languages;
using SkillCraft.Domain.Lineages;
using SkillCraft.Domain.Natures;
using SkillCraft.Domain.Talents;

namespace SkillCraft.Application.Characters.Commands;

/// <exception cref="AspectsNotFoundException"></exception>
/// <exception cref="CasteHasNoSkillTalentException"></exception>
/// <exception cref="CasteNotFoundException"></exception>
/// <exception cref="CustomizationsCannotIncludeNatureGiftException"></exception>
/// <exception cref="CustomizationsNotFoundException"></exception>
/// <exception cref="EducationHasNoSkillTalentException"></exception>
/// <exception cref="EducationNotFoundException"></exception>
/// <exception cref="InvalidAspectAttributeSelectionException"></exception>
/// <exception cref="InvalidCasteEducationSelectionException"></exception>
/// <exception cref="InvalidCharacterCustomizationsException"></exception>
/// <exception cref="InvalidCharacterLineageException"></exception>
/// <exception cref="InvalidExtraAttributesException"></exception>
/// <exception cref="InvalidExtraLanguagesException"></exception>
/// <exception cref="InvalidSkillTalentSelectionException"></exception>
/// <exception cref="InvalidStartingWealthSelectionException"></exception>
/// <exception cref="ItemNotFoundException"></exception>
/// <exception cref="LanguagesCannotIncludeLineageLanguageException"></exception>
/// <exception cref="LanguagesNotFoundException"></exception>
/// <exception cref="LineageNotFoundException"></exception>
/// <exception cref="NatureNotFoundException"></exception>
/// <exception cref="NotEnoughAvailableStorageException"></exception>
/// <exception cref="PermissionDeniedException"></exception>
/// <exception cref="TalentsNotFoundException"></exception>
/// <exception cref="ValidationException"></exception>
public record CreateCharacterCommand(Guid? Id, CreateCharacterPayload Payload) : Activity, IRequest<CharacterModel>;

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

    Nature nature = await _sender.Send(new ResolveNatureQuery(command, payload.NatureId), cancellationToken);
    IReadOnlyCollection<Customization> customizations = await _sender.Send(
      new ResolveCustomizationsQuery(command, nature, payload.CustomizationIds),
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
      species: parent ?? lineage,
      nation: parent == null ? null : lineage,
      payload.Height,
      payload.Weight,
      payload.Age,
      nature,
      customizations,
      aspects,
      baseAttributes,
      caste,
      education,
      userId,
      command.Id);

    IReadOnlyCollection<Language> languages = await _sender.Send(new ResolveLanguagesQuery(command, lineage, parent, payload.LanguageIds), cancellationToken);
    foreach (Language language in languages)
    {
      Description notes = new("Lineage Extra Language");
      character.SetLanguage(language, notes, userId);
    }

    IReadOnlyDictionary<Skill, Aspect> discountedSkills = GetDiscountedSkills(aspects);
    IReadOnlyCollection<Talent> talents = await _sender.Send(new ResolveTalentsQuery(command, caste, education, payload.TalentIds), cancellationToken);
    foreach (Talent talent in talents)
    {
      if (!talent.Skill.HasValue)
      {
        throw new InvalidOperationException($"The talent '{talent}' is not associated to a skill.");
      }

      List<string> notes = new(capacity: 3);

      SetTalentOptions options = new()
      {
        Cost = talent.MaximumCost
      };
      if (talent.Skill == caste.Skill)
      {
        notes.Add($"Caste: {caste.Name}");
      }
      if (talent.Skill == education.Skill)
      {
        notes.Add($"Education: {education.Name}");
      }
      if (discountedSkills.TryGetValue(talent.Skill.Value, out Aspect? aspect))
      {
        options.Cost--;
        notes.Add($"Discounted by Aspect: {aspect.Name}");
      }
      if (notes.Count > 0)
      {
        options.Notes = new Description(string.Join("; ", notes));
      }
      character.AddTalent(talent, options, userId);
    }

    if (payload.StartingWealth != null)
    {
      Item item = await _sender.Send(new ResolveItemQuery(command, payload.StartingWealth.ItemId), cancellationToken);
      SetItemOptions options = new()
      {
        Quantity = payload.StartingWealth.Quantity
      };
      character.AddItem(item, options, userId);
    }

    await _sender.Send(new SaveCharacterCommand(character), cancellationToken);

    return await _characterQuerier.ReadAsync(character, cancellationToken);
  }

  private static IReadOnlyDictionary<Skill, Aspect> GetDiscountedSkills(IEnumerable<Aspect> aspects)
  {
    Dictionary<Skill, Aspect> skills = [];
    foreach (Aspect aspect in aspects)
    {
      if (aspect.Skills.Discounted1.HasValue)
      {
        skills[aspect.Skills.Discounted1.Value] = aspect;
      }
      if (aspect.Skills.Discounted2.HasValue)
      {
        skills[aspect.Skills.Discounted2.Value] = aspect;
      }
    }
    return skills.AsReadOnly();
  }
}
