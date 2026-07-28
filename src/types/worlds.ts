import type { Actor, Aggregate, Optional } from "./api";
import type { SearchPayload, SortOption } from "./search";

export type CreateOrReplaceWorldPayload = {
  key: string;
  name?: string | null;
  htmlContent?: string | null;
};

export type SearchWorldsPayload = SearchPayload & {
  sort: WorldSortOption[];
};

export type UpdateWorldPayload = {
  key?: string | null;
  name?: Optional<string> | null;
  htmlContent?: Optional<string> | null;
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
