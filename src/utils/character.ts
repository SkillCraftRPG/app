import type { Attribute, Skill, Statistic } from "@/types/game";
import type { Character, CharacterAttribute, CharacterSkill, CharacterStatistic } from "@/types/characters";

function capitalize(s: string): string {
  return s.substring(0, 1).toUpperCase() + s.substring(1);
}

export function calculateAttributePoints(level: number): number {
  return Math.floor((level + 5) / 10);
}

export function calculateWeight(heightCentimeters: number, bodyMassIndex: number): number {
  return (heightCentimeters / 100) * (heightCentimeters / 100) * bodyMassIndex;
}

export function getAttributeMap(character: Character): Map<Attribute, CharacterAttribute> {
  const map: Map<Attribute, CharacterAttribute> = new Map();
  for (const [key, value] of Object.entries(character.attributes)) {
    const attribute = capitalize(key) as Attribute;
    map.set(attribute, value);
  }
  return map;
}

export function getAttributeTotals(character: Character): Map<Attribute, number> {
  const map: Map<Attribute, number> = new Map();
  for (const [key, value] of Object.entries(character.attributes)) {
    const attribute = capitalize(key) as Attribute;
    map.set(attribute, value.total);
  }
  return map;
}

export function getSkillMap(character: Character): Map<Skill, CharacterSkill> {
  const map: Map<Skill, CharacterSkill> = new Map();
  for (const [key, value] of Object.entries(character.skills)) {
    const skill = capitalize(key) as Skill;
    map.set(skill, value);
  }
  return map;
}

export function getStatisticMap(character: Character): Map<Statistic, CharacterStatistic> {
  const map: Map<Statistic, CharacterStatistic> = new Map();
  for (const [key, value] of Object.entries(character.statistics)) {
    const statistic = capitalize(key) as Statistic;
    map.set(statistic, value);
  }
  return map;
}
