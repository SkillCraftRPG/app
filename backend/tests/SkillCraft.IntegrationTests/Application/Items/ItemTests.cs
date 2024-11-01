using Logitar;
using Logitar.Portal.Contracts.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Application.Items.Commands;
using SkillCraft.Application.Items.Queries;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Items;
using SkillCraft.Contracts.Items.Properties;
using SkillCraft.Domain;
using SkillCraft.Domain.Items;
using SkillCraft.Domain.Items.Properties;

namespace SkillCraft.Application.Items;

[Trait(Traits.Category, Categories.Integration)]
public class ItemTests : IntegrationTests
{
  private readonly IItemRepository _itemRepository;

  private readonly Item _plateArmor;

  public ItemTests() : base()
  {
    _itemRepository = ServiceProvider.GetRequiredService<IItemRepository>();

    _plateArmor = new Item(World.Id, new Name("full-plate"), new EquipmentProperties(defense: 0, resistance: null, traits: []), UserId)
    {
      IsAttunementRequired = true
    };
    _plateArmor.Update(UserId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _itemRepository.SaveAsync(_plateArmor);
  }

  [Fact(DisplayName = "It should create a new weapon.")]
  public async Task It_should_create_a_new_weapon()
  {
    CreateOrReplaceItemPayload payload = new(" Poivrière ")
    {
      Description = "  Un pistolet doté d’un barillet multiple pouvant contenir jusqu’à 6 balles.  ",
      Value = 450.0,
      Weight = 2.5,
      IsAttunementRequired = true,
      Weapon = new WeaponPropertiesModel
      {
        Attack = 5,
        Resistance = 24,
        Traits = [WeaponTrait.Ammunition, WeaponTrait.Loading, WeaponTrait.Range, WeaponTrait.Reload],
        Damages = [new WeaponDamageModel("1d10", DamageType.Piercing)],
        Range = new WeaponRangeModel
        {
          Normal = 8,
          Long = 24
        },
        ReloadCount = 6
      }
    };
    CreateOrReplaceItemCommand command = new(Guid.NewGuid(), payload, Version: null);
    CreateOrReplaceItemResult result = await Pipeline.ExecuteAsync(command);
    Assert.True(result.Created);

    ItemModel? item = result.Item;
    Assert.NotNull(item);
    Assert.Equal(command.Id, item.Id);
    Assert.Equal(3, item.Version);
    Assert.Equal(DateTime.UtcNow, item.CreatedOn, TimeSpan.FromSeconds(1));
    Assert.True(item.CreatedOn < item.UpdatedOn);
    Assert.Equal(Actor, item.CreatedBy);
    Assert.Equal(item.CreatedBy, item.UpdatedBy);

    Assert.Equal(World.Id.ToGuid(), item.World.Id);

    Assert.Equal(payload.Name.Trim(), item.Name);
    Assert.Equal(payload.Description?.CleanTrim(), item.Description);

    Assert.Equal(payload.Value, item.Value);
    Assert.Equal(payload.Weight, item.Weight);

    Assert.Equal(payload.IsAttunementRequired, item.IsAttunementRequired);

    Assert.Equal(ItemCategory.Weapon, item.Category);
    Assert.Null(item.Consumable);
    Assert.Null(item.Container);
    Assert.Null(item.Device);
    Assert.Null(item.Equipment);
    Assert.Null(item.Miscellaneous);
    Assert.Null(item.Money);
    Assert.NotNull(item.Weapon);
    Assert.Equal(payload.Weapon.Attack, item.Weapon.Attack);
    Assert.Equal(payload.Weapon.Resistance, item.Weapon.Resistance);
    Assert.Equal(payload.Weapon.Traits, item.Weapon.Traits);
    Assert.Equal(payload.Weapon.Damages, item.Weapon.Damages);
    Assert.Equal(payload.Weapon.VersatileDamages, item.Weapon.VersatileDamages);
    Assert.Equal(payload.Weapon.Range, item.Weapon.Range);
    Assert.Equal(payload.Weapon.ReloadCount, item.Weapon.ReloadCount);

    Assert.NotNull(await SkillCraftContext.Items.AsNoTracking().SingleOrDefaultAsync(x => x.Id == item.Id));
  }

  [Fact(DisplayName = "It should replace an existing item.")]
  public async Task It_should_replace_an_existing_item()
  {
    long version = _plateArmor.Version;

    Description description = new("  Une imposante armure couvrant son porteur de la tête aux pieds. Elle est constituée de plaques de métal fixées par des sangles et des armatures de cuir. Les parties exposées comme les aisselles sont renforcées de mailles et du feutre est ajouté afin d’atténuer la chaleur du soleil. Malgré son poids imposant, toutes ses parties sont appuyées les unes sur les autres afin d’offrir une protection maximale tout en réduisant le moins possible la mobilité. Elle confère à son porteur l’immunité aux dégâts physiques tranchants. Si elle est magique, elle lui confère également l’immunité aux dégâts magiques tranchants.  ");
    _plateArmor.Description = description;
    _plateArmor.Update(UserId);
    await _itemRepository.SaveAsync(_plateArmor);

    CreateOrReplaceItemPayload payload = new(" Plate complète ")
    {
      Description = "    ",
      Value = 1500.0,
      Weight = 25.0,
      IsAttunementRequired = false,
      Equipment = new EquipmentPropertiesModel
      {
        Defense = 8,
        Resistance = 10,
        Traits = [EquipmentTrait.Bulwark, EquipmentTrait.Hybrid, EquipmentTrait.Noisy, EquipmentTrait.Sturdy]
      }
    };

    CreateOrReplaceItemCommand command = new(_plateArmor.EntityId, payload, version);
    CreateOrReplaceItemResult result = await Pipeline.ExecuteAsync(command);
    Assert.False(result.Created);

    ItemModel? item = result.Item;
    Assert.NotNull(item);
    Assert.Equal(command.Id, item.Id);
    Assert.Equal(_plateArmor.Version + 2, item.Version);
    Assert.Equal(DateTime.UtcNow, item.UpdatedOn, TimeSpan.FromSeconds(1));
    Assert.Equal(Actor, item.UpdatedBy);

    Assert.Equal(payload.Name.Trim(), item.Name);
    Assert.Equal(description.Value, item.Description);

    Assert.Equal(payload.Value, item.Value);
    Assert.Equal(payload.Weight, item.Weight);

    Assert.Equal(payload.IsAttunementRequired, item.IsAttunementRequired);

    Assert.Equal(ItemCategory.Equipment, item.Category);
    Assert.Null(item.Consumable);
    Assert.Null(item.Container);
    Assert.Null(item.Device);
    Assert.NotNull(item.Equipment);
    Assert.Equal(payload.Equipment.Defense, item.Equipment.Defense);
    Assert.Equal(payload.Equipment.Resistance, item.Equipment.Resistance);
    Assert.Equal(payload.Equipment.Traits, item.Equipment.Traits);
    Assert.Null(item.Miscellaneous);
    Assert.Null(item.Money);
    Assert.Null(item.Weapon);
  }

  [Fact(DisplayName = "It should return empty search results.")]
  public async Task It_should_return_empty_search_results()
  {
    SearchItemsPayload payload = new();
    payload.Search.Terms.Add(new SearchTerm("test"));

    SearchItemsQuery query = new(payload);
    SearchResults<ItemModel> results = await Pipeline.ExecuteAsync(query);

    Assert.Equal(0, results.Total);
    Assert.Empty(results.Items);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    _plateArmor.Name = new Name("Plate complète");
    _plateArmor.Value = 1500.0;
    _plateArmor.Weight = 25.0;
    _plateArmor.IsAttunementRequired = false;
    _plateArmor.Update(UserId);

    Item animatedShield = new(World.Id, new Name("Bouclier animé"), new EquipmentProperties(defense: 0, resistance: null, traits: []), UserId)
    {
      Value = 1000.0,
      Weight = 0,
      IsAttunementRequired = true
    };
    animatedShield.Update(UserId);
    Item brigandine = new(World.Id, new Name("Brigandine"), new EquipmentProperties(defense: 0, resistance: null, traits: []), UserId)
    {
      Value = 5.0,
      Weight = 4.0
    };
    brigandine.Update(UserId);
    Item heaterShield = new(World.Id, new Name("Écu"), new EquipmentProperties(defense: 2, resistance: 10, traits: []), UserId)
    {
      Value = 25.0,
      Weight = 4.0
    };
    heaterShield.Update(UserId);
    Item longsword = new(World.Id, new Name("Épée longue"), new WeaponProperties(attack: 0, resistance: null, traits: [], damages: [], versatileDamages: [], range: null, reloadCount: null), UserId)
    {
      Value = 15.0,
      Weight = 1.5
    };
    longsword.Update(UserId);
    Item plastron = new(World.Id, new Name("Plastron"), new EquipmentProperties(defense: 0, resistance: null, traits: []), UserId)
    {
      Value = 400.0,
      Weight = 10.0
    };
    plastron.Update(UserId);
    Item targe = new(World.Id, new Name("Targe"), new EquipmentProperties(defense: 0, resistance: null, traits: []), UserId)
    {
      Value = 10.0,
      Weight = 1.0
    };
    targe.Update(UserId);
    Item towerShield = new(World.Id, new Name("Pavois"), new EquipmentProperties(defense: 2, resistance: 20, traits: [EquipmentTrait.Bulwark, EquipmentTrait.Noisy]), UserId)
    {
      Value = 400.0,
      Weight = 8.0
    };
    towerShield.Update(UserId);

    await _itemRepository.SaveAsync([_plateArmor, animatedShield, brigandine, heaterShield, longsword, plastron, targe, towerShield]);

    SearchItemsPayload payload = new()
    {
      Category = ItemCategory.Equipment,
      IsAttunementRequired = false,
      Value = new DoubleFilter("lte", [1000]),
      Weight = new DoubleFilter("gt", [1]),
      Skip = 1,
      Limit = 1
    };
    payload.Search.Operator = SearchOperator.Or;
    payload.Search.Terms.Add(new SearchTerm("%bouclier%"));
    payload.Search.Terms.Add(new SearchTerm("É%"));
    payload.Search.Terms.Add(new SearchTerm("P%"));
    payload.Search.Terms.Add(new SearchTerm("T%"));
    payload.Sort.Add(new ItemSortOption(ItemSort.Value, isDescending: true));

    payload.Ids.Add(Guid.Empty);
    payload.Ids.AddRange((await _itemRepository.LoadAsync()).Select(item => item.EntityId));
    payload.Ids.Remove(plastron.EntityId);

    SearchItemsQuery query = new(payload);
    SearchResults<ItemModel> results = await Pipeline.ExecuteAsync(query);

    Assert.Equal(2, results.Total);
    ItemModel item = Assert.Single(results.Items);
    Assert.Equal(heaterShield.EntityId, item.Id);
  }

  [Fact(DisplayName = "It should return the item found by ID.")]
  public async Task It_should_return_the_item_found_by_Id()
  {
    ReadItemQuery query = new(_plateArmor.EntityId);
    ItemModel? item = await Pipeline.ExecuteAsync(query);
    Assert.NotNull(item);
    Assert.Equal(_plateArmor.EntityId, item.Id);
  }

  [Fact(DisplayName = "It should update an existing item.")]
  public async Task It_should_update_an_existing_item()
  {
    Description description = new("  Une imposante armure couvrant son porteur de la tête aux pieds. Elle est constituée de plaques de métal fixées par des sangles et des armatures de cuir. Les parties exposées comme les aisselles sont renforcées de mailles et du feutre est ajouté afin d’atténuer la chaleur du soleil. Malgré son poids imposant, toutes ses parties sont appuyées les unes sur les autres afin d’offrir une protection maximale tout en réduisant le moins possible la mobilité. Elle confère à son porteur l’immunité aux dégâts physiques tranchants. Si elle est magique, elle lui confère également l’immunité aux dégâts magiques tranchants.  ");
    _plateArmor.Description = description;
    _plateArmor.Update(UserId);
    await _itemRepository.SaveAsync(_plateArmor);

    UpdateItemPayload payload = new()
    {
      Name = " Plate complète ",
      Value = new Change<double?>(1500.0),
      Weight = new Change<double?>(25.0),
      Equipment = new EquipmentPropertiesModel
      {
        Defense = 8,
        Resistance = 10,
        Traits = [EquipmentTrait.Bulwark, EquipmentTrait.Hybrid, EquipmentTrait.Noisy, EquipmentTrait.Sturdy]
      }
    };
    UpdateItemCommand command = new(_plateArmor.EntityId, payload);
    ItemModel? item = await Pipeline.ExecuteAsync(command);

    Assert.NotNull(item);
    Assert.Equal(command.Id, item.Id);
    Assert.Equal(_plateArmor.Version + 2, item.Version);
    Assert.Equal(DateTime.UtcNow, item.UpdatedOn, TimeSpan.FromSeconds(1));
    Assert.Equal(Actor, item.UpdatedBy);

    Assert.Equal(payload.Name.Trim(), item.Name);
    Assert.Equal(description.Value, item.Description);

    Assert.Equal(payload.Value.Value, item.Value);
    Assert.Equal(payload.Weight.Value, item.Weight);

    Assert.Equal(_plateArmor.IsAttunementRequired, item.IsAttunementRequired);

    Assert.Equal(ItemCategory.Equipment, item.Category);
    Assert.Null(item.Consumable);
    Assert.Null(item.Container);
    Assert.Null(item.Device);
    Assert.NotNull(item.Equipment);
    Assert.Equal(payload.Equipment.Defense, item.Equipment.Defense);
    Assert.Equal(payload.Equipment.Resistance, item.Equipment.Resistance);
    Assert.Equal(payload.Equipment.Traits, item.Equipment.Traits);
    Assert.Null(item.Miscellaneous);
    Assert.Null(item.Money);
    Assert.Null(item.Weapon);
  }
}
