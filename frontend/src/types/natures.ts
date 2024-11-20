import type { Aggregate } from "./aggregate";
import type { Attribute } from "./game";
import type { CustomizationModel } from "./customizations";
import type { SearchPayload, SortOption } from "./search";
import type { WorldModel } from "./worlds";

export type CreateOrReplaceNaturePayload = {
  name: string;
  description?: string;
  attribute?: Attribute;
  giftId?: string;
};

export type NatureModel = Aggregate & {
  world: WorldModel;
  name: string;
  description?: string;
  attribute?: Attribute;
  gift?: CustomizationModel;
};

export type NatureSort = "CreatedOn" | "Name" | "UpdatedOn";

export type NatureSortOption = SortOption & {
  field: NatureSort;
};

export type SearchNaturesPayload = SearchPayload & {
  attribute?: Attribute;
  giftId?: string;
  sort: NatureSortOption[];
};
