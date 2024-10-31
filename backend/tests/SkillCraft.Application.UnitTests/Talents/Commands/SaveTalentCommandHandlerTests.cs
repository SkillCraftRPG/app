using Moq;
using SkillCraft.Application.Storages;
using SkillCraft.Contracts;
using SkillCraft.Domain;
using SkillCraft.Domain.Talents;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Talents.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SaveTalentCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IStorageService> _storageService = new();
  private readonly Mock<ITalentQuerier> _talentQuerier = new();
  private readonly Mock<ITalentRepository> _talentRepository = new();

  private readonly SaveTalentCommandHandler _handler;

  public SaveTalentCommandHandlerTests()
  {
    _handler = new(_storageService.Object, _talentQuerier.Object, _talentRepository.Object);
  }

  [Fact(DisplayName = "It should save the talent.")]
  public async Task It_should_save_the_talent()
  {
    Talent talent = new(WorldId.NewId(), tier: 1, new Name("Ambidextre"), UserId.NewId());
    SaveTalentCommand command = new(talent);

    await _handler.Handle(command, _cancellationToken);

    _talentRepository.Verify(x => x.SaveAsync(talent, _cancellationToken), Times.Once);

    EntityMetadata entity = talent.GetMetadata();
    _storageService.Verify(x => x.EnsureAvailableAsync(entity, _cancellationToken), Times.Once);
    _storageService.Verify(x => x.UpdateAsync(entity, _cancellationToken), Times.Once);

    _talentQuerier.Verify(x => x.FindIdAsync(It.IsAny<WorldId>(), It.IsAny<Skill>(), It.IsAny<CancellationToken>()), Times.Never);
  }

  [Fact(DisplayName = "It should throw TalentSkillAlreadyExistingException when the skill is already associated to another talent.")]
  public async Task It_should_throw_TalentSkillAlreadyExistingException_when_the_skill_is_already_associated_to_another_talent()
  {
    UserId userId = UserId.NewId();
    Talent talent = new(WorldId.NewId(), tier: 0, new Name("Mêlée"), userId)
    {
      Skill = Skill.Melee
    };
    talent.Update(userId);
    SaveTalentCommand command = new(talent);

    TalentId conflictId = new(WorldId.NewId());
    _talentQuerier.Setup(x => x.FindIdAsync(talent.WorldId, talent.Skill.Value, _cancellationToken)).ReturnsAsync(conflictId);

    var exception = await Assert.ThrowsAsync<TalentSkillAlreadyExistingException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(talent.WorldId.ToGuid(), exception.WorldId);
    Assert.Equal([talent.EntityId, conflictId.EntityId], exception.ConflictingIds);
    Assert.Equal(talent.Skill, exception.Skill);
    Assert.Equal("Name", exception.PropertyName);
  }
}
