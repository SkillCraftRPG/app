<template>
  <section>
    <h2 class="h3">{{ t("characters.creation.ascendancy.title") }}</h2>
    <h3 class="h5">{{ t("lineages.species.label") }}</h3>
    <p>TODO(fpion): help</p>
    <div v-if="speciesList.length" class="row">
      <div v-for="lineage in speciesList" :key="lineage.id" class="col-md-6 col-lg-4 col-xl-3 mb-3">
        <LineageCard class="d-flex flex-column h-100" clickable :lineage="lineage" :selected="lineage.id === species?.id" @click="toggleSpecies(lineage)">
          <div class="d-flex justify-content-between align-items-center gap-2 mt-2">
            <font-awesome-icon :icon="lineage.id === species?.id ? 'far fa-square-check' : 'far fa-square'" />
            <RouterLink class="btn btn-outline-primary" target="_blank" :to="{ name: 'Lineage', params: { id: lineage.id } }">
              <!-- TODO(fpion): clicking the link toggles the species 😔 -->
              <font-awesome-icon aria-hidden="true" icon="fas fa-edit" />&nbsp;{{ t("actions.edit") }}
            </RouterLink>
          </div>
        </LineageCard>
      </div>
    </div>
    <p v-else>TODO(fpion): empty</p>
  </section>
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import LineageCard from "@/components/lineages/LineageCard.vue";
import type { Lineage } from "@/types/lineages";
import type { SearchResults } from "@/types/search";
import { listSpecies } from "@/api/lineages";

const { orderBy } = arrayUtils;
const { t } = useI18n();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
}>();

const species = ref<Lineage>();
const speciesList = ref<Lineage[]>([]);

function toggleSpecies(value: Lineage): void {
  species.value = species.value?.id === value.id ? undefined : value;
}

onMounted(async () => {
  try {
    const results: SearchResults<Lineage> = await listSpecies();
    speciesList.value = orderBy(results.items, "name");
  } catch (e: unknown) {
    emit("error", e);
  }
});
</script>
