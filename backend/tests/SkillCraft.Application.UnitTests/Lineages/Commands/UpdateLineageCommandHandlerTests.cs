using FluentValidation.Results;
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
public class UpdateLineageCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ILineageQuerier> _lineageQuerier = new();
  private readonly Mock<ILineageRepository> _lineageRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();

  private readonly UpdateLineageCommandHandler _handler;

  private readonly WorldMock _world = new();

  public UpdateLineageCommandHandlerTests()
  {
    _handler = new(_lineageQuerier.Object, _lineageRepository.Object, _permissionService.Object, _sender.Object);
  }

  [Fact(DisplayName = "It should return null when the lineage could not be found.")]
  public async Task It_should_return_null_when_the_lineage_could_not_be_found()
  {
    UpdateLineagePayload payload = new();
    UpdateLineageCommand command = new(Guid.Empty, payload);

    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    UpdateLineagePayload payload = new()
    {
      Languages = new UpdateLanguagesPayload
      {
        Extra = 10
      }
    };
    UpdateLineageCommand command = new(Guid.Empty, payload);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("InclusiveBetweenValidator", error.ErrorCode);
    Assert.Equal("Languages.Extra.Value", error.PropertyName);
    Assert.Equal(payload.Languages.Extra, error.AttemptedValue);
  }

  [Fact(DisplayName = "It should update an existing lineage.")]
  public async Task It_should_update_an_existing_lineage()
  {
    Lineage lineage = new(_world.Id, parent: null, new Name("gnome"), _world.OwnerId);
    lineage.AddFeature(new Feature(new Name("Bizarrerie"), Description: null));
    lineage.Update(_world.OwnerId);
    _lineageRepository.Setup(x => x.LoadAsync(lineage.Id, _cancellationToken)).ReturnsAsync(lineage);

    UpdateFeaturePayload feature = new(" Curieux ") { Description = "  Le personnage peut acquérir à rabais le talent <u>Investigation</u>.  " };
    UpdateLineagePayload payload = new()
    {
      Name = " Gnome ",
      Description = new Change<string>("  Les gnomes sont des humanoïdes de petite taille, généralement poilus et barbus, similairement aux nains. Ce sont des élémentaires associés à la Terre qu’on peut retrouver dans d’étonnants habitats partageant une certaine clandestinité, à l’écart des regards et de la lumière du grand jour. Les gnomes sont des êtres curieux et habiles de leurs mains, possédant une certaine vivacité d’esprit ainsi qu’une bonne résilience face à la magie.  "),
      Attributes = new UpdateAttributeBonusesPayload
      {
        Coordination = 1,
        Intellect = 1
      },
      Features =
      [
        new UpdateFeaturePayload(lineage.Features.Values.Single().Name.Value) { Id = lineage.Features.Keys.Single(), Remove = true },
        feature
      ],
      Languages = new UpdateLanguagesPayload { Extra = 1 },
      Names = new UpdateNamesPayload
      {
        Text = new Change<string>("  les gnomes adorent les noms. Ils portent généralement deux prénoms, chacun lui étant assigné par un de ses parents, en plus de porter le nom de famille de chacun de ses parents. De plus, ils peuvent se voir assigner plusieurs surnoms par leurs proches. Ces surnoms peuvent représenter leurs exploits ou encore leurs gaffes.  ")
      },
      Speeds = new UpdateSpeedsPayload { Walk = 4 },
      Size = new UpdateSizePayload { Category = SizeCategory.Small, Roll = new Change<string>(" 90+3d10 ") }
    };
    UpdateLineageCommand command = new(lineage.Id.ToGuid(), payload);
    command.Contextualize();

    LineageModel model = new();
    _lineageQuerier.Setup(x => x.ReadAsync(lineage, _cancellationToken)).ReturnsAsync(model);

    LineageModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Key.Type == EntityType.Lineage && y.Key.Id == lineage.Id.ToGuid() && y.Size > 0),
      _cancellationToken), Times.Once);

    Assert.NotNull(payload.Description.Value);
    Assert.NotNull(payload.Names.Text.Value);
    Assert.NotNull(payload.Size.Roll.Value);
    _sender.Verify(x => x.Send(It.Is<SaveLineageCommand>(y => y.Lineage.Equals(lineage)
      && y.Lineage.Name.Value == payload.Name.Trim()
      && y.Lineage.Description != null && y.Lineage.Description.Value == payload.Description.Value.Trim()
      && y.Lineage.Attributes.Coordination == payload.Attributes.Coordination && y.Lineage.Attributes.Intellect == payload.Attributes.Intellect
      && AreEqual(y.Lineage.Features.Single(), feature)
      && y.Lineage.Languages.Extra == 1
      && y.Lineage.Names.Text == payload.Names.Text.Value.CleanTrim()
      && y.Lineage.Speeds.Walk == payload.Speeds.Walk
      && y.Lineage.Size.Category == payload.Size.Category
      && y.Lineage.Size.Roll != null && y.Lineage.Size.Roll.Value == payload.Size.Roll.Value.Trim()), _cancellationToken), Times.Once);
  }
  private static bool AreEqual(KeyValuePair<Guid, Feature> feature, UpdateFeaturePayload payload) => (!payload.Id.HasValue || payload.Id.Value == feature.Key)
    && feature.Value.Name.Value == payload.Name.Trim()
    && feature.Value.Description?.Value == payload.Description?.CleanTrim();
}
