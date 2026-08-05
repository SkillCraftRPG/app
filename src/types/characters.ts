import type { Aggregate } from "./api";
import type { Caste } from "./castes";
import type { Customization } from "./customizations";
import type { Education } from "./educations";
import type { Language } from "./languages";
import type { Lineage } from "./lineages";
import type { SearchPayload, SortOption } from "./search";
import type { Talent } from "./talents";

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

export type CharacterTalent = {
  id: string;
  talent: Talent;
  qualifier?: string | null;
  notes?: string | null;
  discounts: CharacterTalentDiscount[];
};

export type CharacterTalentContext = {
  tier: number;
  lineage: Lineage;
  customizations: Customization[];
  talents: CharacterTalent[];
};

export type CharacterTalentDetail = {
  qualifier: string;
  notes: string;
  discounts: CharacterTalentDiscount[];
};

export type CharacterTalentDiscount = {
  source: CharacterTalentDiscountSource;
  target: string;
  amount: number;
};

export type CharacterTalentDiscountSource = "Custom" | "Customization" | "Lineage"; // TODO(fpion): Specialization

/* TODO(fpion): languages can be granted by:
 * lineage.languages.extra (species+ethnicity)
 * a customization (e.g. Polyglotte grants 2+Tier languages)
 * talents (e.g. Langue supplémentaire, Linguistique, Philologie, Synergie alphabétique, Interprète all grant 1 language)
 * custom
 */
