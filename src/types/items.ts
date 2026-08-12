import type { Aggregate, Optional } from "./api";
import type { SearchPayload, SortOption } from "./search";

export type CreateOrReplaceItemPayload = {
  category: ItemCategory;
  name: string;
  summary?: string | null;
  content?: string | null;
  price?: number | null;
  weight?: number | null;
  charges?: ItemChargesPayload | null;
};

export type DepletionBehavior = "Keep" | "Remove" | "Replace";

export type Item = Aggregate & {
  category: ItemCategory;
  name: string;
  summary?: string | null;
  content?: string | null;
  price?: number | null;
  weight?: number | null;
  charges?: ItemCharges | null;
};

export type ItemCategory =
  | "Ammunition"
  | "Armor"
  | "Clothing"
  | "Consumable"
  | "Container"
  | "Currency"
  | "Material"
  | "Miscellaneous"
  | "Shield"
  | "Tool"
  | "Treasure"
  | "Weapon";

export type ItemCharges = {
  maximum: number;
  depletionBehavior: DepletionBehavior;
  replacement?: Item | null;
};

export type ItemChargesPayload = {
  maximum: number;
  depletionBehavior: DepletionBehavior;
  replacementId?: string | null;
};

export type ItemSort = "CreatedOn" | "Name" | "Price" | "UpdatedOn" | "Weight";

export type ItemSortOption = SortOption & {
  field: ItemSort;
};

export type SearchItemsPayload = SearchPayload & {
  category?: ItemCategory | null;
  sort: ItemSortOption[];
};

export type UpdateItemPayload = {
  name?: string | null;
  summary?: Optional<string> | null;
  content?: Optional<string> | null;
  price?: Optional<number> | null;
  weight?: Optional<number> | null;
  charges?: Optional<ItemChargesPayload> | null;
};
