using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain.Characters;

namespace SkillCraft.Application.Characters.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class IncreaseCharacterSkillRankCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICharacterQuerier> _characterQuerier = new();
  private readonly Mock<ICharacterRepository> _characterRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();

  private readonly IncreaseCharacterSkillRankCommandHandler _handler;

  private readonly WorldMock _world = new();

  public IncreaseCharacterSkillRankCommandHandlerTests()
  {
    _handler = new(_characterQuerier.Object, _characterRepository.Object, _permissionService.Object, _sender.Object);
  }

  [Fact(DisplayName = "It should increase the rank of the character skill.")]
  public async Task It_should_increase_the_rank_of_the_character_skill()
  {
    Character character = new CharacterBuilder(_world).Build();
    _characterRepository.Setup(x => x.LoadAsync(character.Id, _cancellationToken)).ReturnsAsync(character);

    CharacterModel model = new();
    _characterQuerier.Setup(x => x.ReadAsync(character, _cancellationToken)).ReturnsAsync(model);

    IncreaseCharacterSkillRankCommand command = new(character.EntityId, Skill.Resistance);
    command.Contextualize(_world);

    CharacterModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    Assert.Equal(1, character.SkillRanks[Skill.Resistance]);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Character && y.Id == character.EntityId),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveCharacterCommand>(y => y.Character.Equals(character)), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return null when the character could not be found.")]
  public async Task It_should_return_null_when_the_character_could_not_be_found()
  {
    IncreaseCharacterSkillRankCommand command = new(Guid.NewGuid(), Skill.Athletics);
    command.Contextualize(_world);

    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should return null when the skill is not defined.")]
  public async Task It_should_return_null_when_the_skill_is_not_defined()
  {
    IncreaseCharacterSkillRankCommand command = new(Guid.Empty, (Skill)(-1));
    command.Contextualize(_world);

    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }
}
