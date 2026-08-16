import type { Actor, Aggregate, Optional } from "./api";
import type { Caste } from "./castes";
import type { Customization } from "./customizations";
import type { Education } from "./educations";
import type { Item } from "./items";
import type { Language } from "./languages";
import type { Lineage } from "./lineages";
import type { SearchPayload, SortOption } from "./search";
import type { Skill } from "./game";
import type { Talent } from "./talents";

export type AddCharacterTalentPayload = CharacterTalentPayload & {
  talentId: string;
};

export type Alignment =
  | "TrueNeutral"
  | "ChaoticEvil"
  | "ChaoticGood"
  | "ChaoticNeutral"
  | "LawfulEvil"
  | "LawfulGood"
  | "LawfulNeutral"
  | "NeutralEvil"
  | "NeutralGood";

export type Character = Aggregate & {
  name: string;
  dominantHand?: DominantHand | null;
  tier: number;
  level: number;
  experience: number;
  lineage: Lineage;
  caste: Caste;
  education: Education;
  appearance: CharacterAppearance;
  alignment?: Alignment | null;
  personality: CharacterPersonality;
  background?: string | null;
  attributes: CharacterAttributes;
  statistics: CharacterStatistics;
  skills: CharacterSkills;
  speeds: CharacterSpeeds;
  vitality: number;
  stamina: number;
  bloodAlcoholContent: number;
  intoxication: number;
  hope: number;
  customizations: Customization[];
  languages: CharacterLanguage[];
  talents: CharacterTalent[];
  points: CharacterPoints;
  /* TODO(fpion): complete this
   * Player
   * Picture
   * Bonuses
   * Inventory & Load
   * Attacks & Defense
   * Notes
   * Conditions
   * Specializations
   * Spells
   */
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

export type CharacterAttribute = {
  starting: number;
  progression: number;
  bonus: number;
  total: number;
};

export type CharacterAttributes = {
  dexterity: CharacterAttribute;
  health: CharacterAttribute;
  intellect: CharacterAttribute;
  senses: CharacterAttribute;
  vigor: CharacterAttribute;
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
  alignment?: Alignment | null;
  personality: CharacterPersonality;
  background: string;
  currency?: Item;
  quantity: number;
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
  Background = 8,
  Equipment = 9,
}

export type CharacterLanguage = {
  language: Language;
  source: CharacterLanguageSource;
  target?: string | null;
  notes?: string | null;
  createdBy: Actor;
  createdOn: string;
  updatedBy: Actor;
  updatedOn: string;
};

export type CharacterLanguageSource = "Custom" | "Extra" | "Customization" | "Talent";

export type CharacterPersonality = {
  traits?: string | null;
  ideals?: string | null;
  flaws?: string | null;
};

export type CharacterPoints = {
  attributes: number;
  skills: number;
  talents: number;
};

export type CharacterSkill = {
  rank: number;
  talents: number;
  attribute: number;
  bonus: number;
  total: number;
};

export type CharacterSkills = {
  acrobatics: CharacterSkill;
  athletics: CharacterSkill;
  crafting: CharacterSkill;
  deception: CharacterSkill;
  diplomacy: CharacterSkill;
  discipline: CharacterSkill;
  insight: CharacterSkill;
  investigation: CharacterSkill;
  knowledge: CharacterSkill;
  linguistics: CharacterSkill;
  medicine: CharacterSkill;
  melee: CharacterSkill;
  occultism: CharacterSkill;
  orientation: CharacterSkill;
  perception: CharacterSkill;
  performance: CharacterSkill;
  resistance: CharacterSkill;
  stealth: CharacterSkill;
  survival: CharacterSkill;
  thievery: CharacterSkill;
};

export type CharacterSort = "CreatedOn" | "UpdatedOn";

export type CharacterSortOption = SortOption & {
  field: CharacterSort;
};

export type CharacterSpeed = {
  lineage: number;
  bonus: number;
  encumbrance: number;
  total: number;
};

export type CharacterSpeeds = {
  walk: CharacterSpeed;
  climb: CharacterSpeed;
  swim: CharacterSpeed;
  fly: CharacterSpeed;
  hover: boolean;
  burrow: CharacterSpeed;
};

export type CharacterStatistic = {
  base: number;
  bonus: number;
  total: number;
};

export type CharacterStatistics = {
  dodge: CharacterStatistic;
  initiative: CharacterStatistic;
  learning: CharacterStatistic;
  load: CharacterStatistic;
  power: CharacterStatistic;
  precision: CharacterStatistic;
  stamina: CharacterStatistic;
  stratagem: CharacterStatistic;
  strength: CharacterStatistic;
  vitality: CharacterStatistic;
};

export type CharacterTalent = {
  id: string;
  talent: Talent;
  qualifier?: string | null;
  notes?: string | null;
  discounts: CharacterTalentDiscount[];
  cost: number;
  createdBy: Actor;
  createdOn: string;
  updatedBy: Actor;
  updatedOn: string;
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
  alignment?: Alignment | null;
  personality: CharacterPersonality;
  background?: string | null;
  startingWealth?: StartingWealth | null;
};

export type DominantHand = "Left" | "Right";

export type SearchCharactersPayload = SearchPayload & {
  lineageId?: string | null;
  casteId?: string | null;
  educationId?: string | null;
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

export type StartingWealth = {
  currencyId: string;
  quantity: number;
};

export type UpdateCharacterPayload = {
  name?: string;
  dominantHand?: Optional<DominantHand> | null;
  appearance?: CharacterAppearance | null;
  alignment?: Optional<Alignment> | null;
  personality?: CharacterPersonality | null;
  background?: Optional<string> | null;
};
