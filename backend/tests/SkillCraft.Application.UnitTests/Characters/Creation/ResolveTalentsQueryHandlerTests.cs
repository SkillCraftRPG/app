using Moq;
using SkillCraft.Application.Castes;
using SkillCraft.Application.Characters.Commands;
using SkillCraft.Application.Educations;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Talents;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain;
using SkillCraft.Domain.Castes;
using SkillCraft.Domain.Educations;
using SkillCraft.Domain.Talents;

namespace SkillCraft.Application.Characters.Creation;

[Trait(Traits.Category, Categories.Unit)]
public class ResolveTalentsQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ITalentRepository> _talentRepository = new();

  private readonly ResolveTalentsQueryHandler _handler;

  private readonly WorldMock _world = new();
  private readonly Caste _amuseur;
  private readonly Caste _artisan;
  private readonly Caste _exile;
  private readonly Education _apprentiDUnMaitre;
  private readonly Education _classique;
  private readonly Education _dansLaRue;
  private readonly Education _rebelle;
  private readonly Talent _artisanat;
  private readonly Talent _connaissance;
  private readonly Talent _formationMartiale;
  private readonly Talent _melee;
  private readonly Talent _resistance;

  private readonly CreateCharacterCommand _activity = new(new CreateCharacterPayload());

  public ResolveTalentsQueryHandlerTests()
  {
    _handler = new(_permissionService.Object, _talentRepository.Object);

    _amuseur = new(_world.Id, new Name("Amuseur"), _world.OwnerId);
    _artisan = new(_world.Id, new Name("Artisan"), _world.OwnerId)
    {
      Skill = Skill.Craft
    };
    _artisan.Update(_world.OwnerId);
    _exile = new(_world.Id, new Name("Exilé"), _world.OwnerId)
    {
      Skill = Skill.Survival
    };
    _exile.Update(_world.OwnerId);

    _apprentiDUnMaitre = new(_world.Id, new Name("Apprenti d’un maître"), _world.OwnerId)
    {
      Skill = _artisan.Skill
    };
    _apprentiDUnMaitre.Update(_world.OwnerId);
    _classique = new(_world.Id, new Name("Classique"), _world.OwnerId)
    {
      Skill = Skill.Knowledge
    };
    _classique.Update(_world.OwnerId);
    _dansLaRue = new(_world.Id, new Name("Dans la rue"), _world.OwnerId);
    _rebelle = new(_world.Id, new Name("Rebelle"), _world.OwnerId)
    {
      Skill = Skill.Deception
    };
    _rebelle.Update(_world.OwnerId);

    _artisanat = new(_world.Id, tier: 0, new Name("Artisanat"), _world.OwnerId);
    _connaissance = new(_world.Id, tier: 0, new Name("Connaissance"), _world.OwnerId);
    _melee = new(_world.Id, tier: 0, new Name("Mêlée"), _world.OwnerId);
    _resistance = new(_world.Id, tier: 0, new Name("Résistance"), _world.OwnerId);

    _formationMartiale = new(_world.Id, tier: 0, new Name("Formation martiale"), _world.OwnerId);
    _formationMartiale.SetRequiredTalent(_melee);
    _formationMartiale.Update(_world.OwnerId);

    _talentRepository.Setup(x => x.LoadAsync(_world.Id, Skill.Craft, _cancellationToken)).ReturnsAsync(_artisanat);
    _talentRepository.Setup(x => x.LoadAsync(_world.Id, Skill.Knowledge, _cancellationToken)).ReturnsAsync(_connaissance);

    _activity.Contextualize(_world);
  }

  [Fact(DisplayName = "It should return the found talents.")]
  public async Task It_should_return_the_found_talents()
  {
    _talentRepository.Setup(x => x.LoadAsync(
      It.Is<IEnumerable<TalentId>>(x => x.All(y => y.WorldId == _world.Id) && x.Count() == 2
        && x.Any(y => y.EntityId == _melee.EntityId) && x.Any(y => y.EntityId == _resistance.EntityId)),
      _cancellationToken)).ReturnsAsync([_melee, _resistance]);

    ResolveTalentsQuery query = new(_activity, _artisan, _classique, Ids: [_melee.EntityId, _resistance.EntityId]);

    IReadOnlyCollection<Talent> talents = await _handler.Handle(query, _cancellationToken);
    Assert.Equal(4, talents.Count);
    Assert.Contains(_artisanat, talents);
    Assert.Contains(_connaissance, talents);
    Assert.Contains(_melee, talents);
    Assert.Contains(_resistance, talents);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(_activity, EntityType.Talent, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw CasteHasNoSkillTalentException when the caste has no associated skill talent.")]
  public async Task It_should_throw_CasteHasNoSkillTalentException_when_the_caste_has_no_associated_skill_talent()
  {
    ResolveTalentsQuery query = new(_activity, _exile, _dansLaRue, Ids: []);

    var exception = await Assert.ThrowsAsync<CasteHasNoSkillTalentException>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(_exile.EntityId, exception.CasteId);
    Assert.NotNull(exception.Skill);
    Assert.Equal(_exile.Skill, exception.Skill);
    Assert.Equal("CasteId", exception.PropertyName);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(_activity, EntityType.Talent, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw CasteHasNoSkillTalentException when the caste has no skill.")]
  public async Task It_should_throw_CasteHasNoSkillTalentException_when_the_caste_has_no_skill()
  {
    ResolveTalentsQuery query = new(_activity, _amuseur, _dansLaRue, Ids: []);

    var exception = await Assert.ThrowsAsync<CasteHasNoSkillTalentException>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(_amuseur.EntityId, exception.CasteId);
    Assert.Null(exception.Skill);
    Assert.Equal("CasteId", exception.PropertyName);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(_activity, EntityType.Talent, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw EducationHasNoSkillTalentException when the education has no associated skill talent.")]
  public async Task It_should_throw_EducationHasNoSkillTalentException_when_the_education_has_no_associated_skill_talent()
  {
    ResolveTalentsQuery query = new(_activity, _artisan, _rebelle, Ids: []);

    var exception = await Assert.ThrowsAsync<EducationHasNoSkillTalentException>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(_rebelle.EntityId, exception.EducationId);
    Assert.NotNull(exception.Skill);
    Assert.Equal(_rebelle.Skill, exception.Skill);
    Assert.Equal("EducationId", exception.PropertyName);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(_activity, EntityType.Talent, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw EducationHasNoSkillTalentException when the education has no skill.")]
  public async Task It_should_throw_EducationHasNoSkillTalentException_when_the_education_has_no_skill()
  {
    ResolveTalentsQuery query = new(_activity, _artisan, _dansLaRue, Ids: []);

    var exception = await Assert.ThrowsAsync<EducationHasNoSkillTalentException>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(_dansLaRue.EntityId, exception.EducationId);
    Assert.Null(exception.Skill);
    Assert.Equal("EducationId", exception.PropertyName);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(_activity, EntityType.Talent, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw InvalidCasteEducationSelectionException when the caste and the education are associated to the same skill.")]
  public async Task It_should_throw_InvalidCasteEducationSelectionException_when_the_caste_and_the_education_are_associated_to_the_same_skill()
  {
    ResolveTalentsQuery query = new(_activity, _artisan, _apprentiDUnMaitre, Ids: []);

    var exception = await Assert.ThrowsAsync<InvalidCasteEducationSelectionException>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(_artisan.EntityId, exception.CasteId);
    Assert.Equal(_apprentiDUnMaitre.EntityId, exception.EducationId);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(_activity, EntityType.Talent, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw InvalidSkillTalentSelectionException when some talents have no skill or are associated to the caste or the education.")]
  public async Task It_should_throw_InvalidSkillTalentSelectionException_when_some_talents_have_no_skill_or_are_associated_to_the_caste_or_the_education()
  {
    _talentRepository.Setup(x => x.LoadAsync(It.IsAny<IEnumerable<TalentId>>(), _cancellationToken))
      .ReturnsAsync([_artisanat, _connaissance, _melee, _formationMartiale]);

    ResolveTalentsQuery query = new(_activity, _artisan, _classique, Ids: [_artisanat.EntityId, _connaissance.EntityId, _melee.EntityId, _formationMartiale.EntityId]);

    var exception = await Assert.ThrowsAsync<InvalidSkillTalentSelectionException>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(_artisan.EntityId, exception.CasteId);
    Assert.Equal(_classique.EntityId, exception.EducationId);
    Assert.Equal([_artisanat.EntityId, _connaissance.EntityId, _formationMartiale.EntityId], exception.TalentIds);
    Assert.Equal("TalentIds", exception.PropertyName);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(_activity, EntityType.Talent, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw TalentsNotFoundException when some talents could not be found.")]
  public async Task It_should_throw_TalentsNotFoundException_when_some_talents_could_not_be_found()
  {
    _talentRepository.Setup(x => x.LoadAsync(It.IsAny<IEnumerable<TalentId>>(), _cancellationToken)).ReturnsAsync([_melee]);

    ResolveTalentsQuery query = new(_activity, _artisan, _classique, Ids: [_melee.EntityId, Guid.Empty, Guid.NewGuid()]);

    var exception = await Assert.ThrowsAsync<TalentsNotFoundException>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(query.Ids.Skip(1), exception.TalentIds);
    Assert.Equal("TalentIds", exception.PropertyName);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(_activity, EntityType.Talent, _cancellationToken), Times.Once);
  }
}
