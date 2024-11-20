import { nanoid } from "nanoid";
import { beforeEach, describe, it, expect } from "vitest";
import { setActivePinia, createPinia } from "pinia";

import { useCharacterStore } from "../character";
import type { Step1, Step2, Step3, Step4, Step5, Step6 } from "@/types/characters";
import type { WorldModel } from "@/types/worlds";
import type { Actor } from "@/types/actor";

const actor: Actor = {
  id: nanoid(),
  type: "User",
  isDeleted: false,
  displayName: "Francis Pion",
};
const now: string = new Date().toISOString();

const world: WorldModel = {
  id: nanoid(),
  version: 0,
  createdBy: actor,
  createdOn: now,
  updatedBy: actor,
  updatedOn: now,
  owner: actor,
  slug: "ungar",
};

describe("characterStore", () => {
  beforeEach(() => {
    setActivePinia(createPinia());
  });

  it("should advance forward in steps", () => {
    const character = useCharacterStore();
    expect(character.step).toBe(1);
    character.next();
    expect(character.step).toBe(2);
  });

  it("should be empty initially", () => {
    const character = useCharacterStore();
    expect(character.step).toBe(1);
    expect(character.creation.step1).toBeUndefined();
    expect(character.creation.step2).toBeUndefined();
    expect(character.creation.step3).toBeUndefined();
    expect(character.creation.step4).toBeUndefined();
    expect(character.creation.step5).toBeUndefined();
    expect(character.creation.step6).toBeUndefined();
  });

  it("should go back in steps", () => {
    const character = useCharacterStore();
    character.next();
    expect(character.step).toBe(2);
    character.back();
    expect(character.step).toBe(1);
  });

  it("should not go beyond step #6", () => {
    const character = useCharacterStore();
    character.next();
    character.next();
    character.next();
    character.next();
    character.next();
    expect(character.step).toBe(6);
    character.next();
    expect(character.step).toBe(6);
  });

  it("should not go under step #1", () => {
    const character = useCharacterStore();
    character.back();
    expect(character.step).toBe(1);
  });

  it("should reset the character creation steps", () => {
    const character = useCharacterStore();
    character.next();
    character.next();
    character.next();
    character.setStep4({
      attributes: {
        agility: 0,
        coordination: 0,
        intellect: 0,
        presence: 0,
        sensitivity: 0,
        spirit: 0,
        vigor: 0,
        best: "Vigor",
        worst: "Sensitivity",
        optional: [],
        extra: [],
      },
    });
    character.next();
    expect(character.step).toBe(5);
    expect(character.creation.step4).toBeDefined();
    character.reset();
    expect(character.step).toBe(1);
    expect(character.creation.step1).toBeUndefined();
    expect(character.creation.step2).toBeUndefined();
    expect(character.creation.step3).toBeUndefined();
    expect(character.creation.step4).toBeUndefined();
    expect(character.creation.step5).toBeUndefined();
    expect(character.creation.step6).toBeUndefined();
  });

  it("should step the character creation step #1", () => {
    const character = useCharacterStore();
    const step1: Step1 = {
      name: "Heracles Aetos",
      species: {
        id: nanoid(),
        version: 0,
        createdBy: actor,
        createdOn: now,
        updatedBy: actor,
        updatedOn: now,
        world,
        name: "Orrin",
        attributes: { agility: 0, coordination: 0, intellect: 0, presence: 0, sensitivity: 0, spirit: 0, vigor: 0, extra: 0 },
        traits: [],
        languages: { items: [], extra: 0 },
        names: { family: [], female: [], male: [], unisex: [], custom: [] },
        speeds: { walk: 0, climb: 0, swim: 0, fly: 0, hover: 0, burrow: 0 },
        size: { category: "Medium" },
        weight: {},
        ages: {},
        nations: [],
      },
      height: 1.67,
      weight: 62.8,
      age: 18,
      languages: [],
    };
    character.setStep1(step1);
    expect(character.creation.step1).toBeDefined();
    expect(JSON.stringify(character.creation.step1)).toBe(JSON.stringify(step1));
  });

  it("should set the character creation step #2", () => {
    const character = useCharacterStore();
    const step2: Step2 = {
      nature: {
        id: nanoid(),
        version: 0,
        createdBy: actor,
        createdOn: now,
        updatedBy: actor,
        updatedOn: now,
        world,
        name: "Courroucé",
      },
      customizations: [],
    };
    character.setStep2(step2);
    expect(character.creation.step2).toBeDefined();
    expect(JSON.stringify(character.creation.step2)).toBe(JSON.stringify(step2));
  });

  it("should set the character creation step #3", () => {
    const character = useCharacterStore();
    const step3: Step3 = {
      aspects: [
        {
          id: nanoid(),
          version: 0,
          createdBy: actor,
          createdOn: now,
          updatedBy: actor,
          updatedOn: now,
          world,
          name: "Farouche",
          attributes: {},
          skills: {},
        },
        {
          id: nanoid(),
          version: 0,
          createdBy: actor,
          createdOn: now,
          updatedBy: actor,
          updatedOn: now,
          world,
          name: "Gymnaste",
          attributes: {},
          skills: {},
        },
      ],
    };
    character.setStep3(step3);
    expect(character.creation.step3).toBeDefined();
    expect(JSON.stringify(character.creation.step3)).toBe(JSON.stringify(step3));
  });

  it("should set the character creation step #4", () => {
    const character = useCharacterStore();
    const step4: Step4 = {
      attributes: {
        agility: 8,
        coordination: 8,
        intellect: 8,
        presence: 8,
        sensitivity: 8,
        spirit: 8,
        vigor: 9,
        best: "Vigor",
        worst: "Sensitivity",
        optional: ["Sensitivity", "Vigor"],
        extra: ["Agility", "Vigor"],
      },
    };
    character.setStep4(step4);
    expect(character.creation.step4).toBeDefined();
    expect(JSON.stringify(character.creation.step4)).toBe(JSON.stringify(step4));
  });

  it("should set the character creation step #5", () => {
    const character = useCharacterStore();
    const step5: Step5 = {
      caste: {
        id: nanoid(),
        version: 0,
        createdBy: actor,
        createdOn: now,
        updatedBy: actor,
        updatedOn: now,
        world,
        name: "Exilé",
        features: [],
      },
      education: {
        id: nanoid(),
        version: 0,
        createdBy: actor,
        createdOn: now,
        updatedBy: actor,
        updatedOn: now,
        world,
        name: "Champs de bataille",
      },
      item: {
        id: nanoid(),
        version: 0,
        createdBy: actor,
        createdOn: now,
        updatedBy: actor,
        updatedOn: now,
        world,
        name: "Denier",
        isAttunementRequired: false,
        category: "Money",
      },
      quantity: 100,
    };
    character.setStep5(step5);
    expect(character.creation.step5).toBeDefined();
    expect(JSON.stringify(character.creation.step5)).toBe(JSON.stringify(step5));
  });

  it("should set the character creation step #6", () => {
    const character = useCharacterStore();
    const step6: Step6 = {
      talents: [
        {
          id: nanoid(),
          version: 0,
          createdBy: actor,
          createdOn: now,
          updatedBy: actor,
          updatedOn: now,
          world,
          tier: 0,
          name: "Acrobaties",
          allowMultiplePurchases: false,
          skill: "Acrobatics",
        },
        {
          id: nanoid(),
          version: 0,
          createdBy: actor,
          createdOn: now,
          updatedBy: actor,
          updatedOn: now,
          world,
          tier: 0,
          name: "Athlétisme",
          allowMultiplePurchases: false,
          skill: "Athletics",
        },
      ],
    };
    character.setStep6(step6);
    expect(character.creation.step6).toBeDefined();
    expect(JSON.stringify(character.creation.step6)).toBe(JSON.stringify(step6));
  });
});
