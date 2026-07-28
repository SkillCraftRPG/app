import type { Actor, Aggregate, Optional } from "./api";
import type { SearchPayload, SortOption } from "./search";

export type CreateOrReplaceScriptPayload = {
  name: string;
  summary?: string | null;
  htmlContent?: string | null;
};

export type Script = Aggregate & {
  name: string;
  summary?: string | null;
  htmlContent?: string | null;
};

export type SearchScriptsPayload = SearchPayload & {
  sort: ScriptSortOption[];
};

export type ScriptSort = "CreatedOn" | "Name" | "UpdatedOn";

export type ScriptSortOption = SortOption & {
  field: ScriptSort;
};

export type UpdateScriptPayload = {
  name?: string | null;
  summary?: Optional<string> | null;
  htmlContent?: Optional<string> | null;
};
