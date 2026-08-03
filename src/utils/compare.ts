import type { Customization } from "@/types/customizations";

export function compareCustomizations(left: Customization, right: Customization): boolean {
  return left.name === right.name && (left.summary ?? "") === (right.summary ?? "") && (left.content ?? "") === (right.content ?? "");
}
