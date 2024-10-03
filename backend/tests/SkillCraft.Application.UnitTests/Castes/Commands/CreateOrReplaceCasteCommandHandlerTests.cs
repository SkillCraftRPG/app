using FluentValidation;
using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Castes;
using SkillCraft.Domain;
using SkillCraft.Domain.Castes;

namespace SkillCraft.Application.Castes.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateOrReplaceCasteCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICasteQuerier> _casteQuerier = new();
  private readonly Mock<ICasteRepository> _casteRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();

  private readonly CreateOrReplaceCasteCommandHandler _handler;

  private readonly WorldMock _world = new();
  private readonly Caste _caste;
  private readonly CasteModel _model = new();

  public CreateOrReplaceCasteCommandHandlerTests()
  {
    _handler = new(_casteQuerier.Object, _casteRepository.Object, _permissionService.Object, _sender.Object);

    _caste = new(_world.Id, new Name("classique"), _world.OwnerId);
    _caste.AddTrait(new Trait(new Name("professionnel"), Description: null));
    _caste.AddTrait(new Trait(new Name("sujet"), Description: null));
    _caste.Update(_world.OwnerId);
    _casteRepository.Setup(x => x.LoadAsync(_caste.Id, _cancellationToken)).ReturnsAsync(_caste);

    _casteQuerier.Setup(x => x.ReadAsync(It.IsAny<Caste>(), _cancellationToken)).ReturnsAsync(_model);
  }

  [Theory(DisplayName = "It should create a new caste.")]
  [InlineData(null)]
  [InlineData("f0e55347-881c-4bf8-b859-ecd119533992")]
  public async Task It_should_create_a_new_caste(string? idValue)
  {
    CreateOrReplaceCastePayload payload = new(" Artisan ")
    {
      Description = "    ",
      Skill = Skill.Craft,
      WealthRoll = "8d6"
    };
    payload.Traits.Add(new TraitPayload("Professionnel")
    {
      Description = "Les apprentissages et réalisations du personnage lui ont permis de devenir membre d’une organisation de professionnels comme lui, telle une guilde d’artisans ou de marchands. S’il ne peut payer pour un toit ou de la nourriture, il peut facilement trouver du travail afin de couvrir ces dépenses essentielles."
    });
    payload.Traits.Add(new TraitPayload("Sujet")
    {
      Id = Guid.NewGuid(),
      Description = "Sujet d’un seigneur quelconque, le personnage n’est victime d’aucune taxe imposée aux voyageurs étrangers. Il peut réduire ses dépenses essentielles de 10 % sur sa terre natale."
    });

    bool parsed = Guid.TryParse(idValue, out Guid id);
    CreateOrReplaceCasteCommand command = new(parsed ? id : null, payload, Version: null);
    command.Contextualize(_world);

    CreateOrReplaceCasteResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_model, result.Caste);
    Assert.True(result.Created);

    _permissionService.Verify(x => x.EnsureCanCreateAsync(command, EntityType.Caste, _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(
      It.Is<SaveCasteCommand>(y => (!parsed || y.Caste.EntityId == id)
        && y.Caste.Name.Value == payload.Name.Trim()
        && y.Caste.Description == null
        && y.Caste.Skill == payload.Skill
        && y.Caste.WealthRoll != null && y.Caste.WealthRoll.Value == payload.WealthRoll
        && y.Caste.Traits.Count == payload.Traits.Count
        && payload.Traits.All(t => (t.Id == null || y.Caste.Traits.ContainsKey(t.Id.Value))
          && y.Caste.Traits.Values.Any(v => v.Name.Value == t.Name && v.Description != null && v.Description.Value == t.Description))),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should replace an existing caste.")]
  public async Task It_should_replace_an_existing_caste()
  {
    CreateOrReplaceCastePayload payload = new(" Artisan ")
    {
      Description = "    ",
      Skill = Skill.Craft,
      WealthRoll = "8d6"
    };
    payload.Traits.Add(new TraitPayload("Professionnel")
    {
      Description = "Les apprentissages et réalisations du personnage lui ont permis de devenir membre d’une organisation de professionnels comme lui, telle une guilde d’artisans ou de marchands. S’il ne peut payer pour un toit ou de la nourriture, il peut facilement trouver du travail afin de couvrir ces dépenses essentielles."
    });
    payload.Traits.Add(new TraitPayload("Sujet")
    {
      Id = Guid.NewGuid(),
      Description = "Sujet d’un seigneur quelconque, le personnage n’est victime d’aucune taxe imposée aux voyageurs étrangers. Il peut réduire ses dépenses essentielles de 10 % sur sa terre natale."
    });

    CreateOrReplaceCasteCommand command = new(_caste.EntityId, payload, Version: null);
    command.Contextualize(_world);

    CreateOrReplaceCasteResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_model, result.Caste);
    Assert.False(result.Created);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Caste && y.Id == _caste.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(
      It.Is<SaveCasteCommand>(y => y.Caste.Equals(_caste)
        && y.Caste.Name.Value == payload.Name.Trim()
        && y.Caste.Description == null
        && y.Caste.Skill == payload.Skill
        && y.Caste.WealthRoll != null && y.Caste.WealthRoll.Value == payload.WealthRoll
        && y.Caste.Traits.Count == payload.Traits.Count
        && payload.Traits.All(t => (t.Id == null || y.Caste.Traits.ContainsKey(t.Id.Value))
          && y.Caste.Traits.Values.Any(v => v.Name.Value == t.Name && v.Description != null && v.Description.Value == t.Description))),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return null when updating a caste that does not exist.")]
  public async Task It_should_return_null_when_updating_an_caste_that_does_not_exist()
  {
    CreateOrReplaceCasteCommand command = new(Guid.Empty, new CreateOrReplaceCastePayload("Artisan"), Version: 0);
    command.Contextualize(_world);

    CreateOrReplaceCasteResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Null(result.Caste);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateOrReplaceCastePayload payload = new()
    {
      Skill = (Skill)(-1)
    };

    CreateOrReplaceCasteCommand command = new(Id: null, payload, Version: null);
    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Name");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "EnumValidator" && e.PropertyName == "Skill.Value");
  }

  [Fact(DisplayName = "It should update an existing caste.")]
  public async Task It_should_update_an_existing_caste()
  {
    Caste reference = new(_caste.WorldId, _caste.Name, _world.OwnerId, _caste.EntityId)
    {
      Description = _caste.Description,
      Skill = _caste.Skill,
      WealthRoll = _caste.WealthRoll
    };
    foreach (KeyValuePair<Guid, Trait> pair in _caste.Traits)
    {
      reference.SetTrait(pair.Key, pair.Value);
    }
    reference.Update(_world.OwnerId);
    _casteRepository.Setup(x => x.LoadAsync(reference.Id, reference.Version, _cancellationToken)).ReturnsAsync(reference);

    Description description = new("L’artisan est un expert d’un procédé de transformation des matières brutes. Il peut être un boulanger, un forgeron, un orfèvre, un tisserand ou pratiquer tout genre de profession œuvrant dans la transformation des matières brutes.");
    _caste.Description = description;
    Trait trait = new(new Name("Professionnel"), new Description("Les apprentissages et réalisations du personnage lui ont permis de devenir membre d’une organisation de professionnels comme lui, telle une guilde d’artisans ou de marchands. S’il ne peut payer pour un toit ou de la nourriture, il peut facilement trouver du travail afin de couvrir ces dépenses essentielles."));
    _caste.AddTrait(trait);
    _caste.Update(_world.OwnerId);

    CreateOrReplaceCastePayload payload = new(" Artisan ")
    {
      Description = "    ",
      Skill = Skill.Craft,
      WealthRoll = "8d6"
    };
    payload.Traits.Add(new TraitPayload(" Sujet ")
    {
      Id = _caste.Traits.Single(x => x.Value.Name.Value == "sujet").Key,
      Description = "  Sujet d’un seigneur quelconque, le personnage n’est victime d’aucune taxe imposée aux voyageurs étrangers. Il peut réduire ses dépenses essentielles de 10 % sur sa terre natale.  "
    });

    CreateOrReplaceCasteCommand command = new(_caste.EntityId, payload, reference.Version);
    command.Contextualize(_world);

    CreateOrReplaceCasteResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_model, result.Caste);
    Assert.False(result.Created);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Caste && y.Id == _caste.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(
      It.Is<SaveCasteCommand>(y => y.Caste.Equals(_caste)
        && y.Caste.Name.Value == payload.Name.Trim()
        && y.Caste.Description == description
        && y.Caste.Skill == payload.Skill
        && y.Caste.WealthRoll != null && y.Caste.WealthRoll.Value == payload.WealthRoll
        && y.Caste.Traits.Count == 2
        && payload.Traits.All(t => (t.Id == null || y.Caste.Traits.ContainsKey(t.Id.Value))
          && y.Caste.Traits.Values.Any(v => v.Name.Value == t.Name.Trim() && v.Description != null && t.Description != null && v.Description.Value == t.Description.Trim()))
        && y.Caste.Traits.Values.Any(t => t.Equals(trait))),
      _cancellationToken), Times.Once);
  }
}
