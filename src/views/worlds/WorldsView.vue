<template>
  <main class="container page">
    <header class="mb-4">
      <div class="d-flex flex-column flex-sm-row justify-content-between align-items-sm-start gap-3">
        <h1 class="mb-0">{{ title }}</h1>
        <CreateWorld class="mb-3" v-if="!isLoading && worlds.length && canCreateWorld" @created="onCreate" @error="handleError" />
      </div>
      <p class="text-body-secondary">{{ t("worlds.help") }}</p>
      <RouterLink :to="{ name: 'CharacterSheets' }"><font-awesome-icon icon="fas fa-id-card" />&nbsp;{{ t("sheets.characters.go") }}</RouterLink>
    </header>
    <LoadingSpinner v-if="isLoading" />
    <div v-else-if="worlds.length" class="row">
      <div v-for="world in worlds" :key="world.id" class="col-md-6 col-lg-4 mb-3">
        <WorldCard :world="world" />
      </div>
    </div>
    <div v-else class="d-flex flex-column justify-content-center align-items-center text-center flex-grow-1 py-5">
      <font-awesome-icon icon="fas fa-dungeon" class="display-4 text-body-secondary mb-3" aria-hidden="true" />
      <h2 class="h4 mb-2">{{ t("worlds.empty.lead") }}</h2>
      <p class="text-body-secondary mb-4">{{ t("worlds.empty.help") }}</p>
      <CreateWorld @created="onCreate" @error="handleError" />
    </div>
  </main>
</template>

<script setup lang="ts">
import { computed, inject, onMounted, ref, watchEffect } from "vue";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";

import CreateWorld from "@/components/worlds/CreateWorld.vue";
import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import WorldCard from "@/components/worlds/WorldCard.vue";
import type { SearchResults } from "@/types/search";
import type { World } from "@/types/worlds";
import { handleErrorKey } from "@/inject";
import { searchWorlds } from "@/api/worlds";
import { useDocument } from "@/composables/document";
import { useEventStore } from "@/stores/event";

const document = useDocument();
const events = useEventStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const router = useRouter();
const { t } = useI18n();

const isLoading = ref<boolean>(true);
const worlds = ref<World[]>([]);

const canCreateWorld = computed<boolean>(() => worlds.value.length < 3);
const title = computed<string>(() => t("worlds.title"));

function onCreate(world: World): void {
  events.push("created");
  router.push({ name: "World", params: { id: world.id } });
}

watchEffect(() => document.setTitle(title.value));

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
