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
  features: FeatureModel[];
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
  features: FeaturePayload[];
};

export type FeatureModel = {
  id: string;
  name: string;
  description?: string;
};

export type FeaturePayload = {
  id?: string;
  name: string;
  description?: string;
};

export type FeatureStatus = {
  feature: FeaturePayload;
  isRemoved: boolean;
  isUpdated: boolean;
};

export type FeatureUpdated = {
  index: number;
  feature: FeaturePayload;
};

export type SearchCastesPayload = SearchPayload & {
  skill?: Skill;
  sort: CasteSortOption[];
};
