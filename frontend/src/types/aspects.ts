import type { Aggregate } from "./aggregate";
import type { Attribute, Skill } from "./game";
import type { SearchPayload, SortOption } from "./search";
import type { WorldModel } from "./worlds";

export type AspectModel = Aggregate & {
  world: WorldModel;
  name: string;
  description?: string;
  attributes: AttributeSelectionModel;
  skills: SkillsModel;
};

export type AspectSort = "CreatedOn" | "Name" | "UpdatedOn";

export type AspectSortOption = SortOption & {
  field: AspectSort;
};

export type AttributeSelectionModel = {
  mandatory1?: Attribute;
  mandatory2?: Attribute;
  optional1?: Attribute;
  optional2?: Attribute;
};

export type CreateOrReplaceAspectPayload = {
  name: string;
  description?: string;
  attributes: AttributeSelectionModel;
  skills: SkillsModel;
};

export type SearchAspectsPayload = SearchPayload & {
  attribute?: Attribute;
  skill?: Skill;
  sort: AspectSortOption[];
};

export type SkillsModel = {
  discounted1?: Skill;
  discounted2?: Skill;
};
