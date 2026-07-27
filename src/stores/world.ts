import { defineStore } from "pinia";
import { ref } from "vue";

import type { World } from "@/types/worlds";

export const useWorldStore = defineStore(
  "world",
  () => {
    const current = ref<World>();

    function enter(value: World): void {
      current.value = value;
    }

    function exit(): void {
      current.value = undefined;
    }

    return { current, enter, exit };
  },
  {
    persist: true,
  },
);
