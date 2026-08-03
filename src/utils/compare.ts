import type { Caste } from "@/types/castes";
import type { Customization } from "@/types/customizations";
import type { Education } from "@/types/educations";
import type { Language } from "@/types/languages";
import type { Script } from "@/types/scripts";
import type { Talent } from "@/types/talents";

export function compareCastes(left: Caste, right: Caste): boolean {
  return (
    left.name === right.name &&
    (left.summary ?? "") === (right.summary ?? "") &&
    (left.content ?? "") === (right.content ?? "") &&
    (left.skill ?? "") === (right.skill ?? "") &&
    (left.wealthRoll ?? "") === (right.wealthRoll ?? "") &&
    (left.feature?.name ?? "") === (right.feature?.name ?? "") &&
    (left.feature?.content ?? "") === (right.feature?.content ?? "")
  );
}

export function compareCustomizations(left: Customization, right: Customization): boolean {
  return left.name === right.name && (left.summary ?? "") === (right.summary ?? "") && (left.content ?? "") === (right.content ?? "");
}

export function compareEducations(left: Education, right: Education): boolean {
  return (
    left.name === right.name &&
    (left.summary ?? "") === (right.summary ?? "") &&
    (left.content ?? "") === (right.content ?? "") &&
    (left.skill ?? "") === (right.skill ?? "") &&
    (left.wealthMultiplier ?? 0) === (right.wealthMultiplier ?? 0) &&
    (left.feature?.name ?? "") === (right.feature?.name ?? "") &&
    (left.feature?.content ?? "") === (right.feature?.content ?? "")
  );
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

export function compareTalents(left: Talent, right: Talent): boolean {
  return (
    left.tier === right.tier &&
    left.name === right.name &&
    (left.summary ?? "") === (right.summary ?? "") &&
    (left.content ?? "") === (right.content ?? "") &&
    left.allowMultiplePurchases === right.allowMultiplePurchases &&
    (left.skill ?? "") === (right.skill ?? "") &&
    (left.requiredTalent?.id ?? "") === (right.requiredTalent?.id ?? "")
  );
}
