import type { Aggregate } from "./aggregate";
import type { SearchPayload, SortOption } from "./search";
import type { Skill } from "./game";
import type { WorldModel } from "./worlds";

export type CreateOrReplaceEducationPayload = {
  name: string;
  description?: string;
  skill?: Skill;
  wealthMultiplier?: number;
};

export type EducationModel = Aggregate & {
  world: WorldModel;
  name: string;
  description?: string;
  skill?: Skill;
  wealthMultiplier?: number;
};

export type EducationSort = "CreatedOn" | "Name" | "UpdatedOn";

export type EducationSortOption = SortOption & {
  field: EducationSort;
};

export type SearchEducationsPayload = SearchPayload & {
  skill?: Skill;
  sort: EducationSortOption[];
};
