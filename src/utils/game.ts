import type { Attribute, Skill, SpeedKind, Statistic } from "@/types/game";

export const ATTRIBUTES: Attribute[] = ["Dexterity", "Health", "Intellect", "Senses", "Vigor"];

export const SKILLS: Skill[] = [
  "Acrobatics",
  "Athletics",
  "Crafting",
  "Deception",
  "Diplomacy",
  "Discipline",
  "Insight",
  "Investigation",
  "Knowledge",
  "Linguistics",
  "Medicine",
  "Melee",
  "Occultism",
  "Orientation",
  "Perception",
  "Performance",
  "Resistance",
  "Stealth",
  "Survival",
  "Thievery",
];

export const SPEED_KINDS: SpeedKind[] = ["Walk", "Climb", "Swim", "Fly", "Burrow"];

export const STATISTICS: Statistic[] = ["Dodge", "Initiative", "Learning", "Load", "Power", "Precision", "Stamina", "Stratagem", "Strength", "Vitality"];

export const ATTRIBUTE_CATEGORIES: Record<Attribute, "mental" | "physical" | "universal"> = {
  Dexterity: "physical",
  Health: "universal",
  Intellect: "mental",
  Senses: "mental",
  Vigor: "physical",
};

export const SKILL_ATTRIBUTES: Record<Skill, Attribute | null> = {
  Acrobatics: "Dexterity",
  Athletics: "Vigor",
  Crafting: "Dexterity",
  Deception: null,
  Diplomacy: null,
  Discipline: "Health",
  Insight: "Senses",
  Investigation: "Intellect",
  Knowledge: "Intellect",
  Linguistics: "Intellect",
  Medicine: "Intellect",
  Melee: "Vigor",
  Occultism: "Senses",
  Orientation: "Dexterity",
  Perception: "Senses",
  Performance: null,
  Resistance: "Health",
  Stealth: "Dexterity",
  Survival: "Senses",
  Thievery: "Dexterity",
};

export const STATISTIC_ATTRIBUTES: Record<Statistic, Attribute> = {
  Dodge: "Dexterity",
  Initiative: "Senses",
  Learning: "Intellect",
  Load: "Vigor",
  Power: "Senses",
  Precision: "Dexterity",
  Stamina: "Health",
  Stratagem: "Intellect",
  Strength: "Vigor",
  Vitality: "Health",
};

export function calculateStatistic(statistic: Statistic, level: number, attributes: Record<Uncapitalize<Attribute>, number>): number {
  switch (statistic) {
    case "Dodge":
      return 10 + attributes.dexterity;
    case "Initiative":
      return 2 * attributes.senses;
    case "Learning":
      return Math.floor(Math.max((5 + attributes.intellect + (level / 5) * (2 + attributes.intellect)), (5 + level / 5)));
    case "Load":
      return 10 * (5 + attributes.vigor);
    case "Power":
      return 5 + attributes.senses * 2;
    case "Precision":
      return 5 + attributes.dexterity * 2;
    case "Stamina":
      return Math.floor(((25 + level) * (5 + attributes.health)) / 5);
    case "Stratagem":
      return 5 + attributes.intellect * 2;
    case "Strength":
      return 5 + attributes.vigor * 2;
    case "Vitality":
      return Math.floor(((25 + level) * (5 + attributes.health)) / 5);
  }
}

export function camelCase<T extends string>(value: T): Uncapitalize<T> {
  return `${value.charAt(0).toLowerCase()}${value.slice(1)}` as Uncapitalize<T>;
}

export const SKILLS_BY_ATTRIBUTE: Record<Attribute | "social", Skill[]> = {
  ...(Object.fromEntries(ATTRIBUTES.map((attribute) => [attribute, [] as Skill[]])) as Record<Attribute, Skill[]>),
  social: [],
};
for (const skill of SKILLS) {
  SKILLS_BY_ATTRIBUTE[SKILL_ATTRIBUTES[skill] ?? "social"].push(skill);
}

export const STATISTICS_BY_ATTRIBUTE: Record<Attribute, Statistic[]> = Object.fromEntries(
  ATTRIBUTES.map((attribute) => [attribute, [] as Statistic[]]),
) as Record<Attribute, Statistic[]>;
for (const statistic of STATISTICS) {
  STATISTICS_BY_ATTRIBUTE[STATISTIC_ATTRIBUTES[statistic]].push(statistic);
}
