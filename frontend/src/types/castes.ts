import type { Aggregate } from "./aggregate";
import type { SearchPayload, SortOption } from "./search";
import type { Skill } from "./game";
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

export type CreateOrReplaceCastePayload = {
  name: string;
  description?: string;
  skill?: Skill;
  wealthRoll?: string;
  traits: TraitPayload[];
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

export type TraitPayload = {
  id?: string;
  name: string;
  description?: string;
};

export type TraitStatus = {
  trait: TraitPayload;
  isRemoved: boolean;
  isUpdated: boolean;
};

export type TraitUpdated = {
  index: number;
  trait: TraitPayload;
};
