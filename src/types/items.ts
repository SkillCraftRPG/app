import type { Aggregate, Optional } from "./api";
import type { SearchPayload, SortOption } from "./search";

export type CreateOrReplaceItemPayload = {
  name: string;
  summary?: string | null;
  content?: string | null;
  price?: number | null;
  weight?: number | null;
};

export type Item = Aggregate & {
  name: string;
  summary?: string | null;
  content?: string | null;
  price?: number | null;
  weight?: number | null;
};

export type ItemSort = "CreatedOn" | "Name" | "Price" | "UpdatedOn" | "Weight";

export type ItemSortOption = SortOption & {
  field: ItemSort;
};

export type SearchItemsPayload = SearchPayload & {
  sort: ItemSortOption[];
};

export type UpdateItemPayload = {
  name?: string | null;
  summary?: Optional<string> | null;
  content?: Optional<string> | null;
  price?: Optional<number | null> | null;
  weight?: Optional<number | null> | null;
};
