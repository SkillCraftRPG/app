import type { Aggregate, Optional } from "./api";
import type { Feature } from "./features";
import type { SearchPayload, SortOption } from "./search";
import type { Skill } from "./game";

export type CreateOrReplaceEducationPayload = {
  name: string;
  summary?: string | null;
  content?: string | null;
  skill?: Skill | null;
  wealthMultiplier?: number | null;
  feature?: Feature | null;
};

export type Education = Aggregate & {
  name: string;
  summary?: string | null;
  content?: string | null;
  skill?: Skill | null;
  wealthMultiplier?: number | null;
  feature?: Feature | null;
};

export type EducationSort = "CreatedOn" | "Name" | "UpdatedOn";

export type EducationSortOption = SortOption & {
  field: EducationSort;
};

export type SearchEducationsPayload = SearchPayload & {
  skill?: Skill | null;
  sort: EducationSortOption[];
};

export type UpdateEducationPayload = {
  name?: string | null;
  summary?: Optional<string> | null;
  content?: Optional<string> | null;
  skill?: Optional<Skill | null> | null;
  wealthMultiplier?: Optional<number | null> | null;
  feature?: Optional<Feature> | null;
};
