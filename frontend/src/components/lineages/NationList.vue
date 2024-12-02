<script setup lang="ts">
import { onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import CreateLineage from "./CreateLineage.vue";
import LineageCard from "./LineageCard.vue";
import type { LineageModel, SearchLineagesPayload } from "@/types/lineages";
import type { SearchResults } from "@/types/search";
import { searchLineages } from "@/api/lineages";
import { useToastStore } from "@/stores/toast";

const toasts = useToastStore();
const { t } = useI18n();

const props = defineProps<{
  species: LineageModel;
}>();

const nations = ref<LineageModel[]>([]);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
}>();

function onCreated(lineage: LineageModel) {
  toasts.success("lineages.created");
  nations.value.push(lineage);
}

onMounted(async () => {
  try {
    const payload: SearchLineagesPayload = {
      ids: [],
      parentId: props.species.id,
      search: { terms: [], operator: "And" },
      sort: [{ field: "Name", isDescending: false }],
      skip: 0,
      limit: 0,
    };
    const results: SearchResults<LineageModel> = await searchLineages(payload);
    nations.value = results.items;
  } catch (e: unknown) {
    emit("error", e);
  }
});
</script>

<template>
  <div>
    <h3>{{ t("lineages.nations.label") }}</h3>
    <div class="mb-3">
      <CreateLineage :species="species" @created="onCreated" @error="$emit('error', $event)" />
    </div>
    <div v-if="nations.length > 0" class="mb-3 row">
      <div v-for="nation in nations" :key="nation.id" class="col-lg-3">
        <LineageCard :lineage="nation" />
      </div>
    </div>
    <p v-else>{{ t("lineages.nations.empty") }}</p>
  </div>
</template>
