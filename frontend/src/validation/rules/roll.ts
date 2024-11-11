import { stringUtils } from "logitar-js";

const { isDigit } = stringUtils;

function isNumber(s: string): boolean {
  return [...s].every(isDigit);
}

function isRoll(s: string): boolean {
  const parts: string[] = s.split("d");
  return parts.length === 2 && parts.every((part) => part.length > 0 && isNumber(part));
}

export default function (s?: string): boolean {
  return typeof s === "string" && (s.length === 0 || s.split("+").every((part) => part.length > 0 && (isNumber(part) || isRoll(part))));
}
