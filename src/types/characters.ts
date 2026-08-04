import type { Aggregate } from "./api";
import type { SearchPayload, SortOption } from "./search";

export type Character = Aggregate & {
  // TODO(fpion): complete
};

export type CreateCharacterPayload = {
  lineageId: string;
  languageIds: string[];
};

export type SearchCharactersPayload = SearchPayload & {
  // TODO(fpion): complete
  sort: CharacterSortOption[];
};

export type CharacterSort = "CreatedOn" | "UpdatedOn";

export type CharacterSortOption = SortOption & {
  field: CharacterSort;
};
