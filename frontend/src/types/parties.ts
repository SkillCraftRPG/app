import type { Aggregate } from "./aggregate";
import type { SearchPayload, SortOption } from "./search";
import type { WorldModel } from "./worlds";

export type PartyModel = Aggregate & {
  world: WorldModel;
  name: string;
  description?: string;
};

export type PartySort = "CreatedOn" | "Name" | "UpdatedOn";

export type PartySortOption = SortOption & {
  field: PartySort;
};

export type SearchPartiesPayload = SearchPayload & {
  sort: PartySortOption[];
};
