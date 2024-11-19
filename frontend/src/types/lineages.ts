import type { Aggregate } from "./aggregate";
import type { Attribute, SizeCategory } from "./game";
import type { WorldModel } from "@/types/worlds";
import type { SearchPayload, SortOption } from "./search";
import type { LanguageModel } from "./languages";

export type AgesModel = {
  adolescent?: number;
  adult?: number;
  mature?: number;
  venerable?: number;
};

export type AttributeBonusesModel = {
  agility: number;
  coordination: number;
  intellect: number;
  presence: number;
  sensitivity: number;
  spirit: number;
  vigor: number;
  extra: number;
};

export type CreateOrReplaceLineagePayload = {
  parentId?: string;
  name: string;
  description?: string;
  attributes: AttributeBonusesModel;
  features: TraitPayload[];
  languages: LanguagesPayload;
  names: NamesModel;
  speeds: SpeedsModel;
  size: SizeModel;
  weight: WeightModel;
  ages: AgesModel;
};

export type LanguagesModel = {
  items: LanguageModel[];
  extra: number;
  text?: string;
};

export type LanguagesPayload = {
  ids: string[];
  extra: number;
  text?: string;
};

export type LineageModel = Aggregate & {
  world: WorldModel;
  name: string;
  description?: string;
  attributes: AttributeBonusesModel;
  features: TraitModel[];
  languages: LanguagesModel;
  names: NamesModel;
  speeds: SpeedsModel;
  size: SizeModel;
  weight: WeightModel;
  ages: AgesModel;
  species?: LineageModel;
  nations: LineageModel[];
};

export type LineageSort = "CreatedOn" | "Name" | "UpdatedOn";

export type LineageSortOption = SortOption & {
  field: LineageSort;
};

export type NameCategory = {
  key: string;
  values: string[];
};

export type NamesModel = {
  text?: string;
  family: string[];
  female: string[];
  male: string[];
  unisex: string[];
  custom: NameCategory[];
};

export type SearchLineagesPayload = SearchPayload & {
  attribute?: Attribute;
  languageId?: string;
  parentId?: string;
  sizeCategory?: SizeCategory;
  sort: LineageSortOption[];
};

export type SizeModel = {
  category: SizeCategory;
  roll?: string;
};

export type SpeedsModel = {
  walk: number;
  climb: number;
  swim: number;
  fly: number;
  hover: number;
  burrow: number;
};

export type TraitModel = {
  id: string;
  name: string;
  description?: string;
};

export type TraitPayload = {
  id?: string;
  name: string;
  description?: string;
};

export type TraitStatus = {
  trait: TraitPayload;
  isRemoved: boolean;
  isUpdated: boolean;
};

export type WeightCategory = "Starved" | "Skinny" | "Normal" | "Overweight" | "Obese";

export type WeightModel = {
  starved?: string;
  skinny?: string;
  normal?: string;
  overweight?: string;
  obese?: string;
};
