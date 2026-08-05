import type { Aggregate, Optional } from "./api";
import type { SearchPayload, SortOption } from "./search";
import type { Skill } from "./game";

export type CreateOrReplaceTalentPayload = {
  tier: number;
  name: string;
  summary?: string | null;
  content?: string | null;
  allowMultiplePurchases?: boolean;
  skill?: Skill | null;
  requiredTalentId?: string | null;
};

export type SearchTalentsPayload = SearchPayload & {
  tiers?: number[];
  allowMultiplePurchases?: boolean | null;
  skill?: string | null;
  requiredTalentId?: string | null;
  sort: TalentSortOption[];
};

export type Talent = Aggregate & {
  tier: number;
  name: string;
  summary?: string | null;
  content?: string | null;
  allowMultiplePurchases: boolean;
  skill?: Skill | null;
  requiredTalent?: Talent | null;
};

export type TalentSort = "CreatedOn" | "Name" | "UpdatedOn";

export type TalentSortOption = SortOption & {
  field: TalentSort;
};

export type UpdateTalentPayload = {
  name?: string | null;
  summary?: Optional<string> | null;
  content?: Optional<string> | null;
  allowMultiplePurchases?: boolean | null;
  skill?: Optional<Skill | null> | null;
  requiredTalentId?: Optional<string | null> | null;
};
