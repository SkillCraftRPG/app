import type { CharacterTalent, CharacterTalentDiscount } from "@/types/characters";
import type { Talent } from "@/types/talents";

export function calculateCost(talent: Talent, discounts?: CharacterTalentDiscount[]): number {
  const base: number = 2 + talent.tier;
  const discount: number = discounts?.reduce((sum, discount) => sum + discount.amount, 0) ?? 0;
  const cost: number = base - discount;
  return Math.max(cost, 0);
}
