<script setup lang="ts">
import { TarButton, type SelectOption } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import AppSelect from "@/components/shared/AppSelect.vue";
import LineageSelect from "@/components/lineages/LineageSelect.vue";
import NameInput from "@/components/shared/NameInput.vue";
import type { LineageModel, SearchLineagesPayload } from "@/types/lineages";
import type { SearchResults } from "@/types/search";
import type { Step1 } from "@/types/characters";
import { searchLineages } from "@/api/lineages";

const { t } = useI18n();

const isLoading = ref<boolean>(false);
const nation = ref<LineageModel>();
const nations = ref<LineageModel[]>([]);
const player = ref<string>("");
const species = ref<LineageModel>();

const nationOptions = computed<SelectOption[]>(() => nations.value.map(({ id, name }) => ({ text: name, value: id })));

const emit = defineEmits<{
  (e: "abandon"): void;
  (e: "continue", value: Step1): void;
  (e: "error", value: unknown): void;
}>();

async function setSpecies(value?: LineageModel): Promise<void> {
  species.value = value;
  nation.value = undefined;
  if (value && !isLoading.value) {
    isLoading.value = true;
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
    } finally {
      isLoading.value = false;
    }
  } else {
    nations.value = [];
  }
}

const { handleSubmit } = useForm();
const onSubmit = handleSubmit(() =>
  emit("continue", {
    player: player.value,
    species: species.value,
    nation: nation.value,
  }),
);
</script>

<template>
  <div>
    <h3>{{ t("characters.steps.lineage") }}</h3>
    <form @submit="onSubmit">
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
      <TarButton class="me-1" icon="fas fa-ban" :text="t('actions.abandon')" variant="danger" @click="$emit('abandon')" />
      <TarButton class="ms-1" :disabled="isLoading" icon="fas fa-arrow-right" :text="t('actions.continue')" type="submit" />
    </form>
  </div>
</template>
