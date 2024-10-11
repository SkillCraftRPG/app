using Logitar;
using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Lineages;
using SkillCraft.Domain;
using SkillCraft.Domain.Lineages;

namespace SkillCraft.Application.Lineages.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class ReplaceLineageCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ILineageQuerier> _lineageQuerier = new();
  private readonly Mock<ILineageRepository> _lineageRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();

  private readonly ReplaceLineageCommandHandler _handler;

  private readonly WorldMock _world = new();

  public ReplaceLineageCommandHandlerTests()
  {
    _handler = new(_lineageQuerier.Object, _lineageRepository.Object, _permissionService.Object, _sender.Object);
  }

  [Fact(DisplayName = "It should replace an existing lineage.")]
  public async Task It_should_replace_an_existing_lineage()
  {
    Lineage lineage = new(_world.Id, parent: null, new Name("ashtavrin"), _world.OwnerId);
    _lineageRepository.Setup(x => x.LoadAsync(lineage.Id, _cancellationToken)).ReturnsAsync(lineage);

    Guid subjectId = Guid.NewGuid();
    ReplaceLineagePayload payload = new(" Ashtavrin ")
    {
      Description = "  Les Ashtavrins sont des humanoïdes similaires aux humains. […]  ",
      Attributes = new() { Presence = 1, Spirit = 1, Extra = 1 },
      Features =
      [
        new FeaturePayload("Double psyché") { Description = "Le personnage acquiert les capacités suivantes. […]" },
        new FeaturePayload("Lien psychique") { Description = "Par une action, le personnage peut créer un lien télépathique avec une créature qu’il peut voir et située à 18 mètres (12 cases) ou moins de sa position. […]" }
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
    ReplaceLineageCommand command = new(lineage.EntityId, payload, Version: null);
    command.Contextualize(_world);

    LineageModel model = new();
    _lineageQuerier.Setup(x => x.ReadAsync(lineage, _cancellationToken)).ReturnsAsync(model);

    LineageModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Key.Type == EntityType.Lineage && y.Key.Id == lineage.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveLineageCommand>(y => y.Lineage.Equals(lineage)
      && y.Lineage.Name.Value == payload.Name.Trim()
      && y.Lineage.Description != null && y.Lineage.Description.Value == payload.Description.CleanTrim()
      && y.Lineage.Attributes.AreEqualTo(payload.Attributes)
      && y.Lineage.Features.AreEqualTo(payload.Features)
      && y.Lineage.Languages.AreEqualTo(payload.Languages)
      && y.Lineage.Names.AreEqualTo(payload.Names)
      && y.Lineage.Speeds.AreEqualTo(payload.Speeds)
      && y.Lineage.Size.IsEqualTo(payload.Size)
      && y.Lineage.Weight.IsEqualTo(payload.Weight)
      && y.Lineage.Ages.AreEqualTo(payload.Ages)), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return null when the lineage could not be found.")]
  public async Task It_should_return_null_when_the_lineage_could_not_be_found()
  {
    ReplaceLineagePayload payload = new("Ashtavrin");
    ReplaceLineageCommand command = new(Guid.Empty, payload, Version: null);
    command.Contextualize(_world);

    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    ReplaceLineagePayload payload = new("Ashtavrin")
    {
      Names = new()
      {
        Custom = [new NameCategory()]
      },
      Speeds = new() { Walk = 30 },
      Size = new((SizeCategory)(-1), "140-2d20"),
      Ages = new() { Adolescent = 0, Adult = 21, Mature = 14, Venerable = null }
    };
    ReplaceLineageCommand command = new(Guid.Empty, payload, Version: null);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    Assert.Equal(5, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && (string?)e.AttemptedValue == string.Empty && e.PropertyName == "Names.Custom[0].Key");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "InclusiveBetweenValidator" && (int?)e.AttemptedValue == 30 && e.PropertyName == "Speeds.Walk");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "RollValidator" && (string?)e.AttemptedValue == "140-2d20" && e.PropertyName == "Size.Roll");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "GreaterThanValidator" && (int?)e.AttemptedValue == 0 && e.PropertyName == "Ages.Adolescent.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "GreaterThanValidator" && (int?)e.AttemptedValue == 14 && e.PropertyName == "Ages.Mature.Value");
  }

  [Fact(DisplayName = "It should update an existing lineage from a reference.")]
  public async Task It_should_update_an_existing_lineage_from_a_reference()
  {
    Lineage reference = new(_world.Id, parent: null, new Name("ashtavrin"), _world.OwnerId);
    long version = reference.Version;
    _lineageRepository.Setup(x => x.LoadAsync(reference.Id, version, _cancellationToken)).ReturnsAsync(reference);

    Lineage lineage = new(_world.Id, parent: null, reference.Name, _world.OwnerId, reference.EntityId);
    _lineageRepository.Setup(x => x.LoadAsync(lineage.Id, _cancellationToken)).ReturnsAsync(lineage);

    Description description = new("  Les Ashtavrins sont des humanoïdes similaires aux humains. […]  ");
    lineage.Description = description;
    lineage.Update(_world.OwnerId);

    ReplaceLineagePayload payload = new(" Ashtavrin ")
    {
      Description = "    ",
      Attributes = new() { Presence = 1, Spirit = 1, Extra = 1 },
      Features =
      [
        new FeaturePayload("Double psyché") { Description = "Le personnage acquiert les capacités suivantes. […]" },
        new FeaturePayload("Lien psychique") { Description = "Par une action, le personnage peut créer un lien télépathique avec une créature qu’il peut voir et située à 18 mètres (12 cases) ou moins de sa position. […]" }
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
    ReplaceLineageCommand command = new(lineage.EntityId, payload, version);
    command.Contextualize(_world);

    LineageModel model = new();
    _lineageQuerier.Setup(x => x.ReadAsync(lineage, _cancellationToken)).ReturnsAsync(model);

    LineageModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Key.Type == EntityType.Lineage && y.Key.Id == lineage.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveLineageCommand>(y => y.Lineage.Equals(lineage)
      && y.Lineage.Name.Value == payload.Name.Trim()
      && y.Lineage.Description == description
      && y.Lineage.Attributes.AreEqualTo(payload.Attributes)
      && y.Lineage.Features.AreEqualTo(payload.Features)
      && y.Lineage.Languages.AreEqualTo(payload.Languages)
      && y.Lineage.Names.AreEqualTo(payload.Names)
      && y.Lineage.Speeds.AreEqualTo(payload.Speeds)
      && y.Lineage.Size.IsEqualTo(payload.Size)
      && y.Lineage.Weight.IsEqualTo(payload.Weight)
      && y.Lineage.Ages.AreEqualTo(payload.Ages)), _cancellationToken), Times.Once);
  }
}
