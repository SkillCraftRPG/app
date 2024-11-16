<script setup lang="ts">
import { TarButton, type SelectOption } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import AgeCategorySelect from "@/components/game/AgeCategorySelect.vue";
import AgeRollInput from "./AgeRollInput.vue";
import AppSelect from "@/components/shared/AppSelect.vue";
import HeightRollInput from "./HeightRollInput.vue";
import LineageDetail from "./LineageDetail.vue";
import LineageNames from "./LineageNames.vue";
import LineageSelect from "@/components/lineages/LineageSelect.vue";
import LineageSpeeds from "./LineageSpeeds.vue";
import MarkdownText from "@/components/shared/MarkdownText.vue";
import NameInput from "@/components/shared/NameInput.vue";
import SizeCategorySelect from "@/components/game/SizeCategorySelect.vue";
import WeightCategorySelect from "@/components/game/WeightCategorySelect.vue";
import WeightRollInput from "./WeightRollInput.vue";
import type { AgeCategory, LineageModel, SearchLineagesPayload, WeightCategory } from "@/types/lineages";
import type { LanguageModel } from "@/types/languages";
import type { SearchResults } from "@/types/search";
import type { SizeCategory } from "@/types/game";
import type { Step1 } from "@/types/characters";
import { readLineage, searchLineages } from "@/api/lineages";

const { t } = useI18n();

const age = ref<number>(0);
const ageCategory = ref<AgeCategory | undefined>("Adult");
const height = ref<number>(0);
const isLoading = ref<boolean>(false);
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

async function setNation(id?: string): Promise<void> {
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
    name: name.value,
    player: player.value,
    species: species.value,
    nation: nation.value,
    height: height.value,
    weight: weight.value,
    age: age.value,
    languages: languages.value,
  }),
);

/* TODO(fpion):
 * - Calculer le nombre de extraLanguages
 * - Si extraLanguages > 0, afficher en row un sélecteur de langue, un bouton d'ajout et le nombre de extra languages
 * - Exclure les langues de la lignée et les langues sélectionnées du sélecteur
 * - Si extraLanguages === languages.length, disable le sélecteur de langue
 * - Afficher les langues de la lignée en ordre alphabétique, permettre la vue seulement
 * - Afficher les langues supplémentaires en ordre de sélection, permettre la vue et le retrait
 * - Empêcher la soumission si extraLanguages !== languages.length
 */
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
          <NameInput class="col" id="player" label="characters.player" placeholder="characters.player" v-model="player" />
        </div>
        <LineageNames :lineage="nation ?? species" />
        <LineageSpeeds :lineage="nation ?? species" />
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
        <h5>{{ t("characters.age") }}</h5>
        <div class="row">
          <AgeCategorySelect class="col" v-model="ageCategory" />
          <AgeRollInput class="col" :range="ageRange" v-model="age" />
        </div>
        <h5>{{ t("characters.languages") }}</h5>
        <!-- TODO(fpion): Languages -->
      </template>
      <TarButton class="me-1" icon="fas fa-ban" :text="t('actions.abandon')" variant="danger" @click="$emit('abandon')" />
      <TarButton class="ms-1" :disabled="isLoading" icon="fas fa-arrow-right" :text="t('actions.continue')" type="submit" />
    </form>
  </div>
</template>
