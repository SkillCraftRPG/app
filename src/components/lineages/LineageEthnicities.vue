<template>
  <div>
    <CreateLineage class="mb-3" :parent="lineage" @created="onCreate" @error="$emit('error', $event)" />
    <div v-if="ethnicities.length" class="row">
      <div v-for="ethnicity in ethnicities" :key="ethnicity.id" class="col-md-6 col-lg-4 col-xl-3 mb-3">
        <LineageLinkCard class="h-100" :lineage="ethnicity" />
      </div>
    </div>
    <p v-else>{{ t("lineages.ethnicities.empty") }}</p>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";

import CreateLineage from "./CreateLineage.vue";
import LineageLinkCard from "./LineageLinkCard.vue";
import type { Lineage, SearchLineagesPayload } from "@/types/lineages";
import type { SearchResults } from "@/types/search";
import { searchLineages } from "@/api/lineages";
import { useEventStore } from "@/stores/event";

const events = useEventStore();
const router = useRouter();
const { t } = useI18n();

const props = defineProps<{
  lineage: Lineage;
}>();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
}>();

const ethnicities = ref<Lineage[]>([]);

function onCreate(lineage: Lineage): void {
  events.push("created");
  router.push({ name: "Lineage", params: { id: lineage.id } });
}

watch(
  () => props.lineage,
  async (lineage) => {
    try {
      const payload: SearchLineagesPayload = {
        ids: [],
        parentId: lineage.id,
        search: { terms: [], operator: "And" },
        sort: [{ field: "Name", isDescending: false }],
        skip: 0,
        limit: 0,
      };
      const results: SearchResults<Lineage> = await searchLineages(payload);
      ethnicities.value = [...results.items];
    } catch (e: unknown) {
      emit("error", e);
    }
  },
  { deep: true, immediate: true },
);
</script>
