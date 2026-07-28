import { stringUtils } from "logitar-js";

const { isLetterOrDigit } = stringUtils;

/**
 * Characters that do not decompose under NFD but should map to ASCII for slugs/search.
 */
const ligatures = new Map<string, string>([
  ["æ", "ae"],
  ["Æ", "AE"],
  ["œ", "oe"],
  ["Œ", "OE"],
  ["ß", "ss"],
  ["ø", "o"],
  ["Ø", "O"],
  ["ł", "l"],
  ["Ł", "L"],
  ["đ", "d"],
  ["Đ", "D"],
]);

/**
 * Formats the specified string into a slug. Slugs are composed of non-empty lowercase words separated by hyphens (`-`).
 * @param value The string to slugify.
 * @returns The formatted slug string.
 */
export function slugify(value: string): string {
  const words: string[] = [];
  let word = "";

  for (const char of unaccent(value)) {
    if (isLetterOrDigit(char)) {
      word += char;
    } else if (word.length > 0) {
      words.push(word);
      word = "";
    }
  }

  if (word.length > 0) {
    words.push(word);
  }

  return words.join("-").toLowerCase();
}

/**
 * Replaces accented and special Latin characters with their ASCII equivalents.
 * For example, `Éléphant` becomes `Elephant` and `œuvre` becomes `oeuvre`.
 * @param value The string to remove accents from.
 * @returns The string without accents.
 */
export function unaccent(value: string): string {
  let result = value.normalize("NFD").replace(/\p{M}/gu, "");

  for (const [from, to] of ligatures) {
    result = result.replaceAll(from, to);
  }

  return result;
}
