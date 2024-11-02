import type { Aggregate } from "./aggregate";
import type { Attribute } from "./game";
import type { CustomizationModel } from "./customizations";
import type { SearchPayload, SortOption } from "./search";
import type { WorldModel } from "./worlds";

export type CreateOrReplacePersonalityPayload = {
  name: string;
  description?: string;
  attribute?: Attribute;
  giftId?: string;
};

export type PersonalityModel = Aggregate & {
  world: WorldModel;
  name: string;
  description?: string;
  attribute?: Attribute;
  gift?: CustomizationModel;
};

export type PersonalitySort = "CreatedOn" | "Name" | "UpdatedOn";

export type PersonalitySortOption = SortOption & {
  field: PersonalitySort;
};

export type SearchPersonalitiesPayload = SearchPayload & {
  attribute?: Attribute;
  giftId?: string;
  sort: PersonalitySortOption[];
};
