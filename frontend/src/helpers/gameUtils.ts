import { parsingUtils } from "logitar-js";

const { parseNumber } = parsingUtils;

export function calculateModifier(score: number): number {
  return Math.floor(score / 2.0) - 5;
}

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
