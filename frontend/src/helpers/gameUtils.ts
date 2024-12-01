import type { CharacterModel, TalentPoints } from "@/types/characters";
import { parsingUtils } from "logitar-js";

const { parseNumber } = parsingUtils;

export function calculateModifier(score: number): number {
  return Math.floor(score / 2.0) - 5;
}

export function calculateTalentPoints(character: CharacterModel): TalentPoints {
  const talentPoints: TalentPoints = {
    available: 8 + character.level * 4,
    spent: character.talents.reduce((sum, talent) => sum + talent.cost, 0),
    remaining: 0,
  };
  talentPoints.remaining = talentPoints.available - talentPoints.spent;
  return talentPoints;
} // TODO(fpion): unit tests

const REQUIRED_EXPERIENCE: number[] = [
  100, 300, 700, 1300, 2100, 3100, 4300, 5700, 7300, 9100, 11100, 13300, 15700, 18300, 21100, 24100, 27300, 30700, 34300, 38100,
];
export function getTotalExperience(level: number): number {
  if (level < 0 || level > 20) {
    throw new Error("The level must be comprised between 0 and 20. The boundaries are inclusive.");
  }
  let total: number = 0;
  for (let i = 0; i < level; i++) {
    total += REQUIRED_EXPERIENCE[i];
  }
  return total;
} // TODO(fpion): unit tests

export function roll(roll: string): number {
  let value: number = 0;
  roll
    .toLowerCase()
    .split("+")
    .forEach((part) => {
      const values: string[] = part.split("d");
      if (values.length === 2) {
        for (let i = 0; i < (parseNumber(values[0]) ?? 0); i++) {
          value += Math.floor(Math.random() * (parseNumber(values[1]) ?? 0)) + 1;
        }
      } else {
        value += parseNumber(values[0]) ?? 0;
      }
    });
  return value;
}
