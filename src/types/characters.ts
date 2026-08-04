import type { Aggregate } from "./api";
import type { Caste } from "./castes";
import type { Customization } from "./customizations";
import type { Education } from "./educations";
import type { Language } from "./languages";
import type { Lineage } from "./lineages";
import type { SearchPayload, SortOption } from "./search";

export type Character = Aggregate & {
  // TODO(fpion): complete
};

export type CharacterCreation = {
  species?: Lineage;
  ethnicity?: Lineage;
  languages: Language[];
  name: string;
  dominantHand?: DominantHand | null;
  customizations: Customization[];
  caste?: Caste;
  education?: Education;
};

export enum CharacterCreationStep {
  Ascendancy = 0,
  Customization = 1,
  Context = 2,
  Talents = 3,
}

export type CreateCharacterPayload = {
  lineageId: string;
  languageIds: string[];
  name: string;
  dominantHand?: DominantHand | null;
  customizationIds: string[];
  casteId: string;
  educationId: string;
};

export type DominantHand = "Left" | "Right";

export type SearchCharactersPayload = SearchPayload & {
  // TODO(fpion): complete
  sort: CharacterSortOption[];
};

export type CharacterSort = "CreatedOn" | "UpdatedOn";

export type CharacterSortOption = SortOption & {
  field: CharacterSort;
};

/* TODO(fpion): languages can be granted by:
 * lineage.languages.extra (species+ethnicity)
 * a customization (e.g. Polyglotte grants 2+Tier languages)
 * talents (e.g. Langue supplémentaire, Linguistique, Philologie, Synergie alphabétique, Interprète all grant 1 language)
 * custom
 */

/* TODO(fpion): talent rebates/gratuity can be granted by:
 * lineage (species+ethnicity)
 * customization (e.g.: Talentueux)
 * specializations
 * custom
 */
