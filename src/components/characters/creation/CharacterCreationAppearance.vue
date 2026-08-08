<template>
  <form @submit.prevent="handleSubmit(submit)">
    <h2 class="h3">{{ t("characters.appearance.title") }}</h2>
    <p class="text-body-secondary">{{ t("characters.appearance.help") }}</p>
    <section>
      <h3 class="h5">{{ t("lineages.physical.size") }}</h3>
      <div class="row">
        <div class="col-md-6">
          <div class="mb-3">
            <div class="fw-bold">{{ t("game.size.category.label") }}</div>
            <div>{{ sizeCategory }}</div>
          </div>
        </div>
        <div class="col-md-6">
          <HeightField class="mb-3" :roll="heightRoll" v-model="height">
            <TarButton v-if="heightRoll" icon="fas fa-dice" :text="heightRoll" @click="randomizeHeight" />
          </HeightField>
        </div>
      </div>
    </section>
    <section>
      <h3 class="h5">{{ t("lineages.physical.weight.label") }}</h3>
      <div class="row">
        <div class="col-md-4">
          <TarSelect
            class="mb-3"
            :disabled="weightOptions.length <= 1"
            floating
            id="weight-category"
            :label="t('lineages.physical.weight.category.label')"
            :model-value="weightCategory"
            :options="weightOptions"
            :placeholder="t('lineages.physical.weight.category.placeholder')"
            @update:model-value="updateWeightCategory"
          />
        </div>
        <div class="col-md-4">
          <InputField
            class="mb-3"
            id="body-mass-index"
            :label="t('characters.physical.bodyMassIndex')"
            min="0"
            max="99"
            :model-value="bodyMassIndex.toString()"
            step="1"
            type="number"
            @update:model-value="bodyMassIndex = parseNumber($event) ?? 0"
          >
            <template #append>
              <TarButton v-if="weightRoll" icon="fas fa-dice" :text="weightRoll" @click="randomizeWeight" />
            </template>
          </InputField>
        </div>
        <div class="col-md-4">
          <div class="mb-3">
            <div class="fw-bold">{{ t("characters.physical.weight") }}</div>
            <div>{{ n(weight, "characterWeight") }}&nbsp;{{ t("game.unit.kg") }}</div>
          </div>
        </div>
      </div>
    </section>
    <div class="d-flex justify-content-between">
      <div class="d-flex gap-2">
        <TarButton icon="fas fa-xmark" outline :text="t('actions.abandon')" variant="danger" @click="$emit('abandon')" />
        <TarButton icon="fas fa-arrow-left" outline :text="t('actions.previous')" variant="secondary" @click="character.goBack" />
      </div>
      <TarButton icon="fas fa-arrow-right" :text="t('actions.next')" type="submit" />
    </div>
  </form>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import HeightField from "@/components/characters/HeightField.vue";
import InputField from "@/components/forms/InputField.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarSelect from "@/components/tar/TarSelect.vue";
import type { SelectOption } from "@/types/tar/select";
import type { SizeCategory } from "@/types/game";
import { roll } from "@/utils/random";
import { useCharacterStore } from "@/stores/character";
import { useForm } from "@/forms";

const character = useCharacterStore();
const { parseNumber } = parsingUtils;
const { n, t } = useI18n();

defineEmits<{
  (e: "abandon"): void;
}>();

const bodyMassIndex = ref<number>(0);
const height = ref<number>(0);
const weightCategory = ref<string>("");

const heightRoll = computed<string | null | undefined>(() => character.creation.ethnicity?.size.height ?? character.creation.species?.size.height);
const sizeCategory = computed<string>(() => {
  const category: SizeCategory = character.creation.ethnicity?.size.category ?? character.creation.species?.size.category ?? "Medium";
  return t(`game.size.category.options.${category}`);
});

const weight = computed<number>(() => (height.value / 100) * (height.value / 100) * bodyMassIndex.value);
const weightOptions = computed<SelectOption[]>(() => {
  const categories: string[] = [];
  if (character.creation.ethnicity?.weight.malnutrition ?? character.creation.species?.weight.malnutrition) {
    categories.push("malnutrition");
  }
  if (character.creation.ethnicity?.weight.skinny ?? character.creation.species?.weight.skinny) {
    categories.push("skinny");
  }
  if (character.creation.ethnicity?.weight.normal ?? character.creation.species?.weight.normal) {
    categories.push("normal");
  }
  if (character.creation.ethnicity?.weight.overweight ?? character.creation.species?.weight.overweight) {
    categories.push("overweight");
  }
  if (character.creation.ethnicity?.weight.obese ?? character.creation.species?.weight.obese) {
    categories.push("obese");
  }
  return categories.map((value) => ({ text: t(`lineages.physical.weight.${value}`), value }));
});
const weightRoll = computed<string | null | undefined>(() => {
  switch (weightCategory.value) {
    case "malnutrition":
      return character.creation.ethnicity?.weight.malnutrition ?? character.creation.species?.weight.malnutrition;
    case "skinny":
      return character.creation.ethnicity?.weight.skinny ?? character.creation.species?.weight.skinny;
    case "normal":
      return character.creation.ethnicity?.weight.normal ?? character.creation.species?.weight.normal;
    case "overweight":
      return character.creation.ethnicity?.weight.overweight ?? character.creation.species?.weight.overweight;
    case "obese":
      return character.creation.ethnicity?.weight.obese ?? character.creation.species?.weight.obese;
  }
});

function randomizeHeight(): void {
  height.value = roll(heightRoll.value ?? "");
}

function randomizeWeight(): void {
  bodyMassIndex.value = roll(weightRoll.value ?? "");
}

function updateWeightCategory(category: string | undefined): void {
  weightCategory.value = category ?? "";
  randomizeWeight();
}

const { handleSubmit } = useForm();
function submit(): void {
  console.log("Submitting!"); // TODO(fpion): implement
}

onMounted(() => {
  // TODO(fpion): implement

  randomizeHeight();

  if (weightOptions.value.some((option) => option.value === "normal")) {
    updateWeightCategory("normal");
  } else if (weightOptions.value.length === 1) {
    updateWeightCategory(weightOptions.value[0]?.value ?? "");
  }
});
</script>
