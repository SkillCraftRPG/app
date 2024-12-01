import { describe, it, expect } from "vitest";

import { calculateModifier, getTotalExperience, roll } from "../gameUtils";

describe("gameUtils.calculateModifier", () => {
  it.concurrent("should return the correct modifier", () => {
    expect(calculateModifier(0)).toBe(-5);
    expect(calculateModifier(6)).toBe(-2);
    expect(calculateModifier(7)).toBe(-2);
    expect(calculateModifier(8)).toBe(-1);
    expect(calculateModifier(9)).toBe(-1);
    expect(calculateModifier(10)).toBe(0);
    expect(calculateModifier(11)).toBe(0);
    expect(calculateModifier(12)).toBe(1);
    expect(calculateModifier(13)).toBe(1);
    expect(calculateModifier(14)).toBe(2);
    expect(calculateModifier(15)).toBe(2);
    expect(calculateModifier(16)).toBe(3);
    expect(calculateModifier(17)).toBe(3);
    expect(calculateModifier(18)).toBe(4);
    expect(calculateModifier(19)).toBe(4);
    expect(calculateModifier(20)).toBe(5);
    expect(calculateModifier(21)).toBe(5);
    expect(calculateModifier(22)).toBe(6);
    expect(calculateModifier(23)).toBe(6);
    expect(calculateModifier(24)).toBe(7);
    expect(calculateModifier(25)).toBe(7);
    expect(calculateModifier(30)).toBe(10);
  });
});

describe("gameUtils.getTotalExperience", () => {
  it.concurrent("should return the correct total experience", () => {
    expect(getTotalExperience(0)).toBe(0);
    expect(getTotalExperience(1)).toBe(100);
    expect(getTotalExperience(10)).toBe(34000);
    expect(getTotalExperience(20)).toBe(268000);
  });

  it.concurrent("should throw an error when the level is not within boundaries", () => {
    expect(() => getTotalExperience(-1)).toThrowError("The level must be comprised between 0 and 20. The boundaries are inclusive.");
    expect(() => getTotalExperience(21)).toThrowError("The level must be comprised between 0 and 20. The boundaries are inclusive.");
  });
});

describe("gameUtils.roll", () => {
  it.concurrent("should perform a complex roll", () => {
    const result: number = roll("2+2d4");
    expect(result >= 4 && result <= 10).toBe(true);
  });

  it.concurrent("should perform a single addition", () => {
    expect(roll("5+7")).toBe(12);
  });

  it.concurrent("should perform multiple additions", () => {
    expect(roll("1+2+3+4+5")).toBe(15);
  });

  it.concurrent("should return a single number", () => {
    expect(roll("5")).toBe(5);
  });

  it.concurrent("should return zero when the roll is empty", () => {
    expect(roll("")).toBe(0);
    expect(roll("   ")).toBe(0);
    expect(roll("0+ ")).toBe(0);
    expect(roll("0d10")).toBe(0);
  });

  it.concurrent("should roll a single die", () => {
    const result: number = roll("1d4");
    expect(result >= 1 && result <= 4).toBe(true);
  });

  it.concurrent("should roll multiple dice", () => {
    const result: number = roll("1d4+2d6");
    expect(result >= 3 && result <= 16).toBe(true);
  });
});
