const HUNDREDTHS: number = 100;
const TENTHS: number = 10;

export function fromHundredths(value: number | null | undefined): number | undefined {
  return value == null ? undefined : Math.round(value) / HUNDREDTHS;
}

export function fromTenths(value: number | null | undefined): number | undefined {
  return value == null ? undefined : Math.round(value) / TENTHS;
}

export function toHundredths(value: number | null | undefined): number | undefined {
  return value == null ? undefined : Math.round(value * HUNDREDTHS);
}

export function toTenths(value: number | null | undefined): number | undefined {
  return value == null ? undefined : Math.round(value * TENTHS);
}
