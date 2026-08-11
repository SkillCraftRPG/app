export function calculateWeight(heightCentimeters: number, bodyMassIndex: number): number {
  return (heightCentimeters / 100) * (heightCentimeters / 100) * bodyMassIndex;
}
