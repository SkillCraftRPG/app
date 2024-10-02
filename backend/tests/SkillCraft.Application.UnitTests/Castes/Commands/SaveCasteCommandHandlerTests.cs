using FluentValidation;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Storages;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Castes;
using SkillCraft.Domain;
using SkillCraft.Domain.Castes;

namespace SkillCraft.Application.Castes.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SaveCasteCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICasteQuerier> _casteQuerier = new();
  private readonly Mock<ICasteRepository> _casteRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly SaveCasteCommandHandler _handler;

  private readonly WorldMock _world = new();
  private readonly Caste _caste;
  private readonly CasteModel _model = new();

  public SaveCasteCommandHandlerTests()
  {
    _handler = new(_casteQuerier.Object, _casteRepository.Object, _permissionService.Object, _storageService.Object);

    _caste = new(_world.Id, new Name("classique"), _world.OwnerId);
    _casteRepository.Setup(x => x.LoadAsync(_caste.Id, _cancellationToken)).ReturnsAsync(_caste);

    _casteQuerier.Setup(x => x.ReadAsync(It.IsAny<Caste>(), _cancellationToken)).ReturnsAsync(_model);
  }

  [Theory(DisplayName = "It should create a new caste.")]
  [InlineData(null)]
  [InlineData("f0e55347-881c-4bf8-b859-ecd119533992")]
  public async Task It_should_create_a_new_caste(string? idValue)
  {
    SaveCastePayload payload = new(" Artisan ")
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
    SaveCasteCommand command = new(parsed ? id : null, payload, Version: null);
    command.Contextualize(_world);

    SaveCasteResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_model, result.Caste);
    Assert.True(result.Created);

    _permissionService.Verify(x => x.EnsureCanCreateAsync(command, EntityType.Caste, _cancellationToken), Times.Once);

    _casteRepository.Verify(x => x.SaveAsync(
      It.Is<Caste>(y => (!parsed || y.EntityId == id)
        && y.Name.Value == payload.Name.Trim() && y.Description == null
        && y.Skill == payload.Skill && y.WealthRoll != null && y.WealthRoll.Value == payload.WealthRoll
        && y.Traits.Count == payload.Traits.Count), // TODO(fpion): implement
      _cancellationToken), Times.Once);

    VerifyStorage(parsed ? id : null);
  }

  [Fact(DisplayName = "It should replace an existing caste.")]
  public async Task It_should_replace_an_existing_caste()
  {
    SaveCastePayload payload = new(" Artisan ")
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

    SaveCasteCommand command = new(_caste.EntityId, payload, Version: null);
    command.Contextualize(_world);

    SaveCasteResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_model, result.Caste);
    Assert.False(result.Created);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Caste && y.Id == _caste.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);

    _casteRepository.Verify(x => x.SaveAsync(
      It.Is<Caste>(y => y.Equals(_caste)
        && y.Name.Value == payload.Name.Trim() && y.Description == null
        && y.Skill == payload.Skill && y.WealthRoll != null && y.WealthRoll.Value == payload.WealthRoll
        && y.Traits.Count == payload.Traits.Count), // TODO(fpion): implement
      _cancellationToken), Times.Once);

    VerifyStorage(_caste.EntityId);
  }

  [Fact(DisplayName = "It should return null when updating an caste that does not exist.")]
  public async Task It_should_return_null_when_updating_an_caste_that_does_not_exist()
  {
    SaveCasteCommand command = new(Guid.Empty, new SaveCastePayload("Artisan"), Version: 0);
    command.Contextualize(_world);

    SaveCasteResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Null(result.Caste);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    SaveCastePayload payload = new()
    {
      Skill = (Skill)(-1)
    };

    SaveCasteCommand command = new(Id: null, payload, Version: null);
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
    reference.Update(_world.OwnerId);
    _casteRepository.Setup(x => x.LoadAsync(reference.Id, reference.Version, _cancellationToken)).ReturnsAsync(reference);

    Description description = new("L’artisan est un expert d’un procédé de transformation des matières brutes. Il peut être un boulanger, un forgeron, un orfèvre, un tisserand ou pratiquer tout genre de profession œuvrant dans la transformation des matières brutes.");
    _caste.Description = description;
    _caste.Update(_world.OwnerId);

    SaveCastePayload payload = new(" Artisan ")
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

    SaveCasteCommand command = new(_caste.EntityId, payload, reference.Version);
    command.Contextualize(_world);

    SaveCasteResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_model, result.Caste);
    Assert.False(result.Created);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Caste && y.Id == _caste.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);

    _casteRepository.Verify(x => x.SaveAsync(
      It.Is<Caste>(y => y.Equals(_caste)
        && y.Name.Value == payload.Name.Trim() && y.Description == description
        && y.Skill == payload.Skill && y.WealthRoll != null && y.WealthRoll.Value == payload.WealthRoll
        && y.Traits.Count == payload.Traits.Count), // TODO(fpion): implement
      _cancellationToken), Times.Once);

    VerifyStorage(_caste.EntityId);
  }

  private void VerifyStorage(Guid? id)
  {
    _storageService.Verify(x => x.EnsureAvailableAsync(
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Caste && (id == null || y.Id == id) && y.Size > 0),
      _cancellationToken), Times.Once);

    _storageService.Verify(x => x.UpdateAsync(
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Caste && (id == null || y.Id == id) && y.Size > 0),
      _cancellationToken), Times.Once);
  }
}
