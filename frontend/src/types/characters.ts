import type { Aggregate } from "./aggregate";
import type { LineageModel } from "./lineages";
import type { SearchPayload, SortOption } from "./search";
import type { WorldModel } from "./worlds";

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
  // TODO(fpion): complete
};

export type SearchCharactersPayload = SearchPayload & {
  sort: CharacterSortOption[];
};
