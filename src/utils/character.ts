export function calculateAttributePoints(level: number): number {
  return Math.floor((level + 5) / 10);
}

export function calculateWeight(heightCentimeters: number, bodyMassIndex: number): number {
  return (heightCentimeters / 100) * (heightCentimeters / 100) * bodyMassIndex;
}
