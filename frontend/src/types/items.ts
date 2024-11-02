import type { Aggregate } from "./aggregate";
import type { DamageType } from "./game";
import type { NumberFilter, SearchPayload, SortOption } from "./search";
import type { WorldModel } from "./worlds";

export type ConsumablePropertiesModel = {
  charges?: number;
  removeWhenEmpty: boolean;
  replaceWithItemWhenEmptyId?: string;
};

export type ContainerPropertiesModel = {
  capacity?: number;
  volume?: number;
};

export type CreateOrReplaceItemPayload = {
  name: string;
  description?: string;
  value?: number;
  weight?: number;
  isAttunementRequired: boolean;
  consumable?: ConsumablePropertiesModel;
  container?: ContainerPropertiesModel;
  device?: DevicePropertiesModel;
  equipment?: EquipmentPropertiesModel;
  miscellaneous?: MiscellaneousPropertiesModel;
  money?: MoneyPropertiesModel;
  weapon?: WeaponPropertiesModel;
};

export type DevicePropertiesModel = {};

export type EquipmentPropertiesModel = {
  defense: number;
  resistance?: number;
  traits: EquipmentTrait[];
};

export type EquipmentTrait = "Bulwark" | "Comfort" | "Hybrid" | "Noisy" | "Quilted" | "Sturdy";

export type ItemCategory = "Consumable" | "Container" | "Device" | "Equipment" | "Miscellaneous" | "Money" | "Weapon";

export type ItemModel = Aggregate & {
  world: WorldModel;
  name: string;
  description?: string;
  value?: number;
  weight?: number;
  isAttunementRequired: boolean;
  category: ItemCategory;
  consumable?: ConsumablePropertiesModel;
  container?: ContainerPropertiesModel;
  device?: DevicePropertiesModel;
  equipment?: EquipmentPropertiesModel;
  miscellaneous?: MiscellaneousPropertiesModel;
  money?: MoneyPropertiesModel;
  weapon?: WeaponPropertiesModel;
};

export type ItemSort = "CreatedOn" | "Name" | "UpdatedOn" | "Value" | "Weight";

export type ItemSortOption = SortOption & {
  field: ItemSort;
};

export type MiscellaneousPropertiesModel = {};

export type MoneyPropertiesModel = {};

export type SearchItemsPayload = SearchPayload & {
  category?: ItemCategory;
  isAttunementRequired?: boolean;
  value?: NumberFilter;
  weight?: NumberFilter;
  sort: ItemSortOption[];
};

export type WeaponDamageModel = {
  roll: string;
  type: DamageType;
};

export type WeaponPropertiesModel = {
  attack: number;
  resistance?: number;
  traits: WeaponTrait[];
  damages: WeaponDamageModel[];
  versatileDamages: WeaponDamageModel[];
  range?: WeaponRangeModel;
  reloadCount?: number;
};

export type WeaponRangeModel = {
  normal?: number;
  long?: number;
};

export type WeaponTrait =
  | "Ammunition"
  | "Finesse"
  | "Heavy"
  | "Light"
  | "Loading"
  | "Range"
  | "Reach"
  | "Reload"
  | "Scatter"
  | "Special"
  | "Thrown"
  | "TwoHanded"
  | "Versatile";
