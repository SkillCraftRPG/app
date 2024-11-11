<script setup lang="ts">
import { onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import CreateLineage from "./CreateLineage.vue";
import type { LineageModel, SearchLineagesPayload } from "@/types/lineages";
import type { SearchResults } from "@/types/search";
import { searchLineages } from "@/api/lineages";

const { t } = useI18n();

const props = defineProps<{
  species: LineageModel;
}>();

const nations = ref<LineageModel[]>([]);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
}>();

function onCreated(lineage: LineageModel) {
  console.log(lineage); // TODO(fpion): implement
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
    <div v-if="nations.length > 0" class="row">
      <!-- TODO(fpion): implement -->
    </div>
    <p v-else>{{ t("lineages.nations.empty") }}</p>
  </div>
</template>
