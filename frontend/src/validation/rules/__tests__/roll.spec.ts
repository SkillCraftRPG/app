import { describe, it, expect } from "vitest";

import roll from "../roll";

describe("roll", () => {
  it.concurrent("should return false when the value is not a valid roll", () => {
    expect(roll()).toBe(false);
    expect(roll("")).toBe(false);
    expect(roll("   ")).toBe(false);
    expect(roll("Test123!")).toBe(false);
    expect(roll("-1d4")).toBe(false);
    expect(roll("1d6-1")).toBe(false);
    expect(roll("1d6+abc")).toBe(false);
  });

  it.concurrent("should return true when the value is a valid roll", () => {
    expect(roll("1")).toBe(true);
    expect(roll("1d4")).toBe(true);
    expect(roll("1d4+1d12")).toBe(true);
    expect(roll("10+1d10")).toBe(true);
  });
});
