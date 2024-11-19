<script setup lang="ts">
import { TarTab, TarTabs } from "logitar-vue3-ui";
import { computed, inject, ref, watch } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AgesEdit from "@/components/lineages/AgesEdit.vue";
import AppBreadcrumb from "@/components/shared/AppBreadcrumb.vue";
import AttributeBonuses from "@/components/lineages/AttributeBonuses.vue";
import BackButton from "@/components/shared/BackButton.vue";
import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import NameInput from "@/components/shared/NameInput.vue";
import NamesEdit from "@/components/lineages/NamesEdit.vue";
import NationList from "@/components/lineages/NationList.vue";
import SaveButton from "@/components/shared/SaveButton.vue";
import SizeEdit from "@/components/lineages/SizeEdit.vue";
import SpeedsEdit from "@/components/lineages/SpeedsEdit.vue";
import SpokenLanguages from "@/components/lineages/SpokenLanguages.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import TraitList from "@/components/lineages/TraitList.vue";
import WeightEdit from "@/components/lineages/WeightEdit.vue";
import type { ApiError } from "@/types/api";
import type { Breadcrumb } from "@/types/components";
import type {
  AgesModel,
  AttributeBonusesModel,
  CreateOrReplaceLineagePayload,
  LanguagesPayload,
  LineageModel,
  NamesModel,
  SizeModel,
  SpeedsModel,
  TraitStatus,
  WeightModel,
} from "@/types/lineages";
import { handleErrorKey } from "@/inject/App";
import { readLineage, replaceLineage } from "@/api/lineages";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const ages = ref<AgesModel>({});
const attributes = ref<AttributeBonusesModel>({ agility: 0, coordination: 0, intellect: 0, presence: 0, sensitivity: 0, spirit: 0, vigor: 0, extra: 0 });
const description = ref<string>("");
const languages = ref<LanguagesPayload>({ ids: [], extra: 0 });
const languagesReference = ref<LanguagesPayload>({ ids: [], extra: 0 });
const lineage = ref<LineageModel>();
const name = ref<string>("");
const names = ref<NamesModel>({ family: [], female: [], male: [], unisex: [], custom: [] });
const size = ref<SizeModel>({ category: "Medium" });
const speeds = ref<SpeedsModel>({ walk: 0, climb: 0, swim: 0, fly: 0, hover: 0, burrow: 0 });
const traits = ref<TraitStatus[]>([]);
const weight = ref<WeightModel>({});

const hasChanges = computed<boolean>(
  () =>
    !!lineage.value &&
    (name.value !== lineage.value.name ||
      description.value !== (lineage.value.description ?? "") ||
      JSON.stringify(attributes.value) !== JSON.stringify(lineage.value.attributes) ||
      traits.value.some((trait) => !trait.trait.id || trait.isRemoved || trait.isUpdated) ||
      JSON.stringify(languages.value) !== JSON.stringify(languagesReference.value) ||
      names.value.text !== (lineage.value.names.text ?? "") ||
      JSON.stringify(names.value.family) !== JSON.stringify(lineage.value.names.family) ||
      JSON.stringify(names.value.female) !== JSON.stringify(lineage.value.names.female) ||
      JSON.stringify(names.value.male) !== JSON.stringify(lineage.value.names.male) ||
      JSON.stringify(names.value.unisex) !== JSON.stringify(lineage.value.names.unisex) ||
      JSON.stringify(names.value.custom) !== JSON.stringify(lineage.value.names.custom) ||
      JSON.stringify(speeds.value) !== JSON.stringify(lineage.value.speeds) ||
      size.value.category !== lineage.value.size.category ||
      size.value.roll !== (lineage.value.size.roll ?? "") ||
      weight.value.starved !== (lineage.value.weight.starved ?? "") ||
      weight.value.skinny !== (lineage.value.weight.skinny ?? "") ||
      weight.value.normal !== (lineage.value.weight.normal ?? "") ||
      weight.value.overweight !== (lineage.value.weight.overweight ?? "") ||
      weight.value.obese !== (lineage.value.weight.obese ?? "") ||
      ages.value.adolescent !== (lineage.value.ages.adolescent ?? undefined) ||
      ages.value.adult !== (lineage.value.ages.adult ?? undefined) ||
      ages.value.mature !== (lineage.value.ages.mature ?? undefined) ||
      ages.value.venerable !== (lineage.value.ages.venerable ?? undefined)),
);
const parent = computed<Breadcrumb[]>(() => {
  const parent: Breadcrumb[] = [{ route: { name: "LineageList" }, text: t("lineages.list") }];
  if (lineage.value?.species) {
    parent.push({ route: { name: "LineageEdit", params: { id: lineage.value.species.id } }, text: lineage.value.species.name });
  }
  return parent;
});

function setModel(model: LineageModel): void {
  lineage.value = model;
  ages.value = {
    adolescent: model.ages.adolescent ?? undefined,
    adult: model.ages.adult ?? undefined,
    mature: model.ages.mature ?? undefined,
    venerable: model.ages.venerable ?? undefined,
  };
  attributes.value = { ...model.attributes };
  description.value = model.description ?? "";
  languages.value = { ids: model.languages.items.map(({ id }) => id), extra: model.languages.extra, text: model.languages.text ?? "" };
  languagesReference.value = { ids: model.languages.items.map(({ id }) => id), extra: model.languages.extra, text: model.languages.text ?? "" };
  name.value = model.name;
  names.value = {
    text: model.names.text ?? "",
    family: [...model.names.family],
    female: [...model.names.female],
    male: [...model.names.male],
    unisex: [...model.names.unisex],
    custom: JSON.parse(JSON.stringify(model.names.custom)),
  };
  size.value = { ...model.size, roll: model.size.roll ?? "" };
  speeds.value = { ...model.speeds };
  traits.value = model.traits.map((trait) => ({ trait, isRemoved: false, isUpdated: false }));
  weight.value = {
    starved: model.weight.starved ?? "",
    skinny: model.weight.skinny ?? "",
    normal: model.weight.normal ?? "",
    overweight: model.weight.overweight ?? "",
    obese: model.weight.obese ?? "",
  };
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  if (lineage.value) {
    try {
      const payload: CreateOrReplaceLineagePayload = {
        parentId: lineage.value.species?.id,
        name: name.value,
        description: description.value,
        attributes: attributes.value,
        traits: traits.value.filter(({ isRemoved }) => !isRemoved).map(({ trait }) => trait),
        languages: languages.value,
        names: names.value,
        speeds: speeds.value,
        size: size.value,
        weight: weight.value,
        ages: ages.value,
      };
      const model: LineageModel = await replaceLineage(lineage.value.id, payload, lineage.value.version);
      setModel(model);
      toasts.success("lineages.updated");
    } catch (e: unknown) {
      handleError(e);
    }
  }
});

watch(
  route,
  async (route) => {
    if (route.name === "LineageEdit") {
      try {
        const id = route.params.id?.toString();
        if (id) {
          const lineage: LineageModel = await readLineage(id);
          setModel(lineage);
          document.getElementById("tab_editing_head")?.click();
        }
      } catch (e: unknown) {
        const { status } = e as ApiError;
        if (status === 404) {
          router.push({ path: "/not-found" });
        } else {
          handleError(e);
        }
      }
    }
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <main class="container">
    <template v-if="lineage">
      <h1>{{ lineage.name }}</h1>
      <AppBreadcrumb :current="lineage.name" :parent="parent" :world="lineage.world" @error="handleError" />
      <StatusDetail :aggregate="lineage" />
      <TarTabs>
        <TarTab active id="editing" :title="t('editing')">
          <form @submit.prevent="onSubmit">
            <NameInput required v-model="name" />
            <DescriptionTextarea v-model="description" />
            <AttributeBonuses v-model="attributes" />
            <TraitList v-model="traits" />
            <SpokenLanguages :languages="lineage.languages.items" v-model="languages" />
            <NamesEdit v-model="names" />
            <SpeedsEdit v-model="speeds" />
            <SizeEdit v-model="size" />
            <WeightEdit v-model="weight" />
            <AgesEdit v-model="ages" />
            <div>
              <SaveButton class="me-1" :disabled="isSubmitting || !hasChanges" :loading="isSubmitting" />
              <BackButton class="ms-1" :has-changes="hasChanges" />
            </div>
          </form>
        </TarTab>
        <TarTab v-if="!lineage.species" id="nations" :title="t('lineages.nations.label')">
          <NationList :species="lineage" @error="handleError" />
        </TarTab>
      </TarTabs>
    </template>
  </main>
</template>
