import type { Actor } from "./actor";
import type { Aggregate } from "./aggregate";
import type { SearchPayload, SortOption } from "./search";

export type CreateOrReplaceWorldPayload = {
  slug: string;
  name?: string;
  description?: string;
};

export type SearchWorldsPayload = SearchPayload & {
  sort: WorldSortOption[];
};

export type WorldModel = Aggregate & {
  owner: Actor;
  slug: string;
  name?: string;
  description?: string;
};

export type WorldSort = "CreatedOn" | "Name" | "Slug" | "UpdatedOn";

export type WorldSortOption = SortOption & {
  field: WorldSort;
};
