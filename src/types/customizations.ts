import type { Aggregate, Optional } from "./api";
import type { SearchPayload, SortOption } from "./search";

export type CreateOrReplaceCustomizationPayload = {
  kind: CustomizationKind;
  name: string;
  summary?: string | null;
  content?: string | null;
};

export type Customization = Aggregate & {
  kind: CustomizationKind;
  name: string;
  summary?: string | null;
  content?: string | null;
};

export type CustomizationKind = "Disability" | "Gift";

export type CustomizationSort = "CreatedOn" | "Name" | "UpdatedOn";

export type CustomizationSortOption = SortOption & {
  field: CustomizationSort;
};

export type SearchCustomizationsPayload = SearchPayload & {
  kind?: CustomizationKind | null;
  sort: CustomizationSortOption[];
};

export type UpdateCustomizationPayload = {
  name?: string | null;
  summary?: Optional<string> | null;
  content?: Optional<string> | null;
};
