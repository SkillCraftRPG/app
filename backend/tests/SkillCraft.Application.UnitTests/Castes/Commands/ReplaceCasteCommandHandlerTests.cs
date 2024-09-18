using FluentValidation.Results;
using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Castes;
using SkillCraft.Domain;
using SkillCraft.Domain.Castes;

namespace SkillCraft.Application.Castes.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class ReplaceCasteCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICasteQuerier> _casteQuerier = new();
  private readonly Mock<ICasteRepository> _casteRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();

  private readonly ReplaceCasteCommandHandler _handler;

  private readonly WorldMock _world = new();

  public ReplaceCasteCommandHandlerTests()
  {
    _handler = new(_casteQuerier.Object, _casteRepository.Object, _permissionService.Object, _sender.Object);
  }

  [Fact(DisplayName = "It should replace an existing caste.")]
  public async Task It_should_replace_an_existing_caste()
  {
    Caste caste = new(_world.Id, new Name("artisan"), _world.OwnerId);
    caste.AddTrait(new Trait(new Name("Autosuffisant"), Description: null));
    Guid professionalId = Guid.NewGuid();
    caste.SetTrait(professionalId, new Trait(new Name("professional"), Description: null));
    caste.Update(_world.OwnerId);
    _casteRepository.Setup(x => x.LoadAsync(caste.Id, _cancellationToken)).ReturnsAsync(caste);

    Guid subjectId = Guid.NewGuid();
    ReplaceCastePayload payload = new(" Artisan ")
    {
      Description = "  L’artisan est un expert d’un procédé de transformation des matières brutes. Il peut être un boulanger, un forgeron, un orfèvre, un tisserand ou pratiquer tout genre de profession œuvrant dans la transformation des matières brutes.  ",
      WealthRoll = "8d6",
      Traits =
      [
        new TraitPayload
        {
          Id = professionalId,
          Name = "Professionnel",
          Description = "Les apprentissages et réalisations du personnage lui ont permis de devenir membre d’une organisation de professionnels comme lui, telle une guilde d’artisans ou de marchands. S’il ne peut payer pour un toit ou de la nourriture, il peut facilement trouver du travail afin de couvrir ces dépenses essentielles."
        },
        new TraitPayload
        {
          Id = subjectId,
          Name = "Sujet",
          Description = "Sujet d’un seigneur quelconque, le personnage n’est victime d’aucune taxe imposée aux voyageurs étrangers. Il peut réduire ses dépenses essentielles de 10 % sur sa terre natale."
        }
      ]
    };
    ReplaceCasteCommand command = new(caste.Id.ToGuid(), payload, Version: null);
    command.Contextualize();

    CasteModel model = new();
    _casteQuerier.Setup(x => x.ReadAsync(caste, _cancellationToken)).ReturnsAsync(model);

    CasteModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Key.Type == EntityType.Caste && y.Key.Id == caste.Id.ToGuid() && y.Size > 0),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveCasteCommand>(y => y.Caste.Equals(caste)
      && y.Caste.Name.Value == payload.Name.Trim()
      && y.Caste.Description != null && y.Caste.Description.Value == payload.Description.Trim()
      && y.Caste.Skill == payload.Skill
      && y.Caste.WealthRoll != null && y.Caste.WealthRoll.Value == payload.WealthRoll
      && y.Caste.Traits.Count == 2
      && y.Caste.Traits[professionalId].Equals(new Trait(new Name("Professionnel"), new Description("Les apprentissages et réalisations du personnage lui ont permis de devenir membre d’une organisation de professionnels comme lui, telle une guilde d’artisans ou de marchands. S’il ne peut payer pour un toit ou de la nourriture, il peut facilement trouver du travail afin de couvrir ces dépenses essentielles.")))
      && y.Caste.Traits[subjectId].Equals(new Trait(new Name("Sujet"), new Description("Sujet d’un seigneur quelconque, le personnage n’est victime d’aucune taxe imposée aux voyageurs étrangers. Il peut réduire ses dépenses essentielles de 10 % sur sa terre natale.")))),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return null when the caste could not be found.")]
  public async Task It_should_return_null_when_the_caste_could_not_be_found()
  {
    ReplaceCastePayload payload = new("Artisan");
    ReplaceCasteCommand command = new(Guid.Empty, payload, Version: null);

    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    ReplaceCastePayload payload = new("Artisan")
    {
      WealthRoll = "8d6+"
    };
    ReplaceCasteCommand command = new(Guid.Empty, payload, Version: null);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("RollValidator", error.ErrorCode);
    Assert.Equal("WealthRoll", error.PropertyName);
    Assert.Equal(payload.WealthRoll, error.AttemptedValue);
  }

  [Fact(DisplayName = "It should update an existing caste from a reference.")]
  public async Task It_should_update_an_existing_caste_from_a_reference()
  {
    Caste reference = new(_world.Id, new Name("artisan"), _world.OwnerId);
    reference.AddTrait(new Trait(new Name("Autosuffisant"), Description: null));
    reference.Update(_world.OwnerId);
    long version = reference.Version;
    _casteRepository.Setup(x => x.LoadAsync(reference.Id, version, _cancellationToken)).ReturnsAsync(reference);

    Caste caste = new(_world.Id, reference.Name, _world.OwnerId, reference.Id);
    _casteRepository.Setup(x => x.LoadAsync(caste.Id, _cancellationToken)).ReturnsAsync(caste);

    Guid professionalId = Guid.NewGuid();
    caste.SetTrait(professionalId, new Trait(new Name("professional"), Description: null));
    Guid subjectId = Guid.NewGuid();
    caste.SetTrait(subjectId, new Trait(new Name("Sujet"), new Description("Sujet d’un seigneur quelconque, le personnage n’est victime d’aucune taxe imposée aux voyageurs étrangers. Il peut réduire ses dépenses essentielles de 10 % sur sa terre natale.")));

    Skill skill = Skill.Craft;
    caste.Skill = skill;
    caste.Update(_world.OwnerId);

    ReplaceCastePayload payload = new(" Artisan ")
    {
      Description = "  L’artisan est un expert d’un procédé de transformation des matières brutes. Il peut être un boulanger, un forgeron, un orfèvre, un tisserand ou pratiquer tout genre de profession œuvrant dans la transformation des matières brutes.  ",
      WealthRoll = "8d6",
      Traits =
      [
        new TraitPayload
        {
          Id = professionalId,
          Name = "Professionnel",
          Description = "Les apprentissages et réalisations du personnage lui ont permis de devenir membre d’une organisation de professionnels comme lui, telle une guilde d’artisans ou de marchands. S’il ne peut payer pour un toit ou de la nourriture, il peut facilement trouver du travail afin de couvrir ces dépenses essentielles."
        }
      ]
    };
    ReplaceCasteCommand command = new(caste.Id.ToGuid(), payload, version);
    command.Contextualize();

    CasteModel model = new();
    _casteQuerier.Setup(x => x.ReadAsync(caste, _cancellationToken)).ReturnsAsync(model);

    CasteModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Key.Type == EntityType.Caste && y.Key.Id == caste.Id.ToGuid() && y.Size > 0),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveCasteCommand>(y => y.Caste.Equals(caste)
      && y.Caste.Name.Value == payload.Name.Trim()
      && y.Caste.Description != null && y.Caste.Description.Value == payload.Description.Trim()
      && y.Caste.Skill == Skill.Craft
      && y.Caste.WealthRoll != null && y.Caste.WealthRoll.Value == payload.WealthRoll
      && y.Caste.Traits.Count == 2
      && y.Caste.Traits[professionalId].Equals(new Trait(new Name("Professionnel"), new Description("Les apprentissages et réalisations du personnage lui ont permis de devenir membre d’une organisation de professionnels comme lui, telle une guilde d’artisans ou de marchands. S’il ne peut payer pour un toit ou de la nourriture, il peut facilement trouver du travail afin de couvrir ces dépenses essentielles.")))
      && y.Caste.Traits[subjectId].Equals(new Trait(new Name("Sujet"), new Description("Sujet d’un seigneur quelconque, le personnage n’est victime d’aucune taxe imposée aux voyageurs étrangers. Il peut réduire ses dépenses essentielles de 10 % sur sa terre natale.")))),
      _cancellationToken), Times.Once);
  }
}
