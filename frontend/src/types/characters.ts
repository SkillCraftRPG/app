import type { Aggregate } from "./aggregate";
import type { AspectModel } from "./aspects";
import type { Attribute, Skill } from "./game";
import type { CasteModel } from "./castes";
import type { CustomizationModel } from "./customizations";
import type { EducationModel } from "./educations";
import type { ItemModel } from "./items";
import type { LanguageModel } from "./languages";
import type { LineageModel } from "./lineages";
import type { NatureModel } from "./natures";
import type { SearchPayload, SortOption } from "./search";
import type { TalentModel } from "./talents";
import type { WorldModel } from "./worlds";

export type BaseAttributesModel = {
  agility: number;
  coordination: number;
  intellect: number;
  presence: number;
  sensitivity: number;
  spirit: number;
  vigor: number;
  best: Attribute;
  worst: Attribute;
  mandatory: Attribute[];
  optional: Attribute[];
  extra: Attribute[];
};

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

export type BonusCategory = "Attribute" | "Miscellaneous" | "Skill" | "Speed" | "Statistic";

export type BonusPayload = {
  category: BonusCategory;
  target: string;
  value: number;
  isTemporary: boolean;
  precision?: string;
  notes?: string;
};

export type BonusModel = {
  id: string;
  category: BonusCategory;
  target: string;
  value: number;
  isTemporary: boolean;
  precision?: string;
  notes?: string;
};

export type CharacterCreation = {
  step1?: Step1;
  step2?: Step2;
  step3?: Step3;
  step4?: Step4;
  step5?: Step5;
  step6?: Step6;
};

export type CharacterLanguageModel = {
  language: LanguageModel;
  notes?: string;
};

export type CharacterLanguagePayload = {
  notes?: string;
};

export type CharacterModel = Aggregate & {
  world: WorldModel;
  name: string;
  playerName?: string;
  experience: number;
  level: number;
  tier: number;
  vitality: number;
  stamina: number;
  bloodAlcoholContent: number;
  intoxication: number;
  bonuses: BonusModel[];
  lineage: LineageModel;
  height: number;
  weight: number;
  age: number;
  languages: CharacterLanguageModel[];
  nature: NatureModel;
  customizations: CustomizationModel[];
  aspects: AspectModel[];
  baseAttributes: BaseAttributesModel;
  caste: CasteModel;
  education: EducationModel;
  talents: CharacterTalentModel[];
  inventory: InventoryModel;
};

export type CharacterSort = "CreatedOn" | "Name" | "UpdatedOn";

export type CharacterSortOption = SortOption & {
  field: CharacterSort;
};

export type CharacterTalentModel = {
  id: string;
  talent: TalentModel;
  cost: number;
  precision?: string;
  notes?: string;
};

export type CreateCharacterPayload = {
  name: string;
  player?: string;
  lineageId: string;
  height: number;
  weight: number;
  age: number;
  languageIds: string[];
  natureId: string;
  customizationIds: string[];
  aspectIds: string[];
  attributes: BaseAttributesPayload;
  casteId: string;
  educationId: string;
  talentIds: string[];
  startingWealth?: StartingWealthPayload;
};

export type InventoryModel = {
  id: string;
  item: ItemModel;
  containingItemId?: string;
  quantity: number;
  isAttuned?: boolean;
  isEquipped: boolean;
  isIdentified: boolean;
  isProficient?: boolean;
  skill?: Skill;
  remainingCharges?: number;
  remainingResistance?: number;
  nameOverride?: string;
  descriptionOverride?: string;
  valueOverride?: number;
};

export type MiscellaneousBonusTarget = "BloodAlcoholContent" | "Defense" | "Dodge" | "Intoxication" | "Stamina" | "Vitality";

export type ReplaceCharacterPayload = {
  name: string;
  player?: string;
  height: number;
  weight: number;
  age: number;
  experience: number;
  vitality: number;
  stamina: number;
  bloodAlcoholContent: number;
  intoxication: number;
};

export type SearchCharactersPayload = SearchPayload & {
  playerName?: string;
  sort: CharacterSortOption[];
};

export type StartingWealthPayload = {
  itemId: string;
  quantity: number;
};

export type Step1 = {
  name: string;
  player?: string;
  species: LineageModel;
  nation?: LineageModel;
  height: number;
  weight: number;
  age: number;
  languages: LanguageModel[];
};

export type Step2 = {
  nature: NatureModel;
  customizations: CustomizationModel[];
};

export type Step3 = {
  aspects: AspectModel[];
};

export type Step4 = {
  attributes: BaseAttributesPayload;
};

export type Step5 = {
  caste: CasteModel;
  education: EducationModel;
  item?: ItemModel;
  quantity: number;
};

export type Step6 = {
  talents: TalentModel[];
};
