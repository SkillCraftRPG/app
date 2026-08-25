export type Attribute = "Dexterity" | "Health" | "Intellect" | "Senses" | "Vigor";

export const ATTRIBUTES: Attribute[] = ["Dexterity", "Health", "Intellect", "Senses", "Vigor"];

export const ATTRIBUTE_SKILLS: Map<Attribute, Skill[]> = new Map([
  ["Dexterity", ["Acrobatics", "Crafting", "Orientation", "Stealth", "Thievery"]],
  ["Health", ["Discipline", "Resistance"]],
  ["Intellect", ["Investigation", "Knowledge", "Linguistics", "Medicine"]],
  ["Senses", ["Insight", "Occultism", "Perception", "Survival"]],
  ["Vigor", ["Athletics", "Melee"]],
]);

export const ATTRIBUTE_STATISTICS: Map<Attribute, Statistic[]> = new Map([
  ["Dexterity", ["Dodge", "Precision"]],
  ["Health", ["Stamina", "Vitality"]],
  ["Intellect", ["Learning", "Stratagem"]],
  ["Senses", ["Initiative", "Power"]],
  ["Vigor", ["Load", "Strength"]],
]);

export type SizeCategory = "Diminutive" | "Tiny" | "Small" | "Medium" | "Large" | "Huge" | "Gargantuan" | "Colossal";

export type Skill =
  | "Acrobatics"
  | "Athletics"
  | "Crafting"
  | "Deception"
  | "Diplomacy"
  | "Discipline"
  | "Insight"
  | "Investigation"
  | "Knowledge"
  | "Linguistics"
  | "Medicine"
  | "Melee"
  | "Occultism"
  | "Orientation"
  | "Perception"
  | "Performance"
  | "Resistance"
  | "Stealth"
  | "Survival"
  | "Thievery";

export const SKILL_ATTRIBUTE: Map<Skill, Attribute> = new Map([
  ["Acrobatics", "Dexterity"],
  ["Athletics", "Vigor"],
  ["Crafting", "Dexterity"],
  ["Discipline", "Health"],
  ["Insight", "Senses"],
  ["Investigation", "Intellect"],
  ["Knowledge", "Intellect"],
  ["Linguistics", "Intellect"],
  ["Medicine", "Intellect"],
  ["Melee", "Vigor"],
  ["Occultism", "Senses"],
  ["Orientation", "Dexterity"],
  ["Perception", "Senses"],
  ["Resistance", "Health"],
  ["Stealth", "Dexterity"],
  ["Survival", "Senses"],
  ["Thievery", "Dexterity"],
]);

export type SpeedKind = "Burrow" | "Climb" | "Fly" | "Swim" | "Walk";

export type Statistic = "Dodge" | "Initiative" | "Learning" | "Load" | "Power" | "Precision" | "Stamina" | "Stratagem" | "Strength" | "Vitality";

export const STATISTICS: Statistic[] = ["Dodge", "Initiative", "Learning", "Load", "Power", "Precision", "Stamina", "Stratagem", "Strength", "Vitality"];

export const STATISTIC_ATTRIBUTE: Map<Statistic, Attribute> = new Map([
  ["Dodge", "Dexterity"],
  ["Initiative", "Senses"],
  ["Learning", "Intellect"],
  ["Load", "Vigor"],
  ["Power", "Senses"],
  ["Precision", "Dexterity"],
  ["Stamina", "Health"],
  ["Stratagem", "Intellect"],
  ["Strength", "Vigor"],
  ["Vitality", "Health"],
]);
