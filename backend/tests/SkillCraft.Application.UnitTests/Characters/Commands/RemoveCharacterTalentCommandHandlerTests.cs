using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain;
using SkillCraft.Domain.Characters;
using SkillCraft.Domain.Talents;

namespace SkillCraft.Application.Characters.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class RemoveCharacterTalentCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICharacterQuerier> _characterQuerier = new();
  private readonly Mock<ICharacterRepository> _characterRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();

  private readonly RemoveCharacterTalentCommandHandler _handler;

  private readonly WorldMock _world = new();

  public RemoveCharacterTalentCommandHandlerTests()
  {
    _handler = new(_characterQuerier.Object, _characterRepository.Object, _permissionService.Object, _sender.Object);
  }

  [Fact(DisplayName = "It should remove a character talent.")]
  public async Task It_should_remove_a_character_talent()
  {
    Talent talent = new(_world.Id, tier: 0, new Name("Mêlée"), _world.OwnerId)
    {
      Skill = Skill.Melee
    };
    talent.Update(_world.OwnerId);

    Character character = new CharacterBuilder(_world).Build();
    character.AddTalent(talent, _world.OwnerId);
    _characterRepository.Setup(x => x.LoadAsync(character.Id, _cancellationToken)).ReturnsAsync(character);

    Guid relationId = Assert.Single(character.Talents).Key;
    RemoveCharacterTalentCommand command = new(character.EntityId, relationId);
    command.Contextualize(_world);

    CharacterModel model = new();
    _characterQuerier.Setup(x => x.ReadAsync(character, _cancellationToken)).ReturnsAsync(model);

    CharacterModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    Assert.Empty(character.Talents);
    Assert.Contains(character.Changes, change => change is Character.TalentRemovedEvent e && e.RelationId == relationId);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(command, character.GetMetadata(), _cancellationToken));

    _sender.Verify(x => x.Send(It.Is<SaveCharacterCommand>(y => y.Character.Equals(character)), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return null when the character could not be found.")]
  public async Task It_should_return_null_when_the_character_could_not_be_found()
  {
    RemoveCharacterTalentCommand command = new(Guid.Empty, Guid.Empty);
    command.Contextualize(_world);

    CharacterModel? character = await _handler.Handle(command, _cancellationToken);
    Assert.Null(character);
  }
}
