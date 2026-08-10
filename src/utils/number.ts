const HUNDREDTHS = 100;

export function fromHundredths(value: number | null | undefined): number | undefined {
  return value == null ? undefined : Math.round(value) / HUNDREDTHS;
}

export function toHundredths(value: number | null | undefined): number | undefined {
  return value == null ? undefined : Math.round(value * HUNDREDTHS);
}
