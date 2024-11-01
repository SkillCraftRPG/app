import type { Aggregate } from "./aggregate";
import type { SearchPayload, SortOption } from "./search";
import type { WorldModel } from "./worlds";

export type LanguageModel = Aggregate & {
  world: WorldModel;
  name: string;
  description?: string;
  script?: string;
  typicalSpeakers?: string;
};

export type LanguageSort = "CreatedOn" | "Name" | "UpdatedOn";

export type LanguageSortOption = SortOption & {
  field: LanguageSort;
};

export type SearchLanguagesPayload = SearchPayload & {
  script?: string;
  sort: LanguageSortOption[];
};
