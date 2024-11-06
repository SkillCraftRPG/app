import { beforeEach, describe, it, expect } from "vitest";
import { nanoid } from "nanoid";
import { setActivePinia, createPinia } from "pinia";

import { useWorldStore } from "../world";
import type { WorldModel } from "@/types/worlds";
import type { Actor } from "@/types/actor";

const owner: Actor = {
  id: nanoid(),
  type: "User",
  isDeleted: false,
  displayName: "Francis Pion",
};
const now: string = new Date().toISOString();
const world: WorldModel = {
  id: nanoid(),
  version: 1,
  createdBy: owner,
  createdOn: now,
  updatedBy: owner,
  updatedOn: now,
  owner,
  slug: "ungar",
};

describe("worldStore", () => {
  beforeEach(() => {
    setActivePinia(createPinia());
  });

  it.concurrent("should be initially empty", () => {
    const world = useWorldStore();
    expect(world.worlds.length).toBe(0);
  });

  it.concurrent("should retrieve a cached world", async () => {
    const store = useWorldStore();
    store.save(world);
    const retrieved: WorldModel = await store.retrieve(world.slug);
    expect(retrieved.id).toBe(world.id);
  });

  it.concurrent("should save a world", () => {
    const store = useWorldStore();
    store.save(world);
    expect(store.worlds.find(({ id }) => id === world.id)?.id).toBe(world.id);
  });
});
