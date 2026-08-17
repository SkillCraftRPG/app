/**
 * Formats an integer with an explicit sign: `+1`, `0`, `−1`.
 * Uses a typographic minus (`−`) for negatives.
 * @param value The integer to format.
 * @param formatInteger Formats the absolute integer according to the current locale (e.g. `n(value, "integer")`).
 */
export function formatSignedInteger(value: number, formatInteger: (value: number) => string): string {
  const formatted: string = formatInteger(value);
  return value > 0 ? `+${formatted}` : formatted.replace("-", "−");
}
