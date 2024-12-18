﻿using Bogus;
using Logitar.Security.Cryptography;
using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain.Characters;

namespace SkillCraft.Application.Characters.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class ReplaceCharacterCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<ICharacterQuerier> _characterQuerier = new();
  private readonly Mock<ICharacterRepository> _characterRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();

  private readonly ReplaceCharacterCommandHandler _handler;

  private readonly WorldMock _world = new();

  public ReplaceCharacterCommandHandlerTests()
  {
    _handler = new(_characterQuerier.Object, _characterRepository.Object, _permissionService.Object, _sender.Object);
  }

  [Fact(DisplayName = "It should replace an existing character.")]
  public async Task It_should_replace_an_existing_character()
  {
    Character character = new CharacterBuilder(_world).Build();
    _characterRepository.Setup(x => x.LoadAsync(character.Id, _cancellationToken)).ReturnsAsync(character);

    ReplaceCharacterPayload payload = new(" Heracles Aetos ")
    {
      Player = $"  {_faker.Person.FullName}  ",
      Height = 1.8,
      Weight = 72.9,
      Age = 21,
      Experience = 100,
      Vitality = 50,
      Stamina = 45,
      BloodAlcoholContent = 2,
      Intoxication = 1
    };
    ReplaceCharacterCommand command = new(character.EntityId, payload, Version: null);
    command.Contextualize(_world);

    CharacterModel model = new();
    _characterQuerier.Setup(x => x.ReadAsync(character, _cancellationToken)).ReturnsAsync(model);

    CharacterModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    Assert.Equal(payload.Name.Trim(), character.Name.Value);
    Assert.Equal(payload.Player.Trim(), character.Player?.Value);
    Assert.Equal(payload.Height, character.Height);
    Assert.Equal(payload.Weight, character.Weight);
    Assert.Equal(payload.Age, character.Age);
    Assert.Equal(payload.Experience, character.Experience);
    Assert.Equal(payload.Vitality, character.Vitality);
    Assert.Equal(payload.Stamina, character.Stamina);
    Assert.Equal(payload.BloodAlcoholContent, character.BloodAlcoholContent);
    Assert.Equal(payload.Intoxication, character.Intoxication);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Character && y.Id == character.EntityId),
      _cancellationToken));

    _sender.Verify(x => x.Send(It.Is<SaveCharacterCommand>(y => y.Character.Equals(character)), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return null when the character could not be found.")]
  public async Task It_should_return_null_when_the_character_could_not_be_found()
  {
    ReplaceCharacterPayload payload = new("Heracles Aetos")
    {
      Height = 1.67,
      Weight = 64.1,
      Age = 20
    };
    ReplaceCharacterCommand command = new(Guid.Empty, payload, Version: null);
    command.Contextualize(_world);

    CharacterModel? character = await _handler.Handle(command, _cancellationToken);
    Assert.Null(character);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    ReplaceCharacterPayload payload = new()
    {
      Player = RandomStringGenerator.GetString(1000),
      Experience = -100,
      Vitality = -10,
      Stamina = -5,
      BloodAlcoholContent = -1,
      Intoxication = -2
    };
    ReplaceCharacterCommand command = new(Guid.Empty, payload, Version: null);
    command.Contextualize(_world);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(10, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Name");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Player");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "GreaterThanValidator" && e.PropertyName == "Height");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "GreaterThanValidator" && e.PropertyName == "Weight");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "GreaterThanValidator" && e.PropertyName == "Age");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "GreaterThanOrEqualValidator" && e.PropertyName == "Experience");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "GreaterThanOrEqualValidator" && e.PropertyName == "Vitality");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "GreaterThanOrEqualValidator" && e.PropertyName == "Stamina");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "GreaterThanOrEqualValidator" && e.PropertyName == "BloodAlcoholContent");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "GreaterThanOrEqualValidator" && e.PropertyName == "Intoxication");
  }

  [Fact(DisplayName = "It should update an existing character.")]
  public async Task It_should_update_an_existing_character()
  {
    Character character = new CharacterBuilder(_world).Build();
    PlayerName player = new(_faker.Person.FullName);
    character.Player = player;
    character.Update(_world.OwnerId);
    _characterRepository.Setup(x => x.LoadAsync(character.Id, _cancellationToken)).ReturnsAsync(character);

    Character reference = new CharacterBuilder(_world).WithId(character.Id).Build();
    _characterRepository.Setup(x => x.LoadAsync(reference.Id, reference.Version, _cancellationToken)).ReturnsAsync(reference);

    ReplaceCharacterPayload payload = new(" Heracles Aetos ")
    {
      Player = "    ",
      Height = 1.8,
      Weight = 72.9,
      Age = 21,
      Experience = 100,
      Vitality = 50,
      Stamina = 45,
      BloodAlcoholContent = 2,
      Intoxication = 1
    };
    ReplaceCharacterCommand command = new(character.EntityId, payload, reference.Version);
    command.Contextualize(_world);

    CharacterModel model = new();
    _characterQuerier.Setup(x => x.ReadAsync(character, _cancellationToken)).ReturnsAsync(model);

    CharacterModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    Assert.Equal(payload.Name.Trim(), character.Name.Value);
    Assert.Equal(player, character.Player);
    Assert.Equal(payload.Height, character.Height);
    Assert.Equal(payload.Weight, character.Weight);
    Assert.Equal(payload.Age, character.Age);
    Assert.Equal(payload.Experience, character.Experience);
    Assert.Equal(payload.Vitality, character.Vitality);
    Assert.Equal(payload.Stamina, character.Stamina);
    Assert.Equal(payload.BloodAlcoholContent, character.BloodAlcoholContent);
    Assert.Equal(payload.Intoxication, character.Intoxication);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Character && y.Id == character.EntityId),
      _cancellationToken));

    _sender.Verify(x => x.Send(It.Is<SaveCharacterCommand>(y => y.Character.Equals(character)), _cancellationToken), Times.Once);
  }
}
