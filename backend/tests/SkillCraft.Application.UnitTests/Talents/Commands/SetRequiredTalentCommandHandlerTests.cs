using Moq;
using SkillCraft.Domain;
using SkillCraft.Domain.Talents;

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

  public SetRequiredTalentCommandHandlerTests()
  {
    _handler = new(_talentRepository.Object);

    _requiredTalent = new(_world.Id, tier: 0, new Name("Mêlée"), _world.OwnerId);
    _talentRepository.Setup(x => x.LoadAsync(_requiredTalent.Id, _cancellationToken)).ReturnsAsync(_requiredTalent);

    _requiringTalent = new(_world.Id, tier: 0, new Name("Formation martiale"), _world.OwnerId);
    _requiringTalent.SetRequiredTalent(_requiredTalent);
    _requiringTalent.Update(_world.OwnerId);
    _talentRepository.Setup(x => x.LoadAsync(_requiringTalent.Id, _cancellationToken)).ReturnsAsync(_requiringTalent);
  }

  [Fact(DisplayName = "It should nullify the required talent.")]
  public async Task It_should_nullify_the_required_talent()
  {
    SetRequiredTalentCommand command = new(_requiringTalent, Id: null);

    await _handler.Handle(command, _cancellationToken);

    Assert.Null(_requiringTalent.RequiredTalentId);
  }

  [Fact(DisplayName = "It should set the required talent.")]
  public async Task It_should_set_the_required_talent()
  {
    Talent talent = new(_world.Id, tier: 1, new Name("Cuirassé"), _world.OwnerId);
    SetRequiredTalentCommand command = new(talent, _requiringTalent.EntityId);

    await _handler.Handle(command, _cancellationToken);

    Assert.Equal(_requiringTalent.Id, talent.RequiredTalentId);
  }

  [Fact(DisplayName = "It should throw InvalidRequiredTalentTierException when requiring a talent with a higher tier.")]
  public async Task It_should_throw_InvalidRequiredTalentTierException_when_requiring_a_talent_with_a_higher_tier()
  {
    Talent talent = new(_world.Id, tier: 1, new Name("Manœuvres de combat"), _world.OwnerId);
    _talentRepository.Setup(x => x.LoadAsync(talent.Id, _cancellationToken)).ReturnsAsync(talent);

    SetRequiredTalentCommand command = new(_requiringTalent, talent.EntityId);

    var exception = await Assert.ThrowsAsync<InvalidRequiredTalentTierException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(_requiringTalent.EntityId, exception.RequiringTalentId);
    Assert.Equal(talent.EntityId, exception.RequiredTalentId);
    Assert.Equal("RequiredTalentId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw TalentNotFoundException when the required talent could not be found.")]
  public async Task It_should_throw_TalentNotFoundException_when_the_required_talent_could_not_be_found()
  {
    Talent talent = new(_world.Id, tier: 1, new Name("Manœuvres de combat"), _world.OwnerId);
    SetRequiredTalentCommand command = new(talent, Guid.NewGuid());
    Assert.True(command.Id.HasValue);

    var exception = await Assert.ThrowsAsync<TalentNotFoundException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(command.Id, exception.Id);
    Assert.Equal("RequiredTalentId", exception.PropertyName);
  }
}
