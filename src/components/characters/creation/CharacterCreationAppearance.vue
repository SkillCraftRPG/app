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
            <template #append>
              <TarButton v-if="heightRoll" icon="fas fa-dice" :text="heightRoll" @click="randomizeHeight" />
            </template>
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
            <div>{{ n(calculateWeight(height, bodyMassIndex), "characterWeight") }}&nbsp;{{ t("game.unit.kg") }}</div>
          </div>
        </div>
      </div>
    </section>
    <section>
      <h3 class="h5">{{ t("lineages.physical.age.title") }}</h3>
      <div class="row">
        <div class="col-md-6">
          <TarSelect
            class="mb-3"
            :disabled="!ageThresholds.length"
            floating
            id="age-category"
            :label="t('lineages.physical.age.category.label')"
            :model-value="ageCategory"
            :options="ageOptions"
            :placeholder="t('lineages.physical.age.category.placeholder')"
            @update:model-value="updateAgeCategory"
          />
        </div>
        <div class="col-md-6">
          <AgeField class="mb-3" v-model="age">
            <template #append>
              <TarButton v-if="ageLimits.length" icon="fas fa-dice" :text="ageLimits.join('–')" @click="randomizeAge" />
            </template>
          </AgeField>
        </div>
      </div>
    </section>
    <section>
      <h3 class="h5">{{ t("characters.appearance.colors") }}</h3>
      <div class="row">
        <div class="col-md-4">
          <SkinField class="mb-3" v-model="skin" />
        </div>
        <div class="col-md-4">
          <EyesField class="mb-3" v-model="eyes" />
        </div>
        <div class="col-md-4">
          <HairField class="mb-3" v-model="hair" />
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

import AgeField from "@/components/lineages/AgeField.vue";
import EyesField from "@/components/characters/EyesField.vue";
import HairField from "@/components/characters/HairField.vue";
import HeightField from "@/components/characters/HeightField.vue";
import InputField from "@/components/forms/InputField.vue";
import SkinField from "@/components/characters/SkinField.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarSelect from "@/components/tar/TarSelect.vue";
import type { CharacterAppearanceDetail } from "@/types/characters";
import type { LineageAge } from "@/types/lineages";
import type { SelectOption } from "@/types/tar/select";
import type { SizeCategory } from "@/types/game";
import { calculateWeight } from "@/utils/character";
import { roll } from "@/utils/random";
import { useCharacterStore } from "@/stores/character";
import { useForm } from "@/forms";

const character = useCharacterStore();
const { parseNumber } = parsingUtils;
const { n, t } = useI18n();

defineEmits<{
  (e: "abandon"): void;
}>();

const age = ref<number>(0);
const ageCategory = ref<string>("");
const bodyMassIndex = ref<number>(0);
const eyes = ref<string>("");
const hair = ref<string>("");
const height = ref<number>(0);
const skin = ref<string>("");
const weightCategory = ref<string>("");

const heightRoll = computed<string | null | undefined>(() => character.creation.ethnicity?.size.height ?? character.creation.species?.size.height);
const sizeCategory = computed<string>(() => {
  const category: SizeCategory = character.creation.ethnicity?.size.category ?? character.creation.species?.size.category ?? "Medium";
  return t(`game.size.category.options.${category}`);
});

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

const ageLimits = computed<number[]>(() => {
  switch (ageCategory.value) {
    case "child":
      const threshold: number = (ageThresholds.value[0] ?? 0) - 1;
      if (threshold) {
        return [0, threshold];
      }
    case "teenager":
      return [ageThresholds.value[0] ?? 0, (ageThresholds.value[1] ?? 0) - 1];
    case "adult":
      return [ageThresholds.value[1] ?? 0, (ageThresholds.value[2] ?? 0) - 1];
    case "mature":
      return [ageThresholds.value[2] ?? 0, (ageThresholds.value[3] ?? 0) - 1];
  }
  return [];
});
const ageOptions = computed<SelectOption[]>(() => {
  if (ageThresholds.value.length !== 4) {
    return [];
  }
  const values: string[] = ["child", "teenager", "adult", "mature", "venerable"];
  const options: SelectOption[] = [];
  for (let i = 0; i < values.length; i++) {
    const value: string = values[i] ?? "";
    const category: string = t(`lineages.physical.age.${value}`);
    let limits: string = "";
    switch (i) {
      case 0:
        const age: string = t("lineages.physical.age.format", ageThresholds.value[0] ?? 0);
        limits = t("lineages.physical.age.less", { age });
        break;
      case 1:
      case 2:
      case 3:
        limits = t("lineages.physical.age.between", { min: ageThresholds.value[i - 1], max: (ageThresholds.value[i] ?? 0) - 1 });
        break;
      case 4:
        limits = t("lineages.physical.age.more", { age: ageThresholds.value[3] });
        break;
    }
    options.push({ text: `${category} (${limits})`, value });
  }
  return options;
});
const ageThresholds = computed<number[]>(() => {
  let age: LineageAge | undefined = character.creation.ethnicity?.age;
  if (age?.teenager && age.adult && age.mature && age.venerable) {
    return [age.teenager, age.adult, age.mature, age.venerable];
  }
  age = character.creation.species?.age;
  if (age?.teenager && age.adult && age.mature && age.venerable) {
    return [age.teenager, age.adult, age.mature, age.venerable];
  }
  return [];
});

function randomizeAge(): void {
  if (ageLimits.value.length === 2) {
    const diff: number = (ageLimits.value[1] ?? 0) - (ageLimits.value[0] ?? 0) + 1;
    age.value = (ageLimits.value[0] ?? 0) + Math.floor(Math.random() * diff);
  } else {
    age.value = 0;
  }
}

function randomizeHeight(): void {
  height.value = roll(heightRoll.value ?? "");
}

function randomizeWeight(): void {
  bodyMassIndex.value = roll(weightRoll.value ?? "");
}

function updateAgeCategory(category: string | undefined): void {
  ageCategory.value = category ?? "";
  randomizeAge();
}

function updateWeightCategory(category: string | undefined): void {
  weightCategory.value = category ?? "";
  randomizeWeight();
}

const { handleSubmit } = useForm();
function submit(): void {
  const appearance: CharacterAppearanceDetail = {
    height: height.value,
    weightCategory: weightCategory.value,
    bodyMassIndex: bodyMassIndex.value,
    age: age.value,
    skin: skin.value,
    eyes: eyes.value,
    hair: hair.value,
  };
  character.saveAppearance(appearance);
}

onMounted(() => {
  const appearance: CharacterAppearanceDetail = character.creation.appearance;

  if (appearance.height) {
    height.value = appearance.height;
  } else {
    randomizeHeight();
  }

  if (appearance.weightCategory && appearance.bodyMassIndex) {
    weightCategory.value = appearance.weightCategory;
    bodyMassIndex.value = appearance.bodyMassIndex;
  } else if (weightOptions.value.some((option) => option.value === "normal")) {
    updateWeightCategory("normal");
  } else if (weightOptions.value.length === 1) {
    updateWeightCategory(weightOptions.value[0]?.value);
  }

  if (appearance.age) {
    for (let i = 0; i < ageThresholds.value.length; i++) {
      if (appearance.age < (ageThresholds.value[i] ?? 0)) {
        ageCategory.value = ageOptions.value[i]?.value ?? "";
        break;
      }
    }
    if (!ageCategory.value) {
      ageCategory.value = "venerable";
    }
    age.value = appearance.age;
  } else if (ageOptions.value.some((option) => option.value === "adult")) {
    updateAgeCategory("adult");
  } else if (ageOptions.value.length === 1) {
    updateAgeCategory(ageOptions.value[0]?.value);
  }

  skin.value = character.creation.appearance.skin;
  eyes.value = character.creation.appearance.eyes;
  hair.value = character.creation.appearance.hair;
});
</script>
