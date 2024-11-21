using FluentValidation;
using Logitar.Security.Cryptography;
using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Talents;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain;
using SkillCraft.Domain.Characters;
using SkillCraft.Domain.Talents;

namespace SkillCraft.Application.Characters.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SetCharacterTalentCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICharacterQuerier> _characterQuerier = new();
  private readonly Mock<ICharacterRepository> _characterRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();
  private readonly Mock<ITalentRepository> _talentRepository = new();

  private readonly SetCharacterTalentCommandHandler _handler;

  private readonly WorldMock _world = new();
  private readonly Talent _occultisme;

  public SetCharacterTalentCommandHandlerTests()
  {
    _handler = new(_characterQuerier.Object, _characterRepository.Object, _permissionService.Object, _sender.Object, _talentRepository.Object);

    _occultisme = new Talent(_world.Id, tier: 0, new Name("Occultisme"), _world.OwnerId)
    {
      Skill = Skill.Occultism
    };
    _occultisme.Update(_world.OwnerId);
    _talentRepository.Setup(x => x.LoadAsync(_occultisme.Id, _cancellationToken)).ReturnsAsync(_occultisme);
  }

  [Theory(DisplayName = "It should add a new character talent.")]
  [InlineData(null)]
  [InlineData("370f889f-1a2a-48e9-9f02-25f151ae7877")]
  public async Task It_should_add_a_new_character_talent(string? relationIdValue)
  {
    Guid? relationId = relationIdValue == null ? null : Guid.Parse(relationIdValue);

    Character character = new CharacterBuilder(_world).Build();
    _characterRepository.Setup(x => x.LoadAsync(character.Id, _cancellationToken)).ReturnsAsync(character);

    CharacterTalentPayload payload = new()
    {
      TalentId = _occultisme.EntityId,
      Cost = 1,
      Precision = " Esprit ",
      Notes = "  Discounted by Aspect: Tenace  "
    };
    SetCharacterTalentCommand command = new(character.EntityId, relationId, payload);
    command.Contextualize(_world);

    CharacterModel model = new();
    _characterQuerier.Setup(x => x.ReadAsync(character, _cancellationToken)).ReturnsAsync(model);

    CharacterModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    Assert.Contains(character.Talents, t => (relationId.HasValue ? t.Key == relationId.Value : t.Key != Guid.Empty) && t.Value.Id == _occultisme.Id
      && t.Value.Cost == payload.Cost && t.Value.Precision?.Value == payload.Precision.Trim() && t.Value.Notes?.Value == payload.Notes.Trim());

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(command, character.GetMetadata(), _cancellationToken), Times.Once);
    _permissionService.Verify(x => x.EnsureCanPreviewAsync(command, EntityType.Talent, _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveCharacterCommand>(y => y.Character.Equals(character)), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return null when the character could not be found.")]
  public async Task It_should_return_null_when_the_character_could_not_be_found()
  {
    CharacterTalentPayload payload = new()
    {
      TalentId = Guid.NewGuid()
    };
    SetCharacterTalentCommand command = new(Guid.Empty, RelationId: null, payload);
    command.Contextualize(_world);

    CharacterModel? character = await _handler.Handle(command, _cancellationToken);
    Assert.Null(character);
  }

  [Fact(DisplayName = "It should set an existing character talent.")]
  public async Task It_should_set_an_existing_character_talent()
  {
    Guid relationId = Guid.NewGuid();

    Character character = new CharacterBuilder(_world).Build();
    character.SetTalent(relationId, _occultisme, _world.OwnerId);
    _characterRepository.Setup(x => x.LoadAsync(character.Id, _cancellationToken)).ReturnsAsync(character);

    CharacterTalentPayload payload = new()
    {
      TalentId = Guid.NewGuid(),
      Cost = 1,
      Precision = " Esprit ",
      Notes = "  Discounted by Aspect: Tenace  "
    };
    SetCharacterTalentCommand command = new(character.EntityId, relationId, payload);
    command.Contextualize(_world);

    CharacterModel model = new();
    _characterQuerier.Setup(x => x.ReadAsync(character, _cancellationToken)).ReturnsAsync(model);

    CharacterModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    Assert.Contains(character.Talents, t => t.Key == relationId && t.Value.Id == _occultisme.Id && t.Value.Cost == payload.Cost
      && t.Value.Precision?.Value == payload.Precision.Trim() && t.Value.Notes?.Value == payload.Notes.Trim());

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(command, character.GetMetadata(), _cancellationToken), Times.Once);
    _permissionService.Verify(x => x.EnsureCanPreviewAsync(command, EntityType.Talent, _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveCharacterCommand>(y => y.Character.Equals(character)), _cancellationToken), Times.Once);
  }

  [Theory(DisplayName = "It should throw TalentNotFoundException when the talent could not be found.")]
  [InlineData(null)]
  [InlineData("e044a30b-f7e5-45ca-be2e-669699fa1556")]
  public async Task It_should_throw_TalentNotFoundException_when_the_talent_could_not_be_found(string? relationIdValue)
  {
    Guid? relationId = relationIdValue == null ? null : Guid.Parse(relationIdValue);

    Character character = new CharacterBuilder(_world).Build();
    _characterRepository.Setup(x => x.LoadAsync(character.Id, _cancellationToken)).ReturnsAsync(character);

    CharacterTalentPayload payload = new()
    {
      TalentId = Guid.NewGuid()
    };
    SetCharacterTalentCommand command = new(character.EntityId, relationId, payload);
    command.Contextualize(_world);

    var exception = await Assert.ThrowsAsync<TalentNotFoundException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(_world.EntityId, exception.WorldId);
    Assert.Equal(payload.TalentId, exception.TalentId);
    Assert.Equal("TalentId", exception.PropertyName);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(command, character.GetMetadata(), _cancellationToken), Times.Once);
    _permissionService.Verify(x => x.EnsureCanPreviewAsync(command, EntityType.Talent, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CharacterTalentPayload payload = new()
    {
      Cost = -1,
      Precision = RandomStringGenerator.GetString(1000)
    };
    SetCharacterTalentCommand command = new(Guid.Empty, RelationId: null, payload);
    command.Contextualize(_world);

    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    Assert.Equal(3, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "TalentId");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "InclusiveBetweenValidator" && e.PropertyName == "Cost");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Precision");
  }
}
