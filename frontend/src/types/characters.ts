import type { Aggregate } from "./aggregate";
import type { AspectModel } from "./aspects";
import type { Attribute } from "./game";
import type { LineageModel } from "./lineages";
import type { PersonalityModel } from "./personalities";
import type { SearchPayload, SortOption } from "./search";
import type { WorldModel } from "./worlds";

export type BaseAttributesPayload = {
  agility: number;
  coordination: number;
  intellect: number;
  presence: number;
  sensitivity: number;
  spirit: number;
  vigor: number;
  best: Attribute;
  worst: Attribute;
  optional: Attribute[];
  extra: Attribute[];
};

export type CharacterCreation = {
  step1?: Step1;
  step2?: Step2;
  step3?: Step3;
  step4?: Step4;
  step5?: Step5;
  step6?: Step6;
};

export type CharacterModel = Aggregate & {
  world: WorldModel;
  name: string;
  playerName?: string;
  lineage: LineageModel;
  height: number; // TODO(fpion): optional
  weight: number; // TODO(fpion): optional
  age: number; // TODO(fpion): optional
  // TODO(fpion): complete
};

export type CharacterSort = "CreatedOn" | "Name" | "UpdatedOn";

export type CharacterSortOption = SortOption & {
  field: CharacterSort;
};

export type CreateCharacterPayload = {
  name: string;
  player?: string;
  lineageId: string;
  height: number; // TODO(fpion): optional
  weight: number; // TODO(fpion): optional
  age: number; // TODO(fpion): optional
  languageIds: string[]; // TODO(fpion): optional
  personalityId: string;
  customizationIds: string[];
  aspectIds: string[];
  attributes: BaseAttributesPayload;
  casteId: string;
  educationId: string;
  talentIds: string[];
  startingWealth?: StartingWealthPayload;
};

export type SearchCharactersPayload = SearchPayload & {
  sort: CharacterSortOption[];
};

export type StartingWealthPayload = {
  itemId: string;
  quantity: number;
};

export type Step1 = {
  player?: string;
  species?: LineageModel;
  nation?: LineageModel;
};

export type Step2 = {
  personality?: PersonalityModel;
};

export type Step3 = {
  aspects: AspectModel[];
};

export type Step4 = {
  attributes: BaseAttributesPayload;
};

export type Step5 = {
  casteId: string;
  educationId: string;
  startingWealth?: StartingWealthPayload;
};

export type Step6 = {
  talentIds: string[];
};
