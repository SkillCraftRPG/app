import type { Customization } from "@/types/customizations";
import type { Language } from "@/types/languages";
import type { Script } from "@/types/scripts";

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

export function compareScripts(left: Script, right: Script): boolean {
  return left.name === right.name && (left.summary ?? "") === (right.summary ?? "") && (left.content ?? "") === (right.content ?? "");
}
