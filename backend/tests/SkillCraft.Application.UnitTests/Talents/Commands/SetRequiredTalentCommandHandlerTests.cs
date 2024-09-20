using Logitar.EventSourcing;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Talents;
using SkillCraft.Domain;
using SkillCraft.Domain.Talents;
using SkillCraft.Domain.Worlds;
using Action = SkillCraft.Application.Permissions.Action;

namespace SkillCraft.Application.Talents.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SetRequiredTalentCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ITalentRepository> _talentRepository = new();

  private readonly SetRequiredTalentCommandHandler _handler;

  private readonly WorldMock _world = new();
  private readonly Talent _requiredTalent;
  private readonly Talent _requiringTalent;

  private readonly UpdateTalentCommand _activity;

  public SetRequiredTalentCommandHandlerTests()
  {
    _handler = new(_talentRepository.Object);

    _requiredTalent = new(_world.Id, tier: 0, new Name("Mêlée"), _world.OwnerId);
    _talentRepository.Setup(x => x.LoadAsync(_requiredTalent.Id, _cancellationToken)).ReturnsAsync(_requiredTalent);

    _requiringTalent = new(_world.Id, tier: 0, new Name("Formation martiale"), _world.OwnerId);
    _requiringTalent.SetRequiredTalent(_requiredTalent);
    _requiringTalent.Update(_world.OwnerId);
    _talentRepository.Setup(x => x.LoadAsync(_requiringTalent.Id, _cancellationToken)).ReturnsAsync(_requiringTalent);

    _activity = new(_requiringTalent.Id.ToGuid(), new UpdateTalentPayload());
    _activity.Contextualize(_world);
  }

  [Fact(DisplayName = "It should nullify the required talent.")]
  public async Task It_should_nullify_the_required_talent()
  {
    SetRequiredTalentCommand command = new(_activity, _requiringTalent, Id: null);

    await _handler.Handle(command, _cancellationToken);

    Assert.Null(_requiringTalent.RequiredTalentId);
  }

  [Fact(DisplayName = "It should set the required talent.")]
  public async Task It_should_set_the_required_talent()
  {
    Talent talent = new(_world.Id, tier: 1, new Name("Cuirassé"), _world.OwnerId);
    SetRequiredTalentCommand command = new(_activity, talent, _requiringTalent.Id.ToGuid());

    await _handler.Handle(command, _cancellationToken);

    Assert.Equal(_requiringTalent.Id, talent.RequiredTalentId);
  }

  [Fact(DisplayName = "It should throw AggregateNotFoundException when the required talent could not be found.")]
  public async Task It_should_throw_AggregateNotFoundException_when_the_required_talent_could_not_be_found()
  {
    Talent talent = new(_world.Id, tier: 1, new Name("Manœuvres de combat"), _world.OwnerId);
    SetRequiredTalentCommand command = new(_activity, talent, Guid.NewGuid());
    Assert.True(command.Id.HasValue);

    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<Talent>>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(new AggregateId(command.Id.Value).Value, exception.Id);
    Assert.Equal("RequiredTalentId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw InvalidRequiredTalentTierException when requiring a talent with a higher tier.")]
  public async Task It_should_throw_InvalidRequiredTalentTierException_when_requiring_a_talent_with_a_higher_tier()
  {
    Talent talent = new(_world.Id, tier: 1, new Name("Manœuvres de combat"), _world.OwnerId);
    _talentRepository.Setup(x => x.LoadAsync(talent.Id, _cancellationToken)).ReturnsAsync(talent);

    SetRequiredTalentCommand command = new(_activity, _requiringTalent, talent.Id.ToGuid());

    var exception = await Assert.ThrowsAsync<InvalidRequiredTalentTierException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(talent.Id.ToGuid(), exception.Id);
    Assert.Equal("RequiredTalentId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when requiring a talent from another world.")]
  public async Task It_should_throw_PermissionDeniedException_when_requiring_a_talent_from_another_world()
  {
    Talent talent = new(WorldId.NewId(), tier: 0, new Name("Mêlée"), UserId.NewId());
    _talentRepository.Setup(x => x.LoadAsync(talent.Id, _cancellationToken)).ReturnsAsync(talent);

    SetRequiredTalentCommand command = new(_activity, _requiringTalent, talent.Id.ToGuid());

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(Action.Preview, exception.Action);
    Assert.Equal(EntityType.Talent, exception.EntityType);
    Assert.Equal(_world.OwnerId.ToGuid(), exception.UserId);
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(talent.Id.ToGuid(), exception.EntityId);
  }
}
