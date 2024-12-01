using FluentValidation.Results;
using Logitar.Security.Cryptography;
using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Talents;
using SkillCraft.Domain;
using SkillCraft.Domain.Talents;

namespace SkillCraft.Application.Talents.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class UpdateTalentCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();
  private readonly Mock<ITalentQuerier> _talentQuerier = new();
  private readonly Mock<ITalentRepository> _talentRepository = new();

  private readonly UpdateTalentCommandHandler _handler;

  private readonly WorldMock _world = new();

  public UpdateTalentCommandHandlerTests()
  {
    _handler = new(_permissionService.Object, _sender.Object, _talentQuerier.Object, _talentRepository.Object);
  }

  [Fact(DisplayName = "It should nullify the required talent.")]
  public async Task It_should_nullify_the_required_talent()
  {
    Talent requiredTalent = new(_world.Id, tier: 0, new Name("Roublardise"), _world.OwnerId);

    Talent talent = new(_world.Id, tier: 0, new Name("Trousses"), _world.OwnerId);
    talent.SetRequiredTalent(requiredTalent);
    talent.Update(_world.OwnerId);
    _talentRepository.Setup(x => x.LoadAsync(talent.Id, _cancellationToken)).ReturnsAsync(talent);

    UpdateTalentPayload payload = new()
    {
      RequiredTalentId = new Change<Guid?>(null)
    };
    UpdateTalentCommand command = new(talent.EntityId, payload);
    command.Contextualize(_world);

    TalentModel model = new();
    _talentQuerier.Setup(x => x.ReadAsync(talent, _cancellationToken)).ReturnsAsync(model);

    TalentModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    Assert.Null(talent.RequiredTalentId);
  }

  [Fact(DisplayName = "It should return null when the talent could not be found.")]
  public async Task It_should_return_null_when_the_talent_could_not_be_found()
  {
    UpdateTalentPayload payload = new();
    UpdateTalentCommand command = new(Guid.Empty, payload);
    command.Contextualize(_world);

    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw TalentNotFoundException when the required talent could not be found.")]
  public async Task It_should_throw_TalentNotFoundException_when_the_required_talent_could_not_be_found()
  {
    Talent talent = new(_world.Id, tier: 0, new Name("Trousses"), _world.OwnerId);
    _talentRepository.Setup(x => x.LoadAsync(talent.Id, _cancellationToken)).ReturnsAsync(talent);

    UpdateTalentPayload payload = new()
    {
      RequiredTalentId = new Change<Guid?>(Guid.NewGuid())
    };
    UpdateTalentCommand command = new(talent.EntityId, payload);
    command.Contextualize(_world);

    var exception = await Assert.ThrowsAsync<TalentNotFoundException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(payload.RequiredTalentId.Value, exception.TalentId);
    Assert.Equal("RequiredTalentId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    UpdateTalentPayload payload = new()
    {
      Name = RandomStringGenerator.GetString(Name.MaximumLength + 1)
    };
    UpdateTalentCommand command = new(Guid.Empty, payload);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("MaximumLengthValidator", error.ErrorCode);
    Assert.Equal("Name", error.PropertyName);
    Assert.Equal(payload.Name, error.AttemptedValue);
  }

  [Fact(DisplayName = "It should update an existing talent.")]
  public async Task It_should_update_an_existing_talent()
  {
    Talent requiredTalent = new(_world.Id, tier: 0, new Name("Tolérance à l’alcool I"), _world.OwnerId);
    _talentRepository.Setup(x => x.LoadAsync(requiredTalent.Id, _cancellationToken)).ReturnsAsync(requiredTalent);

    Talent talent = new(_world.Id, tier: 1, new Name("Résistance"), _world.OwnerId)
    {
      AllowMultiplePurchases = true
    };
    talent.Update(_world.OwnerId);
    _talentRepository.Setup(x => x.LoadAsync(talent.Id, _cancellationToken)).ReturnsAsync(talent);

    UpdateTalentPayload payload = new()
    {
      Name = " Tolérance à l’alcool II ",
      Description = new Change<string>("  Augmente de manière permanente de +1 le seuil de tolérance à l’alcool du personnage. La durée de ses gueules de bois est également réduite de moitié.  "),
      AllowMultiplePurchases = false,
      RequiredTalentId = new Change<Guid?>(requiredTalent.EntityId)
    };
    UpdateTalentCommand command = new(talent.EntityId, payload);
    command.Contextualize(_world);

    TalentModel model = new();
    _talentQuerier.Setup(x => x.ReadAsync(talent, _cancellationToken)).ReturnsAsync(model);

    TalentModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Key.Type == EntityType.Talent && y.Key.Id == talent.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);

    Assert.NotNull(payload.Description.Value);
    _sender.Verify(x => x.Send(
      It.Is<SaveTalentCommand>(y => y.Talent.Equals(talent)
        && y.Talent.Name.Value == payload.Name.Trim()
        && y.Talent.Description != null && y.Talent.Description.Value == payload.Description.Value.Trim()
        && y.Talent.AllowMultiplePurchases == payload.AllowMultiplePurchases
        && y.Talent.RequiredTalentId == requiredTalent.Id
        && y.Talent.Skill == null
      ), _cancellationToken), Times.Once);
  }
}
