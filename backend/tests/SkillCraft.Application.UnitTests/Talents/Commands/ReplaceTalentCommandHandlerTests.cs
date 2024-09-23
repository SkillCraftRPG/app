using FluentValidation.Results;
using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Talents;
using SkillCraft.Domain;
using SkillCraft.Domain.Talents;

namespace SkillCraft.Application.Talents.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class ReplaceTalentCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();
  private readonly Mock<ITalentQuerier> _talentQuerier = new();
  private readonly Mock<ITalentRepository> _talentRepository = new();

  private readonly ReplaceTalentCommandHandler _handler;

  private readonly WorldMock _world = new();

  public ReplaceTalentCommandHandlerTests()
  {
    _handler = new(_permissionService.Object, _sender.Object, _talentQuerier.Object, _talentRepository.Object);
  }

  [Fact(DisplayName = "It should replace an existing talent.")]
  public async Task It_should_replace_an_existing_talent()
  {
    Talent required = new(_world.Id, tier: 0, new Name("Mêlée"), _world.OwnerId);
    Talent talent = new(_world.Id, tier: 1, new Name("Orientation"), _world.OwnerId)
    {
      AllowMultiplePurchases = true
    };
    talent.SetRequiredTalent(required);
    talent.Update(_world.OwnerId);
    _talentRepository.Setup(x => x.LoadAsync(talent.Id, _cancellationToken)).ReturnsAsync(talent);

    Guid subjectId = Guid.NewGuid();
    ReplaceTalentPayload payload = new(" Armes de finesse ")
    {
      Description = "  Lorsque le personnage effectue une attaque en maniant une arme de finesse, alors il peut remplacer le test de _Mêlée_ par un test d’_Orientation_. Si l’attaque réussit, il ajoute sa Précision aux points de dégâts infligés plutôt que sa Force. Également, il se voit conférer un bonus de +2 aux points de dégâts infligés en maniant une arme de finesse lorsqu’il ne tient aucune autre arme.  ",
      AllowMultiplePurchases = false,
      RequiredTalentId = null
    };
    ReplaceTalentCommand command = new(talent.Id.ToGuid(), payload, Version: null);
    command.Contextualize();

    TalentModel model = new();
    _talentQuerier.Setup(x => x.ReadAsync(talent, _cancellationToken)).ReturnsAsync(model);

    TalentModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Key.Type == EntityType.Talent && y.Key.Id == talent.Id.ToGuid() && y.Size > 0),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SetRequiredTalentCommand>(y => y.Activity == command && y.Talent == talent && y.Id == null), _cancellationToken), Times.Once);
    _sender.Verify(x => x.Send(It.Is<SaveTalentCommand>(y => y.Talent.Equals(talent)
      && y.Talent.Name.Value == payload.Name.Trim()
      && y.Talent.Description != null && y.Talent.Description.Value == payload.Description.Trim()
      && y.Talent.AllowMultiplePurchases == payload.AllowMultiplePurchases
      && y.Talent.Skill == null
      ), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return null when the talent could not be found.")]
  public async Task It_should_return_null_when_the_talent_could_not_be_found()
  {
    ReplaceTalentPayload payload = new("Furtivité");
    ReplaceTalentCommand command = new(Guid.Empty, payload, Version: null);

    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    ReplaceTalentPayload payload = new("Intuition")
    {
      RequiredTalentId = Guid.Empty
    };
    ReplaceTalentCommand command = new(Guid.Empty, payload, Version: null);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("NotEmptyValidator", error.ErrorCode);
    Assert.Equal("RequiredTalentId.Value", error.PropertyName);
    Assert.Equal(payload.RequiredTalentId, error.AttemptedValue);
  }

  [Fact(DisplayName = "It should update an existing talent from a reference.")]
  public async Task It_should_update_an_existing_talent_from_a_reference()
  {
    Talent reference = new(_world.Id, tier: 1, new Name("tir-rapide"), _world.OwnerId)
    {
      AllowMultiplePurchases = true
    };
    reference.Update(_world.OwnerId);
    long version = reference.Version;
    _talentRepository.Setup(x => x.LoadAsync(reference.Id, version, _cancellationToken)).ReturnsAsync(reference);

    Talent talent = new(_world.Id, reference.Tier, reference.Name, _world.OwnerId, reference.Id);
    _talentRepository.Setup(x => x.LoadAsync(talent.Id, _cancellationToken)).ReturnsAsync(talent);

    Description description = new("  Permet au personnage d’effectuer l’activité **Viser et tirer** au coût d’une seule action plutôt que deux lorsqu’il utilise une arme possédant la propriété **Munition**. Également, grâce à la rapidité de ses tirs, il ne déclenche pas d’attaque d’opportunité lorsqu’il effectue une attaque à distance en utilisant une de ces armes.  ");
    talent.Description = description;
    talent.Update(_world.OwnerId);

    ReplaceTalentPayload payload = new(" Tir rapide ")
    {
      Description = "    ",
      AllowMultiplePurchases = false,
      RequiredTalentId = Guid.NewGuid()
    };
    ReplaceTalentCommand command = new(talent.Id.ToGuid(), payload, version);
    command.Contextualize();

    TalentModel model = new();
    _talentQuerier.Setup(x => x.ReadAsync(talent, _cancellationToken)).ReturnsAsync(model);

    TalentModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Key.Type == EntityType.Talent && y.Key.Id == talent.Id.ToGuid() && y.Size > 0),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SetRequiredTalentCommand>(y => y.Activity == command && y.Talent == talent
      && y.Id == payload.RequiredTalentId), _cancellationToken), Times.Once);
    _sender.Verify(x => x.Send(It.Is<SaveTalentCommand>(y => y.Talent.Equals(talent)
      && y.Talent.Name.Value == payload.Name.Trim()
      && y.Talent.Description != null && y.Talent.Description == description
      && y.Talent.AllowMultiplePurchases == payload.AllowMultiplePurchases
      ), _cancellationToken), Times.Once);
  }
}
