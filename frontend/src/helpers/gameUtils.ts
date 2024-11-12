import { parsingUtils } from "logitar-js";

const { parseNumber } = parsingUtils;

export function roll(roll: string): number {
  let value: number = 0;
  roll
    .toLowerCase()
    .split("+")
    .forEach((part) => {
      const values: string[] = part.split("d");
      if (values.length === 2) {
        for (let i = 0; i < (parseNumber(values[0]) ?? 0); i++) {
          value += Math.floor(Math.random() * (parseNumber(values[1]) ?? 0));
        }
      } else {
        value += parseNumber(values[0]) ?? 0;
      }
    });
  return value;
} // TODO(fpion): tests
