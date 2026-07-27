<template>
  <main class="container worlds-page d-flex flex-column flex-grow-1">
    <header class="mb-4">
      <div class="d-flex flex-column flex-sm-row justify-content-between align-items-sm-start gap-3">
        <h1 class="mb-0">{{ t("worlds.title") }}</h1>
        <CreateWorld v-if="!isLoading && worlds.length" />
      </div>
      <p class="text-body-secondary mt-2">{{ t("worlds.help") }}</p>
      <RouterLink :to="{ name: 'CharacterSheets' }">{{ t("sheets.characters.go") }}</RouterLink>
    </header>
    <LoadingSpinner v-if="isLoading" />
    <div v-else-if="worlds.length" class="row">
      <div v-for="world in worlds" :key="world.id" class="col-md-6 col-lg-4">
        <WorldCard :world="world" />
      </div>
    </div>
    <div v-else class="d-flex flex-column justify-content-center align-items-center text-center flex-grow-1 py-5">
      <font-awesome-icon icon="fas fa-globe" class="display-4 text-body-secondary mb-3" aria-hidden="true" />
      <h2 class="h4 mb-2">{{ t("worlds.empty.lead") }}</h2>
      <p class="text-body-secondary mb-4">{{ t("worlds.empty.help") }}</p>
      <CreateWorld />
    </div>
  </main>
</template>

<script setup lang="ts">
import { inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import CreateWorld from "@/components/worlds/CreateWorld.vue";
import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import WorldCard from "@/components/worlds/WorldCard.vue";
import type { SearchResults } from "@/types/search";
import type { World } from "@/types/worlds";
import { handleErrorKey } from "@/inject";
import { searchWorlds } from "@/api/worlds";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const { t } = useI18n();

const isLoading = ref<boolean>(true);
const worlds = ref<World[]>([]);

onMounted(async () => {
  try {
    const results: SearchResults<World> = await searchWorlds({
      ids: [],
      search: { terms: [], operator: "And" },
      sort: [{ field: "Name", isDescending: false }],
      skip: 0,
      limit: 0,
    });
    worlds.value = [...results.items];
  } catch (e: unknown) {
    handleError(e);
  } finally {
    isLoading.value = false;
  }
});
</script>

<style scoped>
html,
body,
#app {
  min-height: 100%;
}

#app {
  min-height: 100dvh;
  display: flex;
  flex-direction: column;
}

main {
  display: flex;
  flex: 1 1 auto;
  flex-direction: column;
}

.worlds-page {
  flex: 1 1 auto;
}
</style>
