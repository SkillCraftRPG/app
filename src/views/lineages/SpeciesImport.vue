<template>
  <main class="container page" :aria-busy="isImporting">
    <div v-if="hasLoaded">
      <ImportProgress v-if="data.length" :count="count" :index="index">
        <h1>{{ title }}</h1>
        <WorldBreadcrumb :current="t('import.label')" :parent="breadcrumb" />
        <div class="d-flex flex-column flex-sm-row justify-content-between align-items-sm-start gap-3">
          <p class="text-body-secondary">{{ help }}</p>
          <TarButton v-if="canImport" class="mb-3" :icon="selectIcon" outline :text="selectText" @click="allSelected ? unselectAll() : selectAll()" />
        </div>
        <div class="row">
          <div v-for="item in data" :key="item.reference.id" class="col-md-6 col-lg-4 col-xl-3 mb-3">
            <LineageImportCard class="h-100" :lineage="item.reference" :selected="item.selected" :status="item.status" @click="toggle(item)" />
          </div>
        </div>
        <div v-if="canImport" class="d-flex flex-column flex-sm-row justify-content-between align-items-sm-start gap-3">
          <p>{{ t("import.selected", selectedCount) }}</p>
          <TarButton :disabled="!anySelected" icon="fas fa-download" size="large" :text="t('actions.import')" @click="onImport" />
        </div>
      </ImportProgress>
      <div v-else>
        <h1>{{ title }}</h1>
        <WorldBreadcrumb :current="t('import.label')" :parent="breadcrumb" />
        <p>{{ t("lineages.species.import.empty") }}</p>
      </div>
    </div>
    <LoadingSpinner v-else />
  </main>
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed, inject, onMounted, ref, watchEffect } from "vue";
import { useI18n } from "vue-i18n";

import ImportProgress from "@/components/import/ImportProgress.vue";
import LineageImportCard from "@/components/lineages/LineageImportCard.vue";
import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import TarButton from "@/components/tar/TarButton.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { Breadcrumb } from "@/types/tar/breadcrumb";
import type { ImportData, ImportStatus } from "@/types/import";
import type { Language } from "@/types/languages";
import type { Lineage } from "@/types/lineages";
import type { Script } from "@/types/scripts";
import { compareLineages } from "@/utils/compare";
import { getCompendiumSpecies } from "@/api/compendium";
import { handleErrorKey } from "@/inject";
import { listLanguages, saveLanguage } from "@/api/languages";
import { listSpecies, saveLineage } from "@/api/lineages";
import { listScripts, saveScript } from "@/api/scripts";
import { useDocument } from "@/composables/document";
import { useToastStore } from "@/stores/toast";

const document = useDocument();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const toasts = useToastStore();
const { orderBy } = arrayUtils;
const { t } = useI18n();

const count = ref<number>(0);
const data = ref<ImportData<Lineage>[]>([]);
const hasLoaded = ref<boolean>(false);
const index = ref<number>(0);
const languages = ref<Map<string, Language>>(new Map());
const scripts = ref<Map<string, Script>>(new Map());

const breadcrumb = computed<Breadcrumb>(() => ({ text: t("lineages.species.title"), to: { name: "Lineages" } }));
const canImport = computed<boolean>(() => data.value.some(({ status }) => status !== "UpToDate"));
const help = computed<string>(() => t(canImport.value ? "lineages.species.import.help" : "import.upToDate"));
const isImporting = computed<boolean>(() => Boolean(count.value));
const title = computed<string>(() => t("lineages.species.import.title"));

const allSelected = computed<boolean>(() => data.value.every(({ selected, status }) => selected || status === "UpToDate"));
const anySelected = computed<boolean>(() => data.value.some(({ selected }) => selected));
const selectedCount = computed<number>(() => data.value.filter(({ selected }) => selected).length);
const selectIcon = computed<string>(() => (allSelected.value ? "far fa-square" : "far fa-square-check"));
const selectText = computed<string>(() => t(`actions.${allSelected.value ? "unselectAll" : "selectAll"}`));

function selectAll(): void {
  data.value.forEach((item) => {
    if (item.status !== "UpToDate") {
      item.selected = true;
    }
  });
}
function unselectAll(): void {
  data.value.forEach((item) => (item.selected = false));
}
function toggle(item: ImportData<Lineage>): void {
  if (item.selected) {
    item.selected = false;
  } else if (item.status !== "UpToDate") {
    item.selected = true;
  }
}

async function importLanguage(language: Language): Promise<void> {
  if (language.script && !scripts.value.has(language.script.id)) {
    const script: Script = await saveScript(language.script);
    scripts.value.set(script.id, script);
  }
  if (!languages.value.has(language.id)) {
    const saved: Language = await saveLanguage(language);
    languages.value.set(saved.id, saved);
  }
}

async function importSpecies(lineage: Lineage): Promise<void> {
  for (const language of lineage.languages.granted) {
    await importLanguage(language);
  }
  await saveLineage(lineage);
}

async function onImport(): Promise<void> {
  if (!isImporting.value) {
    try {
      count.value = selectedCount.value;
      for (const item of data.value) {
        if (item.selected) {
          await importSpecies(item.reference);
          index.value++;
        }
      }
      await refresh();
      toasts.success("import.success");
    } catch (e: unknown) {
      handleError(e);
    } finally {
      index.value = 0;
      count.value = 0;
    }
  }
}

watchEffect(() => document.setTitle(title.value));

async function refresh(): Promise<void> {
  try {
    const [compendium, existing, existingLanguages, existingScripts] = await Promise.all([
      getCompendiumSpecies(),
      listSpecies(),
      listLanguages(),
      listScripts(),
    ]);

    const species: Map<string, Lineage> = new Map();
    existing.items.forEach((lineage) => species.set(lineage.id, lineage));

    data.value = [];
    orderBy(compendium.items, "name").forEach((reference) => {
      const existing: Lineage | undefined = species.get(reference.id);
      const status: ImportStatus = existing ? (compareLineages(existing, reference) ? "UpToDate" : "Outdated") : "NotImported";
      data.value.push({ existing, reference, selected: false, status });
    });

    languages.value.clear();
    existingLanguages.items.forEach((language) => languages.value.set(language.id, language));

    scripts.value.clear();
    existingScripts.items.forEach((script) => scripts.value.set(script.id, script));
  } catch (e: unknown) {
    handleError(e);
  } finally {
    hasLoaded.value = true;
  }
}
onMounted(refresh);
</script>
