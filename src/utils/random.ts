import { parsingUtils } from "logitar-js";

const { parseNumber } = parsingUtils;

export function roll(roll: string): number {
  let result: number = 0;

  const parts: string[] = roll.split("+");
  if (parts.length === 2) {
    result += parseNumber(parts[0]) ?? 0;
  }

  const values: string[] = (parts.length === 1 ? parts[0] : parts[1])?.toLowerCase().split("d") ?? [];
  if (values.length === 2) {
    const die: number = parseNumber(values[0]) ?? 0;
    const dice: number = parseNumber(values[1]) ?? 0;
    for (let i = 0; i < die; i++) {
      result += 1 + Math.floor(Math.random() * dice);
    }
  }

  return result;
}
