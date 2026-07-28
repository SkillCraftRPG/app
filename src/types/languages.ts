import type { Aggregate, Optional } from "./api";
import type { Script } from "./scripts";
import type { SearchPayload, SortOption } from "./search";

export type CreateOrReplaceLanguagePayload = {
  name: string;
  summary?: string | null;
  htmlContent?: string | null;
  scriptId?: string | null;
  typicalSpeakers?: string | null;
};

export type Language = Aggregate & {
  name: string;
  summary?: string | null;
  htmlContent?: string | null;
  script?: Script | null;
  typicalSpeakers?: string | null;
};

export type LanguageSort = "CreatedOn" | "Name" | "UpdatedOn";

export type LanguageSortOption = SortOption & {
  field: LanguageSort;
};

export type SearchLanguagesPayload = SearchPayload & {
  sort: LanguageSortOption[];
};

export type UpdateLanguagePayload = {
  name?: string | null;
  summary?: Optional<string> | null;
  htmlContent?: Optional<string> | null;
  scriptId?: Optional<string> | null;
  typicalSpeakers?: Optional<string> | null;
};
