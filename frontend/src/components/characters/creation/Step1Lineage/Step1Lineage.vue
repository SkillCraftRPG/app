<script setup lang="ts">
import { TarButton, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import AgeCategorySelect from "@/components/game/AgeCategorySelect.vue";
import AgeRollInput from "@/components/characters/AgeRollInput.vue";
import AppSelect from "@/components/shared/AppSelect.vue";
import ExtraLanguagesInput from "@/components/lineages/ExtraLanguagesInput.vue";
import HeightRollInput from "@/components/characters/HeightRollInput.vue";
import LanguageCard from "@/components/languages/LanguageCard.vue";
import LanguageSelect from "@/components/languages/LanguageSelect.vue";
import LineageDetail from "./LineageDetail.vue";
import LineageNames from "./LineageNames.vue";
import LineageSelect from "@/components/lineages/LineageSelect.vue";
import LineageSpeeds from "./LineageSpeeds.vue";
import MarkdownText from "@/components/shared/MarkdownText.vue";
import NameInput from "@/components/shared/NameInput.vue";
import SizeCategorySelect from "@/components/game/SizeCategorySelect.vue";
import WeightCategorySelect from "@/components/game/WeightCategorySelect.vue";
import WeightInput from "@/components/characters/WeightInput.vue";
import type { AgeCategory, SizeCategory } from "@/types/game";
import type { LanguageModel } from "@/types/languages";
import type { LineageModel, SearchLineagesPayload, WeightCategory } from "@/types/lineages";
import type { SearchResults } from "@/types/search";
import type { Step1 } from "@/types/characters";
import { readLineage, searchLineages } from "@/api/lineages";
import { useCharacterStore } from "@/stores/character";

type SpokenLanguage = {
  language: LanguageModel;
  source: "extra" | "lineage";
};

const character = useCharacterStore();
const { orderBy } = arrayUtils;
const { t } = useI18n();

const age = ref<number>(0);
const ageCategory = ref<AgeCategory | undefined>("Adult");
const height = ref<number>(0);
const isLoading = ref<boolean>(false);
const language = ref<LanguageModel>();
const languages = ref<LanguageModel[]>([]);
const name = ref<string>("");
const nation = ref<LineageModel>();
const nations = ref<LineageModel[]>([]);
const player = ref<string>("");
const species = ref<LineageModel>();
const weight = ref<number>(0);
const weightCategory = ref<WeightCategory | undefined>("Normal");

const ageRange = computed<number[]>(() => {
  let lower: number = 1;
  let upper: number = 0;
  if (ageCategory.value) {
    switch (ageCategory.value) {
      case "Child":
        upper = (nation.value?.ages.adolescent ?? species.value?.ages.adolescent ?? 1) - 1;
        break;
      case "Adolescent":
        lower = nation.value?.ages.adolescent ?? species.value?.ages.adolescent ?? 0;
        upper = (nation.value?.ages.adult ?? species.value?.ages.adult ?? 1) - 1;
        break;
      case "Adult":
        lower = nation.value?.ages.adult ?? species.value?.ages.adult ?? 0;
        upper = (nation.value?.ages.mature ?? species.value?.ages.mature ?? 1) - 1;
        break;
      case "Mature":
        lower = nation.value?.ages.mature ?? species.value?.ages.mature ?? 0;
        upper = (nation.value?.ages.venerable ?? species.value?.ages.venerable ?? 1) - 1;
        break;
      case "Venerable":
        lower = nation.value?.ages.venerable ?? species.value?.ages.venerable ?? 0;
        upper = Math.pow(10, lower.toString().length + 1) - 1;
        break;
      default:
        throw new Error(`The age category "${ageCategory.value}" is not supported.`);
    }
  }
  return [lower, upper];
});
const excludedLanguages = computed<LanguageModel[]>(() => lineageLanguages.value.concat(languages.value));
const extraLanguages = computed<number>(() => (species.value?.languages.extra ?? 0) + (nation.value?.languages.extra ?? 0));
const isCompleted = computed<boolean>(() => requiredLanguages.value === 0);
const lineageLanguages = computed<LanguageModel[]>(() => (species.value?.languages.items ?? []).concat(nation.value?.languages.items ?? []));
const nationOptions = computed<SelectOption[]>(() => nations.value.map(({ id, name }) => ({ text: name, value: id })));
const requiredLanguages = computed<number>(() => extraLanguages.value - languages.value.length);
const sizeCategory = computed<SizeCategory>(() => nation.value?.size.category ?? species.value?.size.category ?? "Medium");
const sizeRoll = computed<string | undefined>(() => nation.value?.size.roll ?? species.value?.size.roll);
const spokenLanguages = computed<SpokenLanguage[]>(() => {
  const spokenLanguages: SpokenLanguage[] = [];
  spokenLanguages.push(...orderBy(lineageLanguages.value, "name").map((language) => ({ language, source: "lineage" }) as SpokenLanguage));
  spokenLanguages.push(...languages.value.map((language) => ({ language, source: "extra" }) as SpokenLanguage));
  return spokenLanguages;
});
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
  (e: "error", value: unknown): void;
}>();

function addLanguage(): void {
  if (language.value) {
    languages.value.push(language.value);
    language.value = undefined;
  }
}
function removeLanguage(language: LanguageModel): void {
  const index: number = languages.value.findIndex(({ id }) => id === language.id);
  if (index >= 0) {
    languages.value.splice(index, 1);
  }
}

async function setNation(id?: string): Promise<void> {
  languages.value = [];
  if (id) {
    isLoading.value = true;
    try {
      nation.value = await readLineage(id);
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  } else {
    nation.value = undefined;
  }
}
async function setSpecies(value?: LineageModel): Promise<void> {
  languages.value = [];
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

function onAbandon(): void {
  character.reset();
  emit("abandon");
}

const { handleSubmit } = useForm();
const onSubmit = handleSubmit(() => {
  if (species.value) {
    const payload: Step1 = {
      name: name.value,
      player: player.value,
      species: species.value,
      nation: nation.value,
      height: height.value,
      weight: weight.value,
      age: age.value,
      languages: languages.value,
    };
    character.setStep1(payload);
    character.next();
  }
});

onMounted(() => {
  const step1: Step1 | undefined = character.creation.step1;
  if (step1) {
    name.value = step1.name;
    player.value = step1.player ?? "";
    setSpecies(step1.species);
    setNation(step1.nation?.id);
    height.value = step1.height;
    weight.value = step1.weight;
    age.value = step1.age;
    languages.value = [...step1.languages];
  }
});
</script>

<template>
  <div>
    <h3>{{ t("characters.steps.lineage") }}</h3>
    <form @submit="onSubmit">
      <div class="row">
        <div class="col">
          <LineageSelect
            id="species"
            label="lineages.species.label"
            :model-value="species?.id"
            placeholder="lineages.species.placeholder"
            required
            @error="$emit('error', $event)"
            @selected="setSpecies"
          />
          <MarkdownText v-if="species?.description" :text="species.description" />
        </div>
        <div v-if="nations.length > 0" class="col">
          <AppSelect
            floating
            id="nation"
            label="lineages.nation.label"
            :model-value="nation?.id"
            :options="nationOptions"
            placeholder="lineages.nation.placeholder"
            required
            @update:model-value="setNation"
          />
          <MarkdownText v-if="nation?.description" :text="nation.description" />
        </div>
      </div>
      <template v-if="species">
        <LineageDetail :lineage="nation ?? species" />
        <h5>{{ t("characters.name") }}</h5>
        <div class="row">
          <NameInput class="col" required v-model="name" />
          <NameInput class="col" id="player" label="characters.player.label" placeholder="characters.player.label" v-model="player" />
        </div>
        <LineageNames :lineage="nation ?? species" />
        <LineageSpeeds :lineage="nation ?? species" />
        <h5>{{ t("game.size.label") }}</h5>
        <div class="row">
          <SizeCategorySelect class="col" disabled :model-value="sizeCategory" validation="server" />
          <HeightRollInput class="col" :roll="sizeRoll" v-model="height" />
        </div>
        <h5>{{ t("game.weight.label") }}</h5>
        <div class="row">
          <WeightCategorySelect class="col" v-model="weightCategory" />
          <WeightInput class="col" :height="height" :roll="weightRoll" v-model="weight" />
        </div>
        <h5>{{ t("game.age.label") }}</h5>
        <div class="row">
          <AgeCategorySelect class="col" v-model="ageCategory" />
          <AgeRollInput class="col" :range="ageRange" v-model="age" />
        </div>
        <h5>{{ t("languages.list") }}</h5>
        <div v-if="extraLanguages > 0" class="row">
          <LanguageSelect
            class="col"
            :disabled="requiredLanguages === 0"
            :exclude="excludedLanguages"
            :model-value="language?.id"
            validation="server"
            @selected="language = $event"
          >
            <template #append>
              <TarButton :disabled="!language" icon="fas fa-plus" :text="t('actions.add')" variant="success" @click="addLanguage" />
            </template>
          </LanguageSelect>
          <ExtraLanguagesInput class="col" disabled :model-value="extraLanguages" validation="server" />
        </div>
        <p v-if="requiredLanguages !== 0" class="text-danger">
          <font-awesome-icon icon="fas fa-triangle-exclamation" /> {{ t("characters.languages.selectExtra", { n: requiredLanguages }) }}
        </p>
        <div class="mb-3 row">
          <div v-for="language in spokenLanguages" :key="language.language.id" class="col-lg-3">
            <LanguageCard :language="language.language" :remove="language.source === 'extra'" view @removed="removeLanguage(language.language)" />
          </div>
        </div>
      </template>
      <TarButton class="me-1" icon="fas fa-ban" :text="t('actions.abandon')" variant="danger" @click="onAbandon" />
      <TarButton class="ms-1" :disabled="isLoading || !isCompleted" icon="fas fa-arrow-right" :text="t('actions.continue')" type="submit" />
    </form>
  </div>
</template>
