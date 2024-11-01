import type { Aggregate } from "./aggregate";
import type { Skill } from "./game";
import type { SearchPayload, SortOption } from "./search";
import type { WorldModel } from "./worlds";

export type CasteModel = Aggregate & {
  world: WorldModel;
  name: string;
  description?: string;
  skill?: Skill;
  wealthRoll?: string;
  traits: TraitModel[];
};

export type CasteSort = "CreatedOn" | "Name" | "UpdatedOn";

export type CasteSortOption = SortOption & {
  field: CasteSort;
};

export type SearchCastesPayload = SearchPayload & {
  skill?: Skill;
  sort: CasteSortOption[];
};

export type TraitModel = {
  id: string;
  name: string;
  description?: string;
};
