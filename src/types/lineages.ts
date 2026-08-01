import type { Actor, Aggregate, Optional } from "./api";
import type { Language } from "./languages";
import type { SearchPayload, SortOption } from "./search";
import type { SizeCategory } from "./game";
import type { Feature } from "./features";

export type CreateOrReplaceLineagePayload = {
  parentId?: string | null;
  name: string;
  summary?: string | null;
  content?: string | null;
  languages?: LineageLanguagesPayload;
  names?: LineageNames;
  speeds?: LineageSpeeds;
  size?: LineageSize;
  weight?: LineageWeight;
  age?: LineageAge;
};

export type Lineage = Aggregate & {
  name: string;
  summary?: string | null;
  content?: string | null;
  features: LineageFeature[];
  languages: LineageLanguages;
  names: LineageNames;
  speeds: LineageSpeeds;
  size: LineageSize;
  weight: LineageWeight;
  age: LineageAge;
  parent?: Lineage | null;
  children: Lineage[];
};

export type LineageAge = {
  teenager?: number | null;
  adult?: number | null;
  mature?: number | null;
  venerable?: number | null;
};

export type LineageFeature = Feature & {
  id: string;
  createdBy: Actor;
  createdOn: string;
  updatedBy: Actor;
  updatedOn: string;
};

export type LineageLanguages = {
  granted: Language[];
  extra: number;
  content?: string | null;
};

export type LineageLanguagesPayload = {
  ids?: string[];
  extra?: number;
  content?: string | null;
};

export type LineageNames = {
  family: string[];
  female: string[];
  male: string[];
  unisex: string[];
  custom: NameCategory[];
  content?: string | null;
};

export type LineageSize = {
  category: SizeCategory;
  height?: string | null;
};

export type LineageSort = "CreatedOn" | "Name" | "UpdatedOn";

export type LineageSortOption = SortOption & {
  field: LineageSort;
};

export type LineageSpeeds = {
  walk?: number | null;
  climb?: number | null;
  swim?: number | null;
  fly?: number | null;
  hover: boolean;
  burrow?: number | null;
};

export type LineageWeight = {
  malnutrition?: string | null;
  skinny?: string | null;
  normal?: string | null;
  overweight?: string | null;
  obese?: string | null;
};

export type NameCategory = {
  category: string;
  values: string[];
};

export type SearchLineagesPayload = SearchPayload & {
  parentId?: string | null;
  sizeCategory?: SizeCategory | null;
  sort: LineageSortOption[];
};

export type UpdateLineagePayload = {
  name?: string | null;
  summary?: Optional<string> | null;
  content?: Optional<string> | null;
  languages?: LineageLanguagesPayload | null;
  names?: LineageNames | null;
  speeds?: LineageSpeeds | null;
  size?: LineageSize | null;
  weight?: LineageWeight | null;
  age?: LineageAge | null;
};
