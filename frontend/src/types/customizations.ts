import type { Aggregate } from "./aggregate";
import type { SearchPayload, SortOption } from "./search";
import type { WorldModel } from "./worlds";

export type CreateOrReplaceCustomizationPayload = {
  type: CustomizationType;
  name: string;
  description?: string;
};

export type CustomizationModel = Aggregate & {
  world: WorldModel;
  type: CustomizationType;
  name: string;
  description?: string;
};

export type CustomizationSort = "CreatedOn" | "Name" | "UpdatedOn";

export type CustomizationSortOption = SortOption & {
  field: CustomizationSort;
};

export type CustomizationType = "Disability" | "Gift";

export type SearchCustomizationsPayload = SearchPayload & {
  type?: CustomizationType;
  sort: CustomizationSortOption[];
};
