import type { CharacterTalentDiscount } from "@/types/characters";
import type { Talent } from "@/types/talents";

export function calculateCost(talent: Talent, discounts?: CharacterTalentDiscount[]): number {
  const discount: number = discounts?.reduce((sum, discount) => sum + discount.amount, 0) ?? 0;
  const cost: number = talent.cost - discount;
  return Math.max(cost, 0);
}

export function formatTalentName(talent: Talent, qualifier?: string | null): string {
  return qualifier ? `${talent.name} (${qualifier})` : talent.name;
}
