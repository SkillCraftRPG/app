import type { RuleExecutionOutcome, ValidationRule } from "logitar-validation";
import { parsingUtils } from "logitar-js";

const { parseNumber } = parsingUtils;

const roll: ValidationRule = (value: unknown): RuleExecutionOutcome => {
  if (typeof value !== "string") {
    return { severity: "error", message: "{{name}} must be a string." };
  }

  if (value.length > 0) {
    const parts: string[] = value.split("+");
    if (parts.length > 2) {
      return { severity: "error", message: "The number of parts must not exceed 2." };
    } else if (parts.length === 2) {
      const base: number = parseNumber(parts[0]) ?? 0;
      if (base < 1 || base > 999) {
        return { severity: "error", message: "The base must range from 1 to 999." };
      }
    }

    const roll: string[] = (parts[1] ?? "").split("d");
    if (roll.length !== 2) {
      return { severity: "error", message: "The roll part is not valid." };
    }

    const dice: number = parseNumber(roll[0]) ?? 0;
    if (dice < 1 || dice > 99) {
      return { severity: "error", message: "The number of die must range from 1 to 99." };
    }

    const die: number = parseNumber(roll[0]) ?? 0;
    if (die < 1 || die > 999) {
      return { severity: "error", message: "The number of sides must range from 1 to 999." };
    }
  }

  return { severity: "information" };
};

export default roll;
