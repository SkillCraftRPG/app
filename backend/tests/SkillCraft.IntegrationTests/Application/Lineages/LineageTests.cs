using Logitar;
using Logitar.Portal.Contracts.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Application.Lineages.Commands;
using SkillCraft.Application.Lineages.Queries;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Lineages;
using SkillCraft.Domain;
using SkillCraft.Domain.Languages;
using SkillCraft.Domain.Lineages;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Application.Lineages;

[Trait(Traits.Category, Categories.Integration)]
public class LineageTests : IntegrationTests
{
  private readonly ILanguageRepository _languageRepository;
  private readonly ILineageRepository _lineageRepository;

  private readonly Language _commun;
  private readonly Language _orrinique;
  private readonly Language _vulcain;

  private readonly Lineage _humain;
  private readonly Lineage _vodyanoi;

  public LineageTests() : base()
  {
    _languageRepository = ServiceProvider.GetRequiredService<ILanguageRepository>();
    _lineageRepository = ServiceProvider.GetRequiredService<ILineageRepository>();

    _commun = new Language(World.Id, new Name("Commun"), UserId);
    _orrinique = new Language(World.Id, new Name("Orrinique"), UserId);
    _vulcain = new Language(World.Id, new Name("Vulcain"), UserId);

    _humain = new Lineage(World.Id, parent: null, new Name("Humain"), UserId)
    {
      Description = new Description("Les humains représentent le commun des mortels. Répandus sur toute la surface du monde, ils s’adaptent au climat des territoires sur lesquels ils s’installent. Leur apparence physique et leurs traits psychologiques varient grandement en fonction de leur habitat et de leurs traditions. Leur capacité d’apprentissage constitue une caractéristique qui leur est unique et qui surpasse celle des autres espèces."),
      Attributes = new AttributeBonuses(agility: 0, coordination: 0, intellect: 0, presence: 0, sensitivity: 0, spirit: 0, vigor: 0, extra: 2),
      Languages = new Domain.Lineages.Languages(languages: [], extra: 1, text: null),
      Names = new Names("Les humains portent généralement un prénom et un nom de famille.", family: [], female: [], male: [], unisex: [], custom: new Dictionary<string, IReadOnlyCollection<string>>()),
      Speeds = new Speeds(walk: 6, climb: 0, swim: 0, fly: 0, hover: 0, burrow: 0),
      Size = new Size(SizeCategory.Medium, new Roll("140+2d20")),
      Weight = new Weight(new Roll("11+1d4"), new Roll("15+1d4"), new Roll("19+1d6"), new Roll("26+1d6"), new Roll("32+1d10")),
      Ages = new Ages(8, 15, 21, 35)
    };
    _humain.AddFeature(new Feature(new Name("Apprentissage accéléré"), new Description("Le personnage débute avec 4 points d’Apprentissage supplémentaires et acquiert 1 point d’Apprentissage supplémentaire à chaque fois qu’il atteint un niveau pair (2, 4, 6, 8, 10, etc.).")));
    _humain.AddFeature(new Feature(new Name("Versatilité"), new Description("Le personnage acquiert gratuitement un talent associé à une compétence. Ces talents portent le même nom qu’une compétence.")));
    _humain.Update(UserId);

    _vodyanoi = new Lineage(World.Id, parent: null, new Name("vodyanoi"), UserId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _languageRepository.SaveAsync([_commun, _orrinique, _vulcain]);
    await _lineageRepository.SaveAsync([_humain, _vodyanoi]);
  }

  [Fact(DisplayName = "It should create a new nation.")]
  public async Task It_should_create_a_new_nation()
  {
    CreateOrReplaceLineagePayload payload = new(" Orrin ")
    {
      ParentId = _humain.EntityId,
      Description = "L’origine noble des Orrins remonte au peuple des Thronopoi, originaire du Derebon dans l’Ancien-Monde. Ils habitent l’Orrinie, un pays situé dans une péninsule montagnarde au Sud de l’Ouespéro, entourée par la mer Aspidée et la mer Mésienne. Leur culture guerrière, architecturale et artistique a le potentiel d’être exportée dans tous les confins de l’Ouespéro. Souvent en proie à des luttes internes et déchirantes, ils sont divisés en plusieurs entités géopolitiques et États croupions. Ils sont décrits comme étant de petite taille, avec des yeux et cheveux foncés.",
      Languages = new LanguagesPayload([_orrinique.EntityId]),
      Names = new NamesModel
      {
        Family = ["Aetos", "Bringas", "Condos", "Galanis", "Hondros", "Komnena", "Lykaios", "Maleina", "Sideris", "Xiphilinos"],
        Female = ["Alexandra", "Ariadnh", "Constantina", "Eritha", "Irene", "Karpathia", "Korinsia", "Poulxeria", "Sophia", "Theodora"],
        Male = ["Amphimedes", "Demetrios", "Ekhinos", "Glaukos", "Heracles", "Klymenos", "Orestes", "Photios", "Radamanqus", "Valens"]
      }
    };
    CreateOrReplaceLineageCommand command = new(Guid.NewGuid(), payload, Version: null);
    CreateOrReplaceLineageResult result = await Pipeline.ExecuteAsync(command);
    Assert.True(result.Created);

    LineageModel? lineage = result.Lineage;
    Assert.NotNull(lineage);
    Assert.Equal(command.Id, lineage.Id);
    Assert.Equal(2, lineage.Version);
    Assert.Equal(DateTime.UtcNow, lineage.CreatedOn, TimeSpan.FromSeconds(1));
    Assert.True(lineage.CreatedOn < lineage.UpdatedOn);
    Assert.Equal(Actor, lineage.CreatedBy);
    Assert.Equal(lineage.CreatedBy, lineage.UpdatedBy);

    Assert.Equal(World.Id.ToGuid(), lineage.World.Id);

    //Assert.NotNull(lineage.Species); // TODO(fpion): complete & optimize Querier
    //Assert.Equal(_humain.EntityId, lineage.Species.Id);

    Assert.Equal(payload.Name.Trim(), lineage.Name);
    Assert.Equal(payload.Description?.CleanTrim(), lineage.Description);

    Assert.Equal(_orrinique.EntityId, Assert.Single(lineage.Languages.Items).Id);
    Assert.Equal(0, lineage.Languages.Extra);
    Assert.Null(lineage.Languages.Text);

    Assert.Null(lineage.Names.Text);
    Assert.Equal(payload.Names.Family, lineage.Names.Family);
    Assert.Equal(payload.Names.Female, lineage.Names.Female);
    Assert.Equal(payload.Names.Male, lineage.Names.Male);
    Assert.Empty(lineage.Names.Unisex);
    Assert.Empty(lineage.Names.Custom);

    Assert.NotNull(await SkillCraftContext.Lineages.AsNoTracking().SingleOrDefaultAsync(x => x.Id == lineage.Id));
  }

  [Fact(DisplayName = "It should create a new species.")]
  public async Task It_should_create_a_new_species()
  {
    CreateOrReplaceLineagePayload payload = new(" Dhampir ")
    {
      Description = "  Le Dhampir est un humanoïde similaire en tout point aux humains. […]  ",
      Attributes = new AttributeBonusesModel
      {
        Agility = 1,
        Intellect = 1,
        Extra = 1
      },
      Features =
      [
        new FeaturePayload { Id = Guid.NewGuid(), Name = "Facultés surnaturelles" },
        new FeaturePayload { Name = " Morsure vampirique ", Description = "  Le personnage est doté d’une paire de canines acérées. Il peut les utiliser par un test de _Mêlée_ afin d’infliger 1d4 points de dégâts perçants additionnés à sa Force. Lorsqu’il réussit une attaque en les utilisant contre une créature n’étant pas un mort-vivant ni une construction, il peut dépenser des points d’Énergie afin de bénéficier d’un des deux effets suivants. […]  " }
      ],
      Languages = new LanguagesPayload
      {
        Extra = 2,
        Text = "Le personnage apprend la langue du peuple au sein duquel il a été élevé ainsi qu’une langue supplémentaire au choix."
      },
      Names = new NamesModel
      {
        Text = "Le personnage porte un prénom et un nom de famille en fonction du peuple au sein duquel il a été élevé. Son prénom est généralement sélectionné afin d’évoquer ses origines ténébreuses."
      },
      Speeds = new SpeedsModel
      {
        Walk = 7
      },
      Size = new SizeModel(SizeCategory.Medium, "140+2d20"),
      Weight = new WeightModel
      {
        Starved = "11+1d4",
        Skinny = "15+1d4",
        Normal = "19+1d6",
        Overweight = "26+1d6",
        Obese = "32+1d10"
      },
      Ages = new AgesModel
      {
        Adolescent = 8,
        Adult = 15,
        Mature = 275,
        Venerable = 750
      }
    };
    CreateOrReplaceLineageCommand command = new(Guid.NewGuid(), payload, Version: null);
    CreateOrReplaceLineageResult result = await Pipeline.ExecuteAsync(command);
    Assert.True(result.Created);

    LineageModel? lineage = result.Lineage;
    Assert.NotNull(lineage);
    Assert.Equal(command.Id, lineage.Id);
    Assert.Equal(2, lineage.Version);
    Assert.Equal(DateTime.UtcNow, lineage.CreatedOn, TimeSpan.FromSeconds(1));
    Assert.True(lineage.CreatedOn < lineage.UpdatedOn);
    Assert.Equal(Actor, lineage.CreatedBy);
    Assert.Equal(lineage.CreatedBy, lineage.UpdatedBy);

    Assert.Equal(World.Id.ToGuid(), lineage.World.Id);

    Assert.Null(lineage.Species);

    Assert.Equal(payload.Name.Trim(), lineage.Name);
    Assert.Equal(payload.Description?.CleanTrim(), lineage.Description);

    Assert.Equal(payload.Attributes, lineage.Attributes);
    Assert.Equal(2, lineage.Features.Count);
    Assert.Contains(lineage.Features, f => f.Id == payload.Features[0].Id && f.Name == payload.Features[0].Name);
    Assert.Contains(lineage.Features, f => f.Name == payload.Features[1].Name.Trim() && f.Description == payload.Features[1].Description?.Trim());

    Assert.Empty(lineage.Languages.Items);
    Assert.Equal(payload.Languages.Extra, lineage.Languages.Extra);
    Assert.Equal(payload.Languages.Text, lineage.Languages.Text);
    Assert.Equal(payload.Names.Text, lineage.Names.Text);
    Assert.Empty(lineage.Names.Family);
    Assert.Empty(lineage.Names.Female);
    Assert.Empty(lineage.Names.Male);
    Assert.Empty(lineage.Names.Unisex);
    Assert.Empty(lineage.Names.Custom);

    Assert.Equal(payload.Speeds, lineage.Speeds);
    Assert.Equal(payload.Size, lineage.Size);
    Assert.Equal(payload.Weight, lineage.Weight);
    Assert.Equal(payload.Ages, lineage.Ages);

    Assert.NotNull(await SkillCraftContext.Lineages.AsNoTracking().SingleOrDefaultAsync(x => x.Id == lineage.Id));
  }

  [Fact(DisplayName = "It should replace an existing lineage.")]
  public async Task It_should_replace_an_existing_lineage()
  {
    _vodyanoi.AddFeature(new Feature(new Name("Vision nocturne"), new Description("Le personnage voit dans la pénombre comme si la zone était dans la clarté à une distance de 18 mètres (12 cases). Dans l’obscurité, il ne voit qu’en teintes de gris, il ne peut donc pas distinguer les couleurs")));
    Guid featureId = Guid.NewGuid();
    _vodyanoi.SetFeature(featureId, new Feature(new Name("lutteur-amphibien"), Description: null));
    _vodyanoi.Update(UserId);
    await _lineageRepository.SaveAsync(_vodyanoi);

    long version = _vodyanoi.Version;

    Description description = new("Les Vodyanoy (singulier Vodyanoi) sont des hommes-animaux, hybrides entre humains et crapauds. Leur mode de vie non conventionnel, dû à leur habitat marécageux, explique leur crainte envers les étrangers et le fait qu’ils leur accordent difficilement leur confiance. Dans un environnement aquatique, ils sont de redoutables adversaires et s’adonnent à de dangereuses joutes de lutte. Par ailleurs, ce sport extrême leur a valu leur réputation, selon laquelle ils attirent les voyageurs dans les marais afin de les noyer.");
    _vodyanoi.Description = description;
    _vodyanoi.AddFeature(new Feature(new Name("Vodyanoi"), Description: null));
    _vodyanoi.Update(UserId);
    await _lineageRepository.SaveAsync(_vodyanoi);

    CreateOrReplaceLineagePayload payload = new(" Vodyanoi ")
    {
      ParentId = Guid.NewGuid(),
      Description = "    ",
      Attributes = new AttributeBonusesModel
      {
        Sensitivity = 1,
        Spirit = 1,
        Vigor = 1
      },
      Features =
      [
        new FeaturePayload { Name = " Habitat marécageux ", Description = "  Le personnage se voit conférer l’avantage à ses jets de sauvegarde contre les effets et pouvoirs de poisons. Il est également résistant aux points de dégâts de poison, et son seuil de tolérance à l’alcool augmente de 1. Il est capable de communiquer des idées simples aux amphibiens.  " },
        new FeaturePayload { Id = featureId, Name = "Lutteur amphibien" }
      ],
      Languages = new LanguagesPayload
      {
        Ids = [_commun.EntityId],
        Extra = 1,
        Text = "  Les Vodyanoy connaissent également le langage secret de leur espèce, qu’ils feront tout pour conserver inconnu. Il est exprimé par des sons de bouche et des signes. Lorsqu’ils parlent, ils exagèrent les sons de B, de C/K/Q et de R.  "
      },
      Names = new NamesModel
      {
        Text = "  Les Vodyanoy portent des noms unisexes et ne portent pas de nom de famille. Ils révèlent rarement leur vrai nom à ceux qui n’ont pas pleinement gagné leur confiance, utilisant le pseudonyme _Vodnik_ afin de se désigner.  ",
        Unisex = ["Berrik", "Dema", "Gebbal", "Kodav", "Koru", "Krilim", "Mibar", "Rarrat", "Tobbut", "Vorran"]
      },
      Speeds = new SpeedsModel
      {
        Walk = 6,
        Swim = 6
      },
      Size = new SizeModel(SizeCategory.Medium, "160+3d10"),
      Weight = new WeightModel
      {
        Starved = "13+1d4",
        Skinny = "17+1d4",
        Normal = "21+1d6",
        Overweight = "27+1d8",
        Obese = "35+1d10"
      },
      Ages = new AgesModel
      {
        Adolescent = 8,
        Adult = 15,
        Mature = 40,
        Venerable = 70
      }
    };
    CreateOrReplaceLineageCommand command = new(_vodyanoi.EntityId, payload, version);
    CreateOrReplaceLineageResult result = await Pipeline.ExecuteAsync(command);
    Assert.False(result.Created);

    LineageModel? lineage = result.Lineage;
    Assert.NotNull(lineage);
    Assert.Equal(command.Id, lineage.Id);
    Assert.Equal(version + 2, lineage.Version);
    Assert.Equal(DateTime.UtcNow, lineage.CreatedOn, TimeSpan.FromSeconds(1));
    Assert.True(lineage.CreatedOn < lineage.UpdatedOn);
    Assert.Equal(Actor, lineage.CreatedBy);
    Assert.Equal(lineage.CreatedBy, lineage.UpdatedBy);

    Assert.Equal(World.Id.ToGuid(), lineage.World.Id);

    Assert.Null(lineage.Species);

    Assert.Equal(payload.Name.Trim(), lineage.Name);
    Assert.Equal(description.Value, lineage.Description);

    Assert.Equal(payload.Attributes, lineage.Attributes);
    Assert.Equal(3, lineage.Features.Count);
    Assert.Contains(lineage.Features, f => f.Name == payload.Features[0].Name.Trim() && f.Description == payload.Features[0].Description?.Trim());
    Assert.Contains(lineage.Features, f => f.Id == payload.Features[1].Id && f.Name == payload.Features[1].Name);
    Assert.Contains(lineage.Features, f => f.Name == "Vodyanoi");

    Assert.Equal(payload.Languages.Ids, lineage.Languages.Items.Select(language => language.Id));
    Assert.Equal(payload.Languages.Extra, lineage.Languages.Extra);
    Assert.Equal(payload.Languages.Text.Trim(), lineage.Languages.Text);
    Assert.Equal(payload.Names.Text.Trim(), lineage.Names.Text);
    Assert.Empty(lineage.Names.Family);
    Assert.Empty(lineage.Names.Female);
    Assert.Empty(lineage.Names.Male);
    Assert.Equal(payload.Names.Unisex, lineage.Names.Unisex);
    Assert.Empty(lineage.Names.Custom);

    Assert.Equal(payload.Speeds, lineage.Speeds);
    Assert.Equal(payload.Size, lineage.Size);
    Assert.Equal(payload.Weight, lineage.Weight);
    Assert.Equal(payload.Ages, lineage.Ages);
  }

  [Fact(DisplayName = "It should return empty search results.")]
  public async Task It_should_return_empty_search_results()
  {
    SearchLineagesPayload payload = new();
    payload.Search.Terms.Add(new SearchTerm("test"));

    SearchLineagesQuery query = new(payload);
    SearchResults<LineageModel> results = await Pipeline.ExecuteAsync(query);

    Assert.Equal(0, results.Total);
    Assert.Empty(results.Items);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    _humain.Languages = new Domain.Lineages.Languages([_commun], extra: 0, text: null);
    _humain.Update(UserId);

    Lineage ashtavrin = new(World.Id, parent: null, new Name("Ashtavrin"), UserId)
    {
      Attributes = new AttributeBonuses(agility: 0, coordination: 0, intellect: 0, presence: 1, sensitivity: 0, spirit: 1, vigor: 0, extra: 1),
      Languages = new Domain.Lineages.Languages([_commun], extra: 1, text: null),
      Size = new Size(SizeCategory.Medium, new Roll("150+3d10"))
    };
    ashtavrin.Update(UserId);
    Lineage celestin = new(World.Id, parent: null, new Name("Célestin"), UserId)
    {
      Attributes = new AttributeBonuses(agility: 0, coordination: 0, intellect: 0, presence: 1, sensitivity: 0, spirit: 1, vigor: 0, extra: 1),
      Languages = new Domain.Lineages.Languages([_commun], extra: 1, text: null),
      Size = new Size(SizeCategory.Medium, new Roll("140+2d20"))
    };
    celestin.Update(UserId);
    Lineage infernon = new(World.Id, parent: null, new Name("Infernon"), UserId)
    {
      Attributes = new AttributeBonuses(agility: 0, coordination: 0, intellect: 0, presence: 1, sensitivity: 0, spirit: 1, vigor: 0, extra: 1),
      Languages = new Domain.Lineages.Languages([_commun], extra: 1, text: null),
      Size = new Size(SizeCategory.Medium, new Roll("140+2d20"))
    };
    infernon.Update(UserId);
    Lineage salamandre = new(World.Id, parent: null, new Name("Salamandre"), UserId)
    {
      Attributes = new AttributeBonuses(agility: 0, coordination: 1, intellect: 0, presence: 0, sensitivity: 1, spirit: 1, vigor: 0, extra: 0),
      Languages = new Domain.Lineages.Languages([_vulcain], extra: 1, text: null),
      Size = new Size(SizeCategory.Medium, new Roll("140+2d20"))
    };
    salamandre.Update(UserId);
    Lineage sangDragon = new(World.Id, parent: salamandre, new Name("Sang-dragon"), UserId)
    {
      Attributes = new AttributeBonuses(agility: 0, coordination: 0, intellect: 0, presence: 1, sensitivity: 0, spirit: 1, vigor: 1, extra: 0),
      Languages = new Domain.Lineages.Languages([_commun], extra: 1, text: null),
      Size = new Size(SizeCategory.Medium, new Roll("180+3d10"))
    };
    sangDragon.Update(UserId);
    Lineage sylphe = new(World.Id, parent: null, new Name("Sylphe"), UserId)
    {
      Attributes = new AttributeBonuses(agility: 1, coordination: 0, intellect: 0, presence: 1, sensitivity: 0, spirit: 1, vigor: 1, extra: 0),
      Languages = new Domain.Lineages.Languages([_commun], extra: 1, text: null),
      Size = new Size(SizeCategory.Small, new Roll("80+2d10"))
    };
    sylphe.Update(UserId);
    Lineage triton = new(World.Id, parent: null, new Name("Triton"), UserId)
    {
      Attributes = new AttributeBonuses(agility: 1, coordination: 0, intellect: 0, presence: 0, sensitivity: 0, spirit: 1, vigor: 1, extra: 0),
      Languages = new Domain.Lineages.Languages([_commun], extra: 1, text: null),
      Size = new Size(SizeCategory.Medium, new Roll("140+2d20"))
    };
    triton.Update(UserId);

    await _lineageRepository.SaveAsync([_humain, ashtavrin, celestin, infernon, salamandre, sangDragon, sylphe, triton]);

    SearchLineagesPayload payload = new()
    {
      Attribute = Attribute.Spirit,
      LanguageId = _commun.EntityId,
      ParentId = null,
      SizeCategory = SizeCategory.Medium,
      Skip = 1,
      Limit = 1
    };
    payload.Search.Operator = SearchOperator.Or;
    payload.Search.Terms.Add(new SearchTerm("%in%"));
    payload.Search.Terms.Add(new SearchTerm("S%"));
    payload.Sort.Add(new LineageSortOption(LineageSort.Name, isDescending: true));

    payload.Ids.Add(Guid.Empty);
    payload.Ids.AddRange((await _lineageRepository.LoadAsync()).Select(lineage => lineage.EntityId));
    payload.Ids.Remove(ashtavrin.EntityId);

    SearchLineagesQuery query = new(payload);
    SearchResults<LineageModel> results = await Pipeline.ExecuteAsync(query);

    Assert.Equal(2, results.Total);
    LineageModel lineage = Assert.Single(results.Items);
    Assert.Equal(celestin.EntityId, lineage.Id);
  }

  [Fact(DisplayName = "It should return the lineage found by ID.")]
  public async Task It_should_return_the_lineage_found_by_Id()
  {
    ReadLineageQuery query = new(_humain.EntityId);
    LineageModel? lineage = await Pipeline.ExecuteAsync(query);
    Assert.NotNull(lineage);
    Assert.Equal(_humain.EntityId, lineage.Id);
  }

  [Fact(DisplayName = "It should update an existing lineage.")]
  public async Task It_should_update_an_existing_lineage()
  {
    Description description = new("Les Vodyanoy (singulier Vodyanoi) sont des hommes-animaux, hybrides entre humains et crapauds. Leur mode de vie non conventionnel, dû à leur habitat marécageux, explique leur crainte envers les étrangers et le fait qu’ils leur accordent difficilement leur confiance. Dans un environnement aquatique, ils sont de redoutables adversaires et s’adonnent à de dangereuses joutes de lutte. Par ailleurs, ce sport extrême leur a valu leur réputation, selon laquelle ils attirent les voyageurs dans les marais afin de les noyer.");
    _vodyanoi.Description = description;

    _vodyanoi.Attributes = new AttributeBonuses(agility: 1, coordination: 1, intellect: 0, presence: 0, sensitivity: 0, spirit: 1, vigor: 0, extra: 0);

    _vodyanoi.Languages = new Domain.Lineages.Languages([_commun], extra: 0, text: null);
    _vodyanoi.Names = new Names(text: null, family: [], female: [], male: [], unisex: ["Berrik", "Dema", "Gebbal", "Kodav", "Koru", "Krilim", "Mibar", "Rarrat", "Tobbut", "Vorran"], custom: new Dictionary<string, IReadOnlyCollection<string>>());
    _vodyanoi.Speeds = new Speeds(walk: 6, climb: 0, swim: 0, fly: 0, hover: 0, burrow: 0);
    _vodyanoi.Size = new Size(SizeCategory.Large, new Roll("160+3d10"));
    _vodyanoi.Weight = new Weight(new Roll("13+1d4"), new Roll("17+1d4"), normal: null, new Roll("27+1d8"), new Roll("35+1d10"));
    _vodyanoi.Ages = new Ages(8, 15, 40, venerable: null);

    _vodyanoi.AddFeature(new Feature(new Name("Vodyanoi"), Description: null));

    Guid lutteurAmphibienId = Guid.NewGuid();
    _vodyanoi.SetFeature(lutteurAmphibienId, new Feature(new Name("lutteur-amphibien"), Description: null));

    Guid visionNocturneId = Guid.NewGuid();
    _vodyanoi.SetFeature(visionNocturneId, new Feature(new Name("Vision nocturne"), Description: null));

    _vodyanoi.Update(UserId);
    await _lineageRepository.SaveAsync(_vodyanoi);

    UpdateLineagePayload payload = new()
    {
      Name = "Vodyanoi",
      Attributes = new UpdateAttributeBonusesPayload
      {
        Agility = 0,
        Coordination = 0,
        Sensitivity = 1,
        Vigor = 1
      },
      Features =
      [
        new UpdateFeaturePayload(" Habitat marécageux ") { Description = "  Le personnage se voit conférer l’avantage à ses jets de sauvegarde contre les effets et pouvoirs de poisons. Il est également résistant aux points de dégâts de poison, et son seuil de tolérance à l’alcool augmente de 1. Il est capable de communiquer des idées simples aux amphibiens.  " },
        new UpdateFeaturePayload("Lutteur amphibien") { Id = lutteurAmphibienId },
        new UpdateFeaturePayload("Vision nocturne") { Id = visionNocturneId, Remove = true }
      ],
      Languages = new UpdateLanguagesPayload
      {
        Extra = 1,
        Text = new Change<string>("  Les Vodyanoy connaissent également le langage secret de leur espèce, qu’ils feront tout pour conserver inconnu. Il est exprimé par des sons de bouche et des signes. Lorsqu’ils parlent, ils exagèrent les sons de B, de C/K/Q et de R.  ")
      },
      Names = new UpdateNamesPayload
      {
        Text = new Change<string>("  Les Vodyanoy portent des noms unisexes et ne portent pas de nom de famille. Ils révèlent rarement leur vrai nom à ceux qui n’ont pas pleinement gagné leur confiance, utilisant le pseudonyme _Vodnik_ afin de se désigner.  ")
      },
      Speeds = new UpdateSpeedsPayload
      {
        Swim = 6
      },
      Size = new UpdateSizePayload
      {
        Category = SizeCategory.Medium
      },
      Weight = new UpdateWeightPayload
      {
        Normal = "27+1d6"
      },
      Ages = new UpdateAgesPayload
      {
        Venerable = new Change<int?>(70)
      }
    };
    UpdateLineageCommand command = new(_vodyanoi.EntityId, payload);
    LineageModel? lineage = await Pipeline.ExecuteAsync(command);

    Assert.NotNull(lineage);
    Assert.Equal(command.Id, lineage.Id);
    Assert.Equal(_vodyanoi.Version + 1, lineage.Version);
    Assert.Equal(DateTime.UtcNow, lineage.UpdatedOn, TimeSpan.FromSeconds(1));
    Assert.Equal(Actor, lineage.UpdatedBy);

    Assert.Equal(payload.Name.Trim(), lineage.Name);
    Assert.Equal(description.Value, lineage.Description);

    Assert.Equal(0, lineage.Attributes.Agility);
    Assert.Equal(0, lineage.Attributes.Coordination);
    Assert.Equal(1, lineage.Attributes.Sensitivity);
    Assert.Equal(1, lineage.Attributes.Spirit);
    Assert.Equal(1, lineage.Attributes.Vigor);

    Assert.Equal(3, lineage.Features.Count);
    Assert.Contains(lineage.Features, f => f.Name == "Vodyanoi");
    Assert.Contains(lineage.Features, f => f.Id == lutteurAmphibienId && f.Name == "Lutteur amphibien");
    Assert.Contains(lineage.Features, f => f.Name == payload.Features.First().Name.Trim() && f.Description == payload.Features.First().Description?.Trim());

    Assert.Equal(_commun.EntityId, Assert.Single(lineage.Languages.Items).Id);
    Assert.Equal(payload.Languages.Extra, lineage.Languages.Extra);
    Assert.Equal(payload.Languages.Text.Value?.Trim(), lineage.Languages.Text);

    Assert.Equal(payload.Names.Text.Value?.Trim(), lineage.Names.Text);
    Assert.NotEmpty(lineage.Names.Unisex);

    Assert.Equal(6, lineage.Speeds.Walk);
    Assert.Equal(payload.Speeds.Swim, lineage.Speeds.Swim);

    Assert.Equal(payload.Size.Category, lineage.Size.Category);
    Assert.Equal("160+3d10", lineage.Size.Roll);

    Assert.Equal("13+1d4", lineage.Weight.Starved);
    Assert.Equal("17+1d4", lineage.Weight.Skinny);
    Assert.Equal(payload.Weight.Normal, lineage.Weight.Normal);
    Assert.Equal("27+1d8", lineage.Weight.Overweight);
    Assert.Equal("35+1d10", lineage.Weight.Obese);

    Assert.Equal(8, lineage.Ages.Adolescent);
    Assert.Equal(15, lineage.Ages.Adult);
    Assert.Equal(40, lineage.Ages.Mature);
    Assert.Equal(payload.Ages.Venerable.Value, lineage.Ages.Venerable);
  }
}
