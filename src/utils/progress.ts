import type { ProgressData } from "@/types/progress";

export function calculateProgress(ratio: number, formatPercentage: (value: number, format: string) => string): ProgressData {
  if (ratio < 0 || ratio > 1) {
    throw new Error("ratio must be comprised between 0 and 1");
  }
  return {
    label: formatPercentage(ratio, "percentage"),
    value: Math.floor(ratio * 100),
  };
}
