<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import WorldCard from "@/components/worlds/WorldCard.vue";
import type { SearchResults } from "@/types/search";
import type { WorldModel } from "@/types/worlds";
import { handleErrorKey } from "@/inject/App";
import { searchWorlds } from "@/api/worlds";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const { t } = useI18n();

const worlds = ref<WorldModel[]>([]);

const names = computed<Map<string, number>>(() => {
  const names = new Map<string, number>();
  worlds.value.forEach(({ name }) => {
    if (name) {
      let count: number = names.get(name) ?? 0;
      count++;
      names.set(name, count);
    }
  });
  return names;
});

function compare(a: WorldModel, b: WorldModel): number {
  const x: string = (a.name ?? a.slug).toLowerCase();
  const y: string = (b.name ?? b.slug).toLowerCase();
  return x > y ? 1 : x < y ? -1 : 0;
}

onMounted(async () => {
  try {
    const results: SearchResults<WorldModel> = await searchWorlds({
      ids: [],
      search: { terms: [], operator: "And" },
      sort: [],
      skip: 0,
      limit: 0,
    });
    worlds.value = results.items.sort(compare);
  } catch (e: unknown) {
    handleError(e);
  }
});
</script>

<template>
  <main class="container">
    <h1 class="text-center">{{ t("worlds.gateway.title") }}</h1>
    <div v-if="worlds.length > 0" class="my-3 row">
      <div v-for="world in worlds" :key="world.id" class="col-md-12 col-lg-6 col-xl-4 mb-3">
        <WorldCard :subtitle="Boolean(world.name && (names.get(world.name) ?? 0) > 1)" :world="world" />
      </div>
    </div>
    <p v-else>
      {{ t("worlds.gateway.empty") }}
    </p>
  </main>
</template>
