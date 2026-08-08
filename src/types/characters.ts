import type { Aggregate } from "./api";
import type { Caste } from "./castes";
import type { Customization } from "./customizations";
import type { Education } from "./educations";
import type { Skill } from "./game";
import type { Language } from "./languages";
import type { Lineage } from "./lineages";
import type { SearchPayload, SortOption } from "./search";
import type { Talent } from "./talents";

export type AddCharacterTalentPayload = CharacterTalentPayload & {
  talentId: string;
};

export type Character = Aggregate & {
  // TODO(fpion): complete
};

export type CharacterAppearance = {
  height?: number | null;
  weight?: number | null;
  age?: number | null;
  skin?: string | null;
  eyes?: string | null;
  hair?: string | null;
};

export type CharacterAppearanceDetail = {
  height: number;
  weightCategory: string;
  bodyMassIndex: number;
  age: number;
  skin: string;
  eyes: string;
  hair: string;
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
  talents: CharacterTalent[];
  attributes: StartingAttributes;
  skills: SkillRankPayload[];
  appearance: CharacterAppearanceDetail;
};

export enum CharacterCreationStep {
  Ascendancy = 0,
  Customization = 1,
  Context = 2,
  Talents = 3,
  Attributes = 4,
  Skills = 5,
  Appearance = 6,
  Personality = 7,
}

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

export type CharacterTalentPayload = {
  qualifier?: string | null;
  notes?: string | null;
  discounts: CharacterTalentDiscount[];
};

export type CreateCharacterPayload = {
  lineageId: string;
  languageIds: string[];
  name: string;
  dominantHand?: DominantHand | null;
  customizationIds: string[];
  casteId: string;
  educationId: string;
  talents: AddCharacterTalentPayload[];
  attributes: StartingAttributes;
  skills: SkillRankPayload[];
  appearance: CharacterAppearance;
};

export type DominantHand = "Left" | "Right";

export type SearchCharactersPayload = SearchPayload & {
  // TODO(fpion): complete
  sort: CharacterSortOption[];
};

export type SkillRankPayload = {
  skill: Skill;
  rank: number;
};

export type StartingAttributes = {
  dexterity: number;
  health: number;
  intellect: number;
  senses: number;
  vigor: number;
};

/* TODO(fpion): languages can be granted by:
 * lineage.languages.extra (species+ethnicity)
 * a customization (e.g. Polyglotte grants 2+Tier languages)
 * talents (e.g. Langue supplémentaire, Linguistique, Philologie, Synergie alphabétique, Interprète all grant 1 language)
 * custom
 */
