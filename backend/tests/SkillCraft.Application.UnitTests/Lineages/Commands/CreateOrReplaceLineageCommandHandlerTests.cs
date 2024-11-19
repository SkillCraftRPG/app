using FluentValidation.Results;
using Logitar;
using MediatR;
using Moq;
using SkillCraft.Application.Languages.Queries;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Lineages;
using SkillCraft.Domain;
using SkillCraft.Domain.Languages;
using SkillCraft.Domain.Lineages;

namespace SkillCraft.Application.Lineages.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateOrReplaceLineageCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ILineageQuerier> _lineageQuerier = new();
  private readonly Mock<ILineageRepository> _lineageRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();

  private readonly CreateOrReplaceLineageCommandHandler _handler;

  private readonly WorldMock _world = new();

  public CreateOrReplaceLineageCommandHandlerTests()
  {
    _handler = new(_lineageQuerier.Object, _lineageRepository.Object, _permissionService.Object, _sender.Object);
  }

  [Fact(DisplayName = "It should create a new nation.")]
  public async Task It_should_create_a_new_nation()
  {
    Lineage species = new(_world.Id, parent: null, new Name("Humain"), _world.OwnerId);
    _lineageRepository.Setup(x => x.LoadAsync(species.Id, _cancellationToken)).ReturnsAsync(species);

    Language language = new(_world.Id, new Name("Orrinique"), _world.OwnerId);
    CreateOrReplaceLineagePayload payload = new(" Orrin ")
    {
      ParentId = species.EntityId,
      Description = "  L’origine noble des Orrins remonte au peuple des Thronopoi, originaire du Derebon dans l’Ancien-Monde. Ils habitent l’Orrinie, un pays situé dans une péninsule montagnarde au Sud de l’Ouespéro, entourée par la mer Aspidée et la mer Mésienne. Leur culture guerrière, architecturale et artistique a le potentiel d’être exportée dans tous les confins de l’Ouespéro. Souvent en proie à des luttes internes et déchirantes, ils sont divisés en plusieurs entités géopolitiques et États croupions. Ils sont décrits comme étant de petite taille, avec des yeux et cheveux foncés.  ",
      Languages = new() { Ids = [language.EntityId] },
      Names = new()
      {
        Family = ["Condos", "Aetos"],
        Female = ["Alexandra", "Theodora  "],
        Male = ["Ekinos", "  Radamanqus"],
        Unisex = [" Phoenix "],
        Custom = [new NameCategory("  Surnoms  ", ["de la Rivière", "du Ponton"])]
      }
    };
    CreateOrReplaceLineageCommand command = new(Id: null, payload, Version: null);
    command.Contextualize(_world);
    _sender.Setup(x => x.Send(It.Is<FindLanguagesQuery>(y => y.Activity == command && Assert.Single(y.Ids) == language.EntityId), _cancellationToken))
      .ReturnsAsync([language]);

    LineageModel model = new();
    _lineageQuerier.Setup(x => x.ReadAsync(It.IsAny<Lineage>(), _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceLineageResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(result.Lineage, model);
    Assert.True(result.Created);

    _permissionService.Verify(x => x.EnsureCanCreateAsync(command, EntityType.Lineage, _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveLineageCommand>(y => y.Lineage.ParentId == species.Id
      && y.Lineage.Name.Value == payload.Name.Trim()
      && y.Lineage.Description != null && y.Lineage.Description.Value == payload.Description.Trim()
      && y.Lineage.Attributes.AreEqualTo(payload.Attributes)
      && y.Lineage.Traits.AreEqualTo(payload.Traits)
      && y.Lineage.Languages.AreEqualTo(payload.Languages)
      && y.Lineage.Names.AreEqualTo(payload.Names)
      && y.Lineage.Speeds.AreEqualTo(payload.Speeds)
      && y.Lineage.Size.IsEqualTo(payload.Size)
      && y.Lineage.Weight.IsEqualTo(payload.Weight)
      && y.Lineage.Ages.AreEqualTo(payload.Ages)), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should create a new species.")]
  public async Task It_should_create_a_new_species()
  {
    CreateOrReplaceLineagePayload payload = new(" Humain ")
    {
      Description = "    ",
      Attributes = new() { Extra = 2 },
      Traits =
      [
        new TraitPayload("Apprentissage accéléré.") { Description = "Le personnage débute avec 4 points d’Apprentissage supplémentaires et acquiert 1 point d’Apprentissage supplémentaire à chaque fois qu’il atteint un niveau pair (2, 4, 6, 8, 10, etc.)." },
        new TraitPayload("Versatilité.") { Description = "Le personnage acquiert gratuitement un talent associé à une compétence. Ces talents portent le même nom qu’une compétence." }
      ],
      Languages = new() { Extra = 1 },
      Names = new() { Text = "les humains portent généralement un prénom et un nom de famille." },
      Speeds = new() { Walk = 6 },
      Size = new(SizeCategory.Medium, "140+2d20"),
      Weight = new()
      {
        Starved = "11+1d4",
        Skinny = "15+1d4",
        Normal = "19+1d6",
        Overweight = "26+1d6",
        Obese = "32+1d10"
      },
      Ages = new() { Adolescent = 8, Adult = 15, Mature = 21, Venerable = 35 }
    };
    CreateOrReplaceLineageCommand command = new(Guid.NewGuid(), payload, Version: null);
    command.Contextualize(_world);

    LineageModel model = new();
    _lineageQuerier.Setup(x => x.ReadAsync(It.IsAny<Lineage>(), _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceLineageResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(model, result.Lineage);
    Assert.True(result.Created);

    _permissionService.Verify(x => x.EnsureCanCreateAsync(command, EntityType.Lineage, _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveLineageCommand>(y => y.Lineage.EntityId == command.Id
      && y.Lineage.ParentId == null
      && y.Lineage.Name.Value == payload.Name.Trim() && y.Lineage.Description == null
      && y.Lineage.Attributes.AreEqualTo(payload.Attributes)
      && y.Lineage.Traits.AreEqualTo(payload.Traits)
      && y.Lineage.Languages.AreEqualTo(payload.Languages)
      && y.Lineage.Names.AreEqualTo(payload.Names)
      && y.Lineage.Speeds.AreEqualTo(payload.Speeds)
      && y.Lineage.Size.IsEqualTo(payload.Size)
      && y.Lineage.Weight.IsEqualTo(payload.Weight)
      && y.Lineage.Ages.AreEqualTo(payload.Ages)), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should replace an existing lineage.")]
  public async Task It_should_replace_an_existing_lineage()
  {
    Lineage lineage = new(_world.Id, parent: null, new Name("ashtavrin"), _world.OwnerId);
    _lineageRepository.Setup(x => x.LoadAsync(lineage.Id, _cancellationToken)).ReturnsAsync(lineage);

    Guid subjectId = Guid.NewGuid();
    CreateOrReplaceLineagePayload payload = new(" Ashtavrin ")
    {
      Description = "  Les Ashtavrins sont des humanoïdes similaires aux humains. […]  ",
      Attributes = new() { Presence = 1, Spirit = 1, Extra = 1 },
      Traits =
      [
        new TraitPayload("Double psyché") { Description = "Le personnage acquiert les capacités suivantes. […]" },
        new TraitPayload("Lien psychique") { Description = "Par une action, le personnage peut créer un lien télépathique avec une créature qu’il peut voir et située à 18 mètres (12 cases) ou moins de sa position. […]" }
      ],
      Languages = new() { Ids = [], Extra = 1 },
      Names = new()
      {
        Text = "  les Ashtavrins portent un prénom et ne portent pas de nom de famille. Cependant, le nom des parents est généralement donné à l’aîné de chaque sexe. Afin de se différencier, un nombre est utilisé comme nom de famille. Par exemple, pour une famille dont les parents sont Tigran 1er (père) et Vane 2e (mère), le fils aîné s’appellerait Tigran 2e, le second fils pourrait s’appeler Zardur 1er, la fille aînée s’appellerait Vane 3e, et la seconde fille pourrait s’appeler Taline 1ière. Ces séquences recommencent à 1 lorsqu’un nouveau prénom est utilisé ou au début d’une nouvelle lignée.  ",
        Female = ["  Anahit", "Taline"],
        Male = ["Utaron  ", "Mihr"]
      },
      Speeds = new() { Walk = 6 },
      Size = new(SizeCategory.Medium, "150+3d10"),
      Weight = new() { Starved = "11+1d4", Skinny = "15+1d4", Normal = "19+1d9", Overweight = "26+1d6", Obese = "32+1d10" },
      Ages = new() { Adolescent = 8, Adult = 15, Mature = 21, Venerable = 75 }
    };
    CreateOrReplaceLineageCommand command = new(lineage.EntityId, payload, Version: null);
    command.Contextualize(_world);

    LineageModel model = new();
    _lineageQuerier.Setup(x => x.ReadAsync(lineage, _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceLineageResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(model, result.Lineage);
    Assert.False(result.Created);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Key.Type == EntityType.Lineage && y.Key.Id == lineage.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveLineageCommand>(y => y.Lineage.Equals(lineage)
      && y.Lineage.Name.Value == payload.Name.Trim()
      && y.Lineage.Description != null && y.Lineage.Description.Value == payload.Description.CleanTrim()
      && y.Lineage.Attributes.AreEqualTo(payload.Attributes)
      && y.Lineage.Traits.AreEqualTo(payload.Traits)
      && y.Lineage.Languages.AreEqualTo(payload.Languages)
      && y.Lineage.Names.AreEqualTo(payload.Names)
      && y.Lineage.Speeds.AreEqualTo(payload.Speeds)
      && y.Lineage.Size.IsEqualTo(payload.Size)
      && y.Lineage.Weight.IsEqualTo(payload.Weight)
      && y.Lineage.Ages.AreEqualTo(payload.Ages)), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return null when updating a lineage that does not exist.")]
  public async Task It_should_return_null_when_updating_an_lineage_that_does_not_exist()
  {
    CreateOrReplaceLineageCommand command = new(Guid.Empty, new CreateOrReplaceLineagePayload("Humain"), Version: 0);
    command.Contextualize(_world);

    CreateOrReplaceLineageResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Null(result.Lineage);
  }

  [Fact(DisplayName = "It should throw InvalidParentLineageException when the parent is a nation (not a species).")]
  public async Task It_should_throw_InvalidParentLineageException_when_the_parent_is_a_nation_not_a_species()
  {
    Lineage species = new(_world.Id, parent: null, new Name("Humain"), _world.OwnerId);
    Lineage nation = new(_world.Id, species, new Name("Orrin"), _world.OwnerId);
    _lineageRepository.Setup(x => x.LoadAsync(nation.Id, _cancellationToken)).ReturnsAsync(nation);

    CreateOrReplaceLineagePayload payload = new("Sophithéon")
    {
      ParentId = nation.EntityId
    };
    CreateOrReplaceLineageCommand command = new(Id: null, payload, Version: null);
    command.Contextualize(_world);

    var exception = await Assert.ThrowsAsync<InvalidParentLineageException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(payload.ParentId.Value, exception.ParentId);
    Assert.Equal("ParentId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw LineageNotFoundException when the parent could not be found.")]
  public async Task It_should_throw_LineageNotFoundException_when_the_parent_could_not_be_found()
  {
    CreateOrReplaceLineagePayload payload = new(" Orrin ")
    {
      ParentId = Guid.NewGuid()
    };
    CreateOrReplaceLineageCommand command = new(Id: null, payload, Version: null);
    command.Contextualize(_world);

    var exception = await Assert.ThrowsAsync<LineageNotFoundException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(payload.ParentId, exception.LineageId);
    Assert.Equal("ParentId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateOrReplaceLineagePayload payload = new("Humain")
    {
      Attributes = new()
      {
        Agility = 1,
        Coordination = 1,
        Intellect = 1,
        Presence = 1,
        Sensitivity = 1,
        Spirit = 1,
        Vigor = 1,
        Extra = 1
      }
    };
    CreateOrReplaceLineageCommand command = new(Id: null, payload, Version: null);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("LessThanOrEqualValidator", error.ErrorCode);
    Assert.Equal("Attributes.Extra", error.PropertyName);
    Assert.Equal(payload.Attributes.Extra, error.AttemptedValue);
  }

  [Fact(DisplayName = "It should update an existing lineage.")]
  public async Task It_should_update_an_existing_lineage()
  {
    Lineage reference = new(_world.Id, parent: null, new Name("ashtavrin"), _world.OwnerId);
    long version = reference.Version;
    _lineageRepository.Setup(x => x.LoadAsync(reference.Id, version, _cancellationToken)).ReturnsAsync(reference);

    Lineage lineage = new(_world.Id, parent: null, reference.Name, _world.OwnerId, reference.EntityId);
    _lineageRepository.Setup(x => x.LoadAsync(lineage.Id, _cancellationToken)).ReturnsAsync(lineage);

    Description description = new("  Les Ashtavrins sont des humanoïdes similaires aux humains. […]  ");
    lineage.Description = description;
    lineage.Update(_world.OwnerId);

    CreateOrReplaceLineagePayload payload = new(" Ashtavrin ")
    {
      Description = "    ",
      Attributes = new() { Presence = 1, Spirit = 1, Extra = 1 },
      Traits =
      [
        new TraitPayload("Double psyché") { Description = "Le personnage acquiert les capacités suivantes. […]" },
        new TraitPayload("Lien psychique") { Description = "Par une action, le personnage peut créer un lien télépathique avec une créature qu’il peut voir et située à 18 mètres (12 cases) ou moins de sa position. […]" }
      ],
      Languages = new() { Ids = [], Extra = 1 },
      Names = new()
      {
        Text = "  les Ashtavrins portent un prénom et ne portent pas de nom de famille. Cependant, le nom des parents est généralement donné à l’aîné de chaque sexe. Afin de se différencier, un nombre est utilisé comme nom de famille. Par exemple, pour une famille dont les parents sont Tigran 1er (père) et Vane 2e (mère), le fils aîné s’appellerait Tigran 2e, le second fils pourrait s’appeler Zardur 1er, la fille aînée s’appellerait Vane 3e, et la seconde fille pourrait s’appeler Taline 1ière. Ces séquences recommencent à 1 lorsqu’un nouveau prénom est utilisé ou au début d’une nouvelle lignée.  ",
        Female = ["  Anahit", "Taline"],
        Male = ["Utaron  ", "Mihr"]
      },
      Speeds = new() { Walk = 6 },
      Size = new(SizeCategory.Medium, "150+3d10"),
      Weight = new() { Starved = "11+1d4", Skinny = "15+1d4", Normal = "19+1d9", Overweight = "26+1d6", Obese = "32+1d10" },
      Ages = new() { Adolescent = 8, Adult = 15, Mature = 21, Venerable = 75 }
    };
    CreateOrReplaceLineageCommand command = new(lineage.EntityId, payload, version);
    command.Contextualize(_world);

    LineageModel model = new();
    _lineageQuerier.Setup(x => x.ReadAsync(lineage, _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceLineageResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(model, result.Lineage);
    Assert.False(result.Created);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Key.Type == EntityType.Lineage && y.Key.Id == lineage.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveLineageCommand>(y => y.Lineage.Equals(lineage)
      && y.Lineage.Name.Value == payload.Name.Trim()
      && y.Lineage.Description == description
      && y.Lineage.Attributes.AreEqualTo(payload.Attributes)
      && y.Lineage.Traits.AreEqualTo(payload.Traits)
      && y.Lineage.Languages.AreEqualTo(payload.Languages)
      && y.Lineage.Names.AreEqualTo(payload.Names)
      && y.Lineage.Speeds.AreEqualTo(payload.Speeds)
      && y.Lineage.Size.IsEqualTo(payload.Size)
      && y.Lineage.Weight.IsEqualTo(payload.Weight)
      && y.Lineage.Ages.AreEqualTo(payload.Ages)), _cancellationToken), Times.Once);
  }
}
