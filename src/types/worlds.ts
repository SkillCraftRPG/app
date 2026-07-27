import type { Actor, Aggregate } from "./api";
import type { SearchPayload, SortOption } from "./search";

export type SearchWorldsPayload = SearchPayload & {
  sort: WorldSortOption[];
};

export type World = Aggregate & {
  owner: Actor;
  key: string;
  name?: string | null;
  htmlContent?: string | null;
};

export type WorldSort = "CreatedOn" | "Key" | "Name" | "UpdatedOn";

export type WorldSortOption = SortOption & {
  field: WorldSort;
};
