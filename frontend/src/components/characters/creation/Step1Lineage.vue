<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import AppSelect from "@/components/shared/AppSelect.vue";
import LineageSelect from "@/components/lineages/LineageSelect.vue";
import NameInput from "@/components/shared/NameInput.vue";
import type { LineageModel, SearchLineagesPayload } from "@/types/lineages";
import type { SearchResults } from "@/types/search";
import { searchLineages } from "@/api/lineages";

const { t } = useI18n();

const nation = ref<LineageModel>();
const nations = ref<LineageModel[]>([]);
const player = ref<string>("");
const species = ref<LineageModel>();

const nationOptions = computed<SelectOption[]>(() => nations.value.map(({ id, name }) => ({ text: name, value: id })));

const emit = defineEmits<{
  (e: "error", value: unknown): void;
}>();

async function setSpecies(value?: LineageModel): Promise<void> {
  species.value = value;
  nation.value = undefined;
  if (value) {
    try {
      const payload: SearchLineagesPayload = {
        ids: [],
        parentId: value.id,
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
  } else {
    nations.value = [];
  }
}
</script>

<template>
  <div>
    <h3>{{ t("characters.steps.lineage") }}</h3>
    <form>
      <div class="row">
        <LineageSelect
          class="col"
          id="species"
          label="lineages.species.label"
          :model-value="species?.id"
          placeholder="lineages.species.placeholder"
          required
          @error="$emit('error', $event)"
          @selected="setSpecies"
        />
        <AppSelect
          v-if="nations.length > 0"
          class="col"
          floating
          id="nation"
          label="lineages.nation.label"
          :model-value="nation?.id"
          :options="nationOptions"
          placeholder="lineages.nation.placeholder"
          required
          @update:model-value="nation = $event"
        />
      </div>
      <NameInput id="player" label="characters.player" placeholder="characters.player" v-model="player" />
      <!-- TODO(fpion): Name -->
      <!-- TODO(fpion): Size -->
      <!-- TODO(fpion): Weight -->
      <!-- TODO(fpion): Age -->
      <!-- TODO(fpion): Languages -->
    </form>
  </div>
</template>
