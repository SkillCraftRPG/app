import type { Aggregate, Optional } from "./api";
import type { Feature } from "./features";
import type { SearchPayload, SortOption } from "./search";
import type { Skill } from "./game";

export type Caste = Aggregate & {
  name: string;
  summary?: string | null;
  content?: string | null;
  skill?: Skill | null;
  wealthRoll?: string | null;
  feature?: Feature | null;
};

export type CasteSort = "CreatedOn" | "Name" | "UpdatedOn";

export type CasteSortOption = SortOption & {
  field: CasteSort;
};

export type CreateOrReplaceCastePayload = {
  name: string;
  summary?: string | null;
  content?: string | null;
  skill?: Skill | null;
  wealthRoll?: string | null;
  feature?: Feature | null;
};

export type SearchCastesPayload = SearchPayload & {
  skill?: Skill | null;
  sort: CasteSortOption[];
};

export type UpdateCastePayload = {
  name?: string | null;
  summary?: Optional<string> | null;
  content?: Optional<string> | null;
  skill?: Optional<Skill | null> | null;
  wealthRoll?: Optional<string> | null;
  feature?: Optional<Feature> | null;
};
