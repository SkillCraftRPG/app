import type { AttunementOption, Item } from "@/types/items";

export function getAttunementOption(item: Item): AttunementOption {
  if (!item.magic?.attunement) {
    return "none";
  }
  return item.magic.attunement.isRequired ? "required" : "optional";
}
