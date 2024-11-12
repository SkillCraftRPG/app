<script setup lang="ts">
import { TarButton, type SelectOption } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import AppSelect from "@/components/shared/AppSelect.vue";
import HeightRollInput from "./HeightRollInput.vue";
import LineageSelect from "@/components/lineages/LineageSelect.vue";
import NameInput from "@/components/shared/NameInput.vue";
import SizeCategorySelect from "@/components/game/SizeCategorySelect.vue";
import WeightCategorySelect from "@/components/game/WeightCategorySelect.vue";
import WeightRollInput from "./WeightRollInput.vue";
import type { LineageModel, SearchLineagesPayload, WeightCategory } from "@/types/lineages";
import type { SearchResults } from "@/types/search";
import type { SizeCategory } from "@/types/game";
import type { Step1 } from "@/types/characters";
import { searchLineages } from "@/api/lineages";

const { t } = useI18n();

const height = ref<number>(0);
const isLoading = ref<boolean>(false);
const nation = ref<LineageModel>();
const nations = ref<LineageModel[]>([]);
const player = ref<string>("");
const species = ref<LineageModel>();
const weight = ref<number>(0);
const weightCategory = ref<WeightCategory>("Normal");

const nationOptions = computed<SelectOption[]>(() => nations.value.map(({ id, name }) => ({ text: name, value: id })));
const sizeCategory = computed<SizeCategory>(() => nation.value?.size.category ?? species.value?.size.category ?? "Medium");
const sizeRoll = computed<string | undefined>(() => nation.value?.size.roll ?? species.value?.size.roll);
const weightRoll = computed<string | undefined>(() => {
  if (species.value && weightCategory.value) {
    switch (weightCategory.value) {
      case "Normal":
        return nation.value?.weight.normal ?? species.value.weight.normal;
      case "Obese":
        return nation.value?.weight.obese ?? species.value.weight.obese;
      case "Overweight":
        return nation.value?.weight.overweight ?? species.value.weight.overweight;
      case "Skinny":
        return nation.value?.weight.skinny ?? species.value.weight.skinny;
      case "Starved":
        return nation.value?.weight.starved ?? species.value.weight.starved;
    }
  }
  return undefined;
});

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
      <NameInput id="player" label="characters.player" placeholder="characters.player" v-model="player" />
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
          @selected="nation = $event"
        />
      </div>
      <!-- TODO(fpion): Name -->
      <template v-if="species">
        <h5>{{ t("characters.size") }}</h5>
        <div class="row">
          <SizeCategorySelect class="col" disabled :model-value="sizeCategory" validation="server" />
          <HeightRollInput class="col" :roll="sizeRoll" v-model="height" />
        </div>
        <h5>{{ t("characters.weight") }}</h5>
        <div class="row">
          <WeightCategorySelect class="col" v-model="weightCategory" />
          <WeightRollInput class="col" :height="height" :roll="weightRoll" v-model="weight" />
        </div>
      </template>
      <!-- TODO(fpion): Age -->
      <!-- TODO(fpion): Languages -->
      <TarButton class="me-1" icon="fas fa-ban" :text="t('actions.abandon')" variant="danger" @click="$emit('abandon')" />
      <TarButton class="ms-1" :disabled="isLoading" icon="fas fa-arrow-right" :text="t('actions.continue')" type="submit" />
    </form>
  </div>
</template>
