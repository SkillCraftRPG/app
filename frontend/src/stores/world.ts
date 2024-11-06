import { defineStore } from "pinia";
import { ref } from "vue";

import type { WorldModel } from "@/types/worlds";
import { readWorld } from "@/api/worlds";

export const useWorldStore = defineStore("world", () => {
  const worlds = ref<WorldModel[]>([]);

  async function retrieve(slug: string): Promise<WorldModel> {
    let world: WorldModel | undefined = worlds.value.find((world) => world.slug === slug);
    if (!world) {
      world = await readWorld(slug);
      save(world);
    }
    return world;
  }
  function save(world: WorldModel): void {
    const index: number = worlds.value.findIndex(({ id }) => id === world.id);
    if (index < 0) {
      worlds.value.push(world);
    } else {
      worlds.value.splice(index, 1, world);
    }
  }

  return { worlds, retrieve, save };
});
