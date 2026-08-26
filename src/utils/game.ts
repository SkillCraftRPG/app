import type { Attribute, Skill, Statistic } from "@/types/game";
import type { CharacterModifier, CharacterModifierKind, CharacterTalent } from "@/types/characters";
import type { SpeedKind } from "@/types/game";
import { ATTRIBUTE_SKILLS, ATTRIBUTE_STATISTICS, ATTRIBUTES, SKILL_ATTRIBUTE, STATISTIC_ATTRIBUTE, STATISTICS } from "@/types/game";
import { MAXIMUM_LEVEL } from "./experience";
import { SKILLS } from "@/types/game";

/******************************************************************** TODO(FPION): REFACTOR *******************************************************************/

export const SPEED_KINDS: SpeedKind[] = ["Walk", "Climb", "Swim", "Fly", "Burrow"];

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

export function camelCase<T extends string>(value: T): Uncapitalize<T> {
  return `${value.charAt(0).toLowerCase()}${value.slice(1)}` as Uncapitalize<T>;
}
/******************************************************************** TODO(FPION): REFACTOR *******************************************************************/

const MAXIMUM_RANK: number = 14;

function calculateTotalModifier(kind: CharacterModifierKind, target: string, modifiers: CharacterModifier[]): number {
  return modifiers.filter((modifier) => modifier.kind === kind && modifier.target === target).reduce((total, modifier) => total + modifier.value, 0);
}

export function calculateSkill(
  skill: Skill,
  attributes: Map<Attribute, number>,
  talents?: CharacterTalent[],
  rank: number = 0,
  modifiers?: CharacterModifier[],
): number {
  if (rank < 0 || rank > MAXIMUM_RANK) {
    throw new Error(`rank must be comprised between 0 and ${MAXIMUM_RANK}`);
  }

  const attribute: Attribute | "Social" = getSkillAttribute(skill);
  const attributeScore: number = (attribute === "Social" ? undefined : attributes.get(attribute)) ?? 0;

  const talentCount: number = (talents ?? []).filter((acquired) => acquired.talent.skill === skill).length;
  const effectiveRank: number = talentCount ? rank : Math.floor(rank / 2);
  const modifierTotal: number = calculateTotalModifier("Skill", skill, modifiers ?? []);

  return attributeScore + talentCount + effectiveRank + modifierTotal;
}

export function calculateStatisticNew(statistic: Statistic, attributes: Map<Attribute, number>, level: number = 0, modifiers?: CharacterModifier[]): number {
  if (level < 0 || level > MAXIMUM_LEVEL) {
    throw new Error(`level must be comprised between 0 and ${MAXIMUM_LEVEL}`);
  }

  let total: number = 0;
  switch (statistic) {
    case "Dodge":
      total = 10 + (attributes.get("Dexterity") ?? 0);
      break;
    case "Initiative":
      total = 2 * (attributes.get("Senses") ?? 0);
      break;
    case "Learning":
      const intellect: number = attributes.get("Intellect") ?? 0;
      total = Math.floor(Math.max(5 + intellect + (level / 5) * (2 + intellect), 5 + level / 5));
      break;
    case "Load":
      total = 10 * (5 + (attributes.get("Vigor") ?? 0));
      break;
    case "Power":
      total = 5 + 2 * (attributes.get("Senses") ?? 0);
      break;
    case "Precision":
      total = 5 + 2 * (attributes.get("Dexterity") ?? 0);
      break;
    case "Stamina":
      total = Math.floor(((25 + level) * (5 + (attributes.get("Health") ?? 0))) / 5);
      break;
    case "Stratagem":
      total = 5 + 2 * (attributes.get("Intellect") ?? 0);
      break;
    case "Strength":
      total = 5 + 2 * (attributes.get("Vigor") ?? 0);
      break;
    case "Vitality":
      total = Math.floor(((25 + level) * (5 + (attributes.get("Health") ?? 0))) / 5);
      break;
  }

  total += calculateTotalModifier("Statistic", statistic, modifiers ?? []);

  switch (statistic) {
    case "Learning":
    case "Load":
      return Math.max(total, 0);
    case "Stamina":
    case "Vitality":
      return Math.max(total, 1);
    default:
      return total;
  }
} // TODO(fpion): rename

export function getAttributeSkills(attribute: Attribute | "Social"): Skill[] {
  const skills: Skill[] | undefined = ATTRIBUTE_SKILLS.get(attribute);
  if (!skills) {
    throw new Error(`The attribute "${attribute}" did not yield any skill.`);
  }
  return skills;
}

export function getAttributeStatistics(attribute: Attribute): Statistic[] {
  const statistics: Statistic[] | undefined = ATTRIBUTE_STATISTICS.get(attribute);
  if (!statistics) {
    throw new Error(`The attribute "${attribute}" did not yield any statistic.`);
  }
  return statistics;
}

export function getSkillAttribute(skill: Skill): Attribute | "Social" {
  const attribute: Attribute | "Social" | undefined = SKILL_ATTRIBUTE.get(skill);
  if (!attribute) {
    throw new Error(`The skill "${skill}" did not yield any attribute.`);
  }
  return attribute;
}

export function getStatisticAttribute(statistic: Statistic): Attribute {
  const attribute: Attribute | undefined = STATISTIC_ATTRIBUTE.get(statistic);
  if (!attribute) {
    throw new Error(`The statistic "${statistic}" did not yield any attribute.`);
  }
  return attribute;
}
