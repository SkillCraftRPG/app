using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Application.Characters.Commands;
using SkillCraft.Application.Characters.Queries;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Domain;
using SkillCraft.Domain.Aspects;
using SkillCraft.Domain.Castes;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Educations;
using SkillCraft.Domain.Items;
using SkillCraft.Domain.Items.Properties;
using SkillCraft.Domain.Languages;
using SkillCraft.Domain.Lineages;
using SkillCraft.Domain.Personalities;
using SkillCraft.Domain.Talents;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Application.Characters;

[Trait(Traits.Category, Categories.Integration)]
public class CharacterTests : IntegrationTests
{
  private readonly IAspectRepository _aspectRepository;
  private readonly ICasteRepository _casteRepository;
  private readonly ICustomizationRepository _customizationRepository;
  private readonly IEducationRepository _educationRepository;
  private readonly IItemRepository _itemRepository;
  private readonly ILanguageRepository _languageRepository;
  private readonly ILineageRepository _lineageRepository;
  private readonly IPersonalityRepository _personalityRepository;
  private readonly ITalentRepository _talentRepository;

  private readonly Language _cassite;
  private readonly Language _orrinique;

  private readonly Lineage _humain;
  private readonly Lineage _orrin;

  private readonly Customization _chaotique;
  private readonly Customization _durACuire;
  private readonly Customization _feroce;
  private readonly Personality _courrouce;

  private readonly Aspect _farouche;
  private readonly Aspect _gymnaste;

  private readonly Caste _milicien;
  private readonly Education _champsDeBataille;

  private readonly Talent _acrobaties;
  private readonly Talent _athletisme;
  private readonly Talent _melee;
  private readonly Talent _resistance;

  private readonly Item _denier;

  public CharacterTests() : base()
  {
    _aspectRepository = ServiceProvider.GetRequiredService<IAspectRepository>();
    _casteRepository = ServiceProvider.GetRequiredService<ICasteRepository>();
    _customizationRepository = ServiceProvider.GetRequiredService<ICustomizationRepository>();
    _educationRepository = ServiceProvider.GetRequiredService<IEducationRepository>();
    _itemRepository = ServiceProvider.GetRequiredService<IItemRepository>();
    _languageRepository = ServiceProvider.GetRequiredService<ILanguageRepository>();
    _lineageRepository = ServiceProvider.GetRequiredService<ILineageRepository>();
    _personalityRepository = ServiceProvider.GetRequiredService<IPersonalityRepository>();
    _talentRepository = ServiceProvider.GetRequiredService<ITalentRepository>();

    _cassite = new Language(World.Id, new Name("Cassite"), UserId);
    _orrinique = new Language(World.Id, new Name("Orrinique"), UserId);

    _humain = new Lineage(World.Id, parent: null, new Name("Humain"), UserId)
    {
      Attributes = new AttributeBonuses(agility: 0, coordination: 0, intellect: 0, presence: 0, sensitivity: 0, spirit: 0, vigor: 0, extra: 2),
      Languages = new Domain.Lineages.Languages(languages: [], extra: 1, text: null),
      Speeds = new Speeds(walk: 6, climb: 0, swim: 0, fly: 0, hover: 0, burrow: 0)
    };
    _humain.AddFeature(new Feature(new Name("Apprentissage accéléré"), new Description("Le personnage débute avec 4 points d’Apprentissage supplémentaires et acquiert 1 point d’Apprentissage supplémentaire à chaque fois qu’il atteint un niveau pair (2, 4, 6, 8, 10, etc.).")));
    _humain.AddFeature(new Feature(new Name("Versatilité"), new Description("Le personnage acquiert gratuitement un talent associé à une compétence. Ces talents portent le même nom qu’une compétence.")));
    _humain.Update(UserId);
    _orrin = new Lineage(World.Id, _humain, new Name("Orrin"), UserId)
    {
      Languages = new Domain.Lineages.Languages(languages: [_orrinique], extra: 0, text: null)
    };
    _orrin.Update(UserId);

    _chaotique = new Customization(World.Id, CustomizationType.Disability, new Name("Chaotique"), UserId)
    {
      Description = new Description("Affligé d’un grand besoin de liberté, le personnage a tendance à désobéir lorsqu’il le peut et à contredire les lois. Ses tests de _Discipline_ sont effectués avec désavantage.")
    };
    _chaotique.Update(UserId);
    _durACuire = new Customization(World.Id, CustomizationType.Gift, new Name("Dur à cuire"), UserId)
    {
      Description = new Description("Lorsque le personnage reçoit des points de dégâts létaux, il retire à ceux-ci un nombre de points égal à son modificateur de Vigueur (minimum 1).")
    };
    _durACuire.Update(UserId);
    _feroce = new Customization(World.Id, CustomizationType.Gift, new Name("Féroce"), UserId)
    {
      Description = new Description("Une fois par round, le personnage peut rouler deux fois les dés de dégâts d’une attaque. Il peut ensuite utiliser le résultat de son choix.")
    };
    _feroce.Update(UserId);
    _courrouce = new Personality(World.Id, new Name("Courroucé"), UserId)
    {
      Description = new Description("Les émotions du personnage sont vives et ses mouvements sont brusques."),
      Attribute = Attribute.Agility
    };
    _courrouce.SetGift(_feroce);
    _courrouce.Update(UserId);

    _farouche = new Aspect(World.Id, new Name("Farouche"), UserId)
    {
      Attributes = new AttributeSelection(Attribute.Agility, Attribute.Sensitivity, Attribute.Spirit, Attribute.Vigor),
      Skills = new Skills(Skill.Melee, Skill.Survival)
    };
    _farouche.Update(UserId);
    _gymnaste = new Aspect(World.Id, new Name("Gymnaste"), UserId)
    {
      Attributes = new AttributeSelection(Attribute.Agility, Attribute.Vigor, Attribute.Coordination, Attribute.Sensitivity),
      Skills = new Skills(Skill.Acrobatics, Skill.Athletics)
    };
    _gymnaste.Update(UserId);

    _milicien = new Caste(World.Id, new Name("Milicien"), UserId)
    {
      Description = new Description("Le milicien est responsable de protéger les citoyens sur les terres de son seigneur. Il possède les droits mais également la responsabilité de faire appliquer les lois. Il doit également parfois jouer le rôle d’éclaireur et de soldat au sein de l’armée de sa patrie."),
      Skill = Skill.Melee,
      WealthRoll = new Roll("7d6")
    };
    _milicien.Update(UserId);
    _champsDeBataille = new Education(World.Id, new Name("Champs de bataille"), UserId)
    {
      Description = new Description("Le personnage est un enfant de la guerre, un fugitif d’un conflit ayant affecté les civils. Son enfance est maigre de moments heureux et aisés, ce a fait de lui une personne endurcie dotée d’une personnalité pragmatique."),
      Skill = Skill.Resistance,
      WealthMultiplier = 4.0
    };
    _champsDeBataille.Update(UserId);

    _acrobaties = new Talent(World.Id, tier: 0, new Name("Acrobaties"), UserId)
    {
      Skill = Skill.Acrobatics
    };
    _acrobaties.Update(UserId);
    _athletisme = new Talent(World.Id, tier: 0, new Name("Athlétisme"), UserId)
    {
      Skill = Skill.Athletics
    };
    _athletisme.Update(UserId);
    _melee = new Talent(World.Id, tier: 0, new Name("Mêlée"), UserId)
    {
      Skill = Skill.Melee
    };
    _melee.Update(UserId);
    _resistance = new Talent(World.Id, tier: 0, new Name("Résistance"), UserId)
    {
      Skill = Skill.Resistance
    };
    _resistance.Update(UserId);

    _denier = new Item(World.Id, new Name("Denier"), new MoneyProperties(), UserId)
    {
      Value = 1.0,
      Weight = 0.005
    };
    _denier.Update(UserId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _languageRepository.SaveAsync([_cassite, _orrinique]);
    await _lineageRepository.SaveAsync([_humain, _orrin]);
    await _customizationRepository.SaveAsync([_chaotique, _durACuire, _feroce]);
    await _personalityRepository.SaveAsync(_courrouce);
    await _aspectRepository.SaveAsync([_farouche, _gymnaste]);
    await _casteRepository.SaveAsync(_milicien);
    await _educationRepository.SaveAsync(_champsDeBataille);
    await _talentRepository.SaveAsync([_acrobaties, _athletisme, _melee, _resistance]);
    await _itemRepository.SaveAsync(_denier);
  }

  [Fact(DisplayName = "It should create a new character, then return the character found by ID.")]
  public async Task It_should_create_a_new_character_then_return_the_character_found_by_Id()
  {
    CreateCharacterPayload payload = new(" Heracles Aetos ")
    {
      Player = Faker.Person.FullName,
      LineageId = _orrin.EntityId,
      Height = 1.84,
      Weight = 84.6,
      Age = 30,
      LanguageIds = [_cassite.EntityId],
      PersonalityId = _courrouce.EntityId,
      CustomizationIds = [_durACuire.EntityId, _chaotique.EntityId],
      AspectIds = [_farouche.EntityId, _gymnaste.EntityId],
      Attributes = new BaseAttributesPayload
      {
        Agility = 9,
        Coordination = 9,
        Intellect = 6,
        Presence = 10,
        Sensitivity = 7,
        Spirit = 6,
        Vigor = 10,
        Best = Attribute.Agility,
        Worst = Attribute.Sensitivity,
        Optional = [Attribute.Coordination, Attribute.Vigor],
        Extra = [Attribute.Agility, Attribute.Vigor]
      },
      CasteId = _milicien.EntityId,
      EducationId = _champsDeBataille.EntityId,
      TalentIds = [_acrobaties.EntityId, _athletisme.EntityId],
      StartingWealth = new StartingWealthPayload
      {
        ItemId = _denier.EntityId,
        Quantity = 100
      }
    };
    CreateCharacterCommand command = new(payload);

    CharacterModel character = await Pipeline.ExecuteAsync(command);
    Assert.NotNull(character);

    Assert.NotEqual(default, character.Id);
    Assert.Equal(7, character.Version);
    Assert.Equal(DateTime.UtcNow, character.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.True(character.CreatedOn < character.UpdatedOn);
    Assert.Equal(Actor, character.CreatedBy);
    Assert.Equal(character.CreatedBy, character.UpdatedBy);

    Assert.Equal(World.Id.ToGuid(), character.World.Id);
    Assert.Equal(payload.Name.Trim(), character.Name);
    Assert.Equal(payload.Player, character.PlayerName);

    Assert.Equal(_orrin.EntityId, character.Lineage.Id);
    Assert.Equal(payload.Height, character.Height);
    Assert.Equal(payload.Weight, character.Weight);
    Assert.Equal(payload.Age, character.Age);

    CharacterLanguageModel language = Assert.Single(character.Languages);
    Assert.Equal(_cassite.EntityId, language.Language.Id);
    Assert.Equal("Lineage Extra Language", language.Notes);

    Assert.Equal(_courrouce.EntityId, character.Personality.Id);
    Assert.Equal(2, character.Customizations.Count);
    Assert.Contains(character.Customizations, c => c.Id == _chaotique.EntityId);
    Assert.Contains(character.Customizations, c => c.Id == _durACuire.EntityId);

    Assert.Equal(2, character.Aspects.Count);
    Assert.Contains(character.Aspects, a => a.Id == _farouche.EntityId);
    Assert.Contains(character.Aspects, a => a.Id == _gymnaste.EntityId);

    Assert.Equal(payload.Attributes.Agility, character.BaseAttributes.Agility);
    Assert.Equal(payload.Attributes.Coordination, character.BaseAttributes.Coordination);
    Assert.Equal(payload.Attributes.Intellect, character.BaseAttributes.Intellect);
    Assert.Equal(payload.Attributes.Presence, character.BaseAttributes.Presence);
    Assert.Equal(payload.Attributes.Sensitivity, character.BaseAttributes.Sensitivity);
    Assert.Equal(payload.Attributes.Spirit, character.BaseAttributes.Spirit);
    Assert.Equal(payload.Attributes.Vigor, character.BaseAttributes.Vigor);
    Assert.Equal(payload.Attributes.Best, character.BaseAttributes.Best);
    Assert.Equal(payload.Attributes.Worst, character.BaseAttributes.Worst);
    Assert.Equal([Attribute.Agility, Attribute.Vigor], character.BaseAttributes.Mandatory.OrderBy(x => x.ToString()));
    Assert.Equal(payload.Attributes.Optional, character.BaseAttributes.Optional);
    Assert.Equal(payload.Attributes.Extra, character.BaseAttributes.Extra);

    Assert.Equal(_milicien.EntityId, character.Caste.Id);
    Assert.Equal(_champsDeBataille.EntityId, character.Education.Id);

    Assert.Equal(4, character.Talents.Count);
    Assert.Contains(character.Talents, t => t.Id != default && t.Talent.Id == _acrobaties.EntityId
      && t.Cost == 1 && t.Precision == null && t.Notes == "Discounted by Aspect: Gymnaste");
    Assert.Contains(character.Talents, t => t.Id != default && t.Talent.Id == _athletisme.EntityId
      && t.Cost == 1 && t.Precision == null && t.Notes == "Discounted by Aspect: Gymnaste");
    Assert.Contains(character.Talents, t => t.Id != default && t.Talent.Id == _melee.EntityId
      && t.Cost == 1 && t.Precision == null && t.Notes == "Caste: Milicien; Discounted by Aspect: Farouche");
    Assert.Contains(character.Talents, t => t.Id != default && t.Talent.Id == _resistance.EntityId
      && t.Cost == 2 && t.Precision == null && t.Notes == "Education: Champs de bataille");

    InventoryModel item = Assert.Single(character.Inventory);
    Assert.NotEqual(default, item.Id);
    Assert.Equal(_denier.EntityId, item.Item.Id);
    Assert.Null(item.ContainingItemId);
    Assert.Equal(payload.StartingWealth.Quantity, item.Quantity);
    Assert.Null(item.IsAttuned);
    Assert.False(item.IsEquipped);
    Assert.True(item.IsIdentified);
    Assert.Null(item.IsProficient);
    Assert.Null(item.Skill);
    Assert.Null(item.RemainingCharges);
    Assert.Null(item.RemainingResistance);
    Assert.Null(item.NameOverride);
    Assert.Null(item.DescriptionOverride);
    Assert.Null(item.ValueOverride);

    Assert.NotNull(await SkillCraftContext.Characters.AsNoTracking().SingleOrDefaultAsync(x => x.Id == character.Id));

    ReadCharacterQuery query = new(character.Id);
    CharacterModel? model = await Pipeline.ExecuteAsync(query);
    Assert.NotNull(model);
    Assert.Equal(character, model);
  }
}
