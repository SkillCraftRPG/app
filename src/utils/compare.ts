import type { Customization } from "@/types/customizations";
import type { Language } from "@/types/languages";

export function compareCustomizations(left: Customization, right: Customization): boolean {
  return left.name === right.name && (left.summary ?? "") === (right.summary ?? "") && (left.content ?? "") === (right.content ?? "");
}

export function compareLanguages(left: Language, right: Language): boolean {
  return (
    left.name === right.name &&
    (left.summary ?? "") === (right.summary ?? "") &&
    (left.content ?? "") === (right.content ?? "") &&
    (left.script?.id ?? "") === (right.script?.id ?? "") &&
    (left.typicalSpeakers ?? "") === (right.typicalSpeakers ?? "")
  );
}
