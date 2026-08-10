import type { Aggregate, Optional } from "./api";
import type { SearchPayload, SortOption } from "./search";

export type CreateOrReplaceItemPayload = {
  category: ItemCategory;
  name: string;
  summary?: string | null;
  content?: string | null;
  price?: number | null;
  weight?: number | null;
};

export type Item = Aggregate & {
  category: ItemCategory;
  name: string;
  summary?: string | null;
  content?: string | null;
  price?: number | null;
  weight?: number | null;
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
  price?: Optional<number | null> | null;
  weight?: Optional<number | null> | null;
};
