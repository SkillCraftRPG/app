import type { Aggregate } from "./aggregate";
import type { NumberFilter, SearchPayload, SortOption } from "./search";
import type { Skill } from "./game";
import type { WorldModel } from "./worlds";

export type TalentModel = Aggregate & {
  world: WorldModel;
  tier: number;
  name: string;
  description?: string;
  allowMultiplePurchases: boolean;
  skill?: Skill;
  requiredTalent?: TalentModel;
};

export type TalentSort = "CreatedOn" | "Name" | "UpdatedOn";

export type TalentSortOption = SortOption & {
  field: TalentSort;
};

export type SearchTalentsPayload = SearchPayload & {
  allowMultiplePurchases?: boolean;
  hasSkill?: boolean;
  requiredTalentId?: string;
  tier?: NumberFilter;
  sort: TalentSortOption[];
};
