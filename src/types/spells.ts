import type { Aggregate, Optional } from "./api";
import type { SearchPayload, SortOption } from "./search";

export type CreateOrReplaceSpellPayload = {
  tier: number;
  name: string;
  summary?: string | null;
  content?: string | null;
};

export type SearchSpellsPayload = SearchPayload & {
  tiers?: number[];
  sort: SpellSortOption[];
};

export type Spell = Aggregate & {
  tier: number;
  name: string;
  summary?: string | null;
  content?: string | null;
};

export type SpellSort = "CreatedOn" | "Name" | "UpdatedOn";

export type SpellSortOption = SortOption & {
  field: SpellSort;
};

export type UpdateSpellPayload = {
  name?: string | null;
  summary?: Optional<string> | null;
  content?: Optional<string> | null;
};
