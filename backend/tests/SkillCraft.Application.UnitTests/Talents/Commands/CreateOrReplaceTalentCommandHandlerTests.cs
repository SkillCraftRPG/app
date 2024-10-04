using FluentValidation;
using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Talents;
using SkillCraft.Domain;
using SkillCraft.Domain.Talents;

namespace SkillCraft.Application.Talents.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateOrReplaceTalentCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();
  private readonly Mock<ITalentQuerier> _talentQuerier = new();
  private readonly Mock<ITalentRepository> _talentRepository = new();

  private readonly CreateOrReplaceTalentCommandHandler _handler;

  private readonly WorldMock _world = new();
  private readonly Talent _melee;
  private readonly Talent _talent;
  private readonly TalentModel _model = new();

  public CreateOrReplaceTalentCommandHandlerTests()
  {
    _handler = new(_permissionService.Object, _sender.Object, _talentQuerier.Object, _talentRepository.Object);

    _melee = new(_world.Id, tier: 0, new Name("Mêlée"), _world.OwnerId);
    _talentRepository.Setup(x => x.LoadAsync(_melee.Id, _cancellationToken)).ReturnsAsync(_melee);

    _talent = new(_world.Id, tier: 0, new Name("formation-martiale"), _world.OwnerId);
    _talentRepository.Setup(x => x.LoadAsync(_talent.Id, _cancellationToken)).ReturnsAsync(_talent);

    _talentQuerier.Setup(x => x.ReadAsync(It.IsAny<Talent>(), _cancellationToken)).ReturnsAsync(_model);
  }

  [Theory(DisplayName = "It should create a new talent.")]
  [InlineData(null)]
  [InlineData("b7e39051-d07e-457e-a193-e4400b60540c")]
  public async Task It_should_create_a_new_talent(string? idValue)
  {
    CreateOrReplaceTalentPayload payload = new(" Cuirassé ")
    {
      Tier = 1,
      Description = "    ",
      AllowMultiplePurchases = true,
      RequiredTalentId = _talent.EntityId
    };

    bool parsed = Guid.TryParse(idValue, out Guid id);
    CreateOrReplaceTalentCommand command = new(parsed ? id : null, payload, Version: null);
    command.Contextualize(_world);

    CreateOrReplaceTalentResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_model, result.Talent);
    Assert.True(result.Created);

    _permissionService.Verify(x => x.EnsureCanCreateAsync(command, EntityType.Talent, _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(
      It.Is<SetRequiredTalentCommand>(y => y.Id == _talent.EntityId),
      _cancellationToken), Times.Once);
    _sender.Verify(x => x.Send(
      It.Is<SaveTalentCommand>(y => (!parsed || y.Talent.EntityId == id)
        && y.Talent.Name.Value == payload.Name.Trim()
        && y.Talent.Description == null
        && y.Talent.AllowMultiplePurchases == payload.AllowMultiplePurchases),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should replace an existing talent.")]
  public async Task It_should_replace_an_existing_talent()
  {
    CreateOrReplaceTalentPayload payload = new(" Formation martiale ")
    {
      Description = "  Forme le personnage au maniement des armes martiales. Il est également formé au port des armures moyennes et à l’utilisation des boucliers moyens. De plus, il n’est pas affligé de la pénalité à ses tests de _Furtivité_ et à sa régénération de Vitalité et d’Énergie lorsqu’il porte une armure légère ou moyenne.  ",
      AllowMultiplePurchases = true,
      RequiredTalentId = _melee.EntityId
    };

    CreateOrReplaceTalentCommand command = new(_talent.EntityId, payload, Version: null);
    command.Contextualize(_world);

    CreateOrReplaceTalentResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_model, result.Talent);
    Assert.False(result.Created);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Talent && y.Id == _talent.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(
      It.Is<SetRequiredTalentCommand>(y => y.Talent.Equals(_talent) && y.Id == _melee.EntityId),
      _cancellationToken), Times.Once);
    _sender.Verify(x => x.Send(
      It.Is<SaveTalentCommand>(y => y.Talent.Equals(_talent)
        && y.Talent.Name.Value == payload.Name.Trim()
        && y.Talent.Description != null && y.Talent.Description.Value == payload.Description.Trim()
        && y.Talent.AllowMultiplePurchases == payload.AllowMultiplePurchases),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return null when updating an talent that does not exist.")]
  public async Task It_should_return_null_when_updating_an_talent_that_does_not_exist()
  {
    CreateOrReplaceTalentCommand command = new(Guid.Empty, new CreateOrReplaceTalentPayload("Formation martiale"), Version: 0);
    command.Contextualize(_world);

    CreateOrReplaceTalentResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Null(result.Talent);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateOrReplaceTalentPayload payload = new()
    {
      Tier = -3
    };

    CreateOrReplaceTalentCommand command = new(Id: null, payload, Version: null);
    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Name");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "InclusiveBetweenValidator" && e.PropertyName == "Tier");
  }

  [Fact(DisplayName = "It should update an existing talent.")]
  public async Task It_should_update_an_existing_talent()
  {
    Talent reference = new(_talent.WorldId, _talent.Tier, _talent.Name, _world.OwnerId, _talent.EntityId)
    {
      Description = _talent.Description,
      AllowMultiplePurchases = _talent.AllowMultiplePurchases
    };
    reference.Update(_world.OwnerId);
    _talentRepository.Setup(x => x.LoadAsync(reference.Id, reference.Version, _cancellationToken)).ReturnsAsync(reference);

    Description description = new("Forme le personnage au maniement des armes martiales. Il est également formé au port des armures moyennes et à l’utilisation des boucliers moyens. De plus, il n’est pas affligé de la pénalité à ses tests de _Furtivité_ et à sa régénération de Vitalité et d’Énergie lorsqu’il porte une armure légère ou moyenne.");
    _talent.Description = description;
    _talent.Update(_world.OwnerId);

    CreateOrReplaceTalentPayload payload = new(" Formation martiale ")
    {
      Tier = 1,
      Description = "    ",
      AllowMultiplePurchases = true,
      RequiredTalentId = _melee.EntityId
    };

    CreateOrReplaceTalentCommand command = new(_talent.EntityId, payload, reference.Version);
    command.Contextualize(_world);

    CreateOrReplaceTalentResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_model, result.Talent);
    Assert.False(result.Created);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Talent && y.Id == _talent.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(
      It.Is<SetRequiredTalentCommand>(y => y.Talent.Equals(_talent) && y.Id == _melee.EntityId),
      _cancellationToken), Times.Once);
    _sender.Verify(x => x.Send(
      It.Is<SaveTalentCommand>(y => y.Talent.Equals(_talent)
        && y.Talent.Tier == _talent.Tier
        && y.Talent.Name.Value == payload.Name.Trim()
        && y.Talent.Description == description
        && y.Talent.AllowMultiplePurchases == payload.AllowMultiplePurchases),
      _cancellationToken), Times.Once);
  }
}
