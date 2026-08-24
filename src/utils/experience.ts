export const MAXIMUM_LEVEL: number = 100;

const THRESHOLDS: number[] = [];
for (let level = 1; level <= MAXIMUM_LEVEL; level++) {
  THRESHOLDS[level - 1] = Math.pow(level, 2) * 100;
}

export function getLevel(experience: number): number {
  if (experience < 0) {
    throw new Error("Experience must be greater than or equal to 0.");
  }
  for (let level = 0; level < MAXIMUM_LEVEL; level++) {
    const threshold: number = THRESHOLDS[level] ?? 0;
    if (experience < threshold) {
      return level;
    }
  }
  return MAXIMUM_LEVEL;
}

export function getThreshold(level: number): number {
  if (level < 0 || level > MAXIMUM_LEVEL) {
    throw new Error(`Level must be comprised between 0 and ${MAXIMUM_LEVEL}.`);
  }
  return THRESHOLDS[level - 1] ?? 0;
}
