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
            <CasteImportCard class="d-flex flex-column h-100" :caste="item.reference" :selected="item.selected" :status="item.status" @click="toggle(item)" />
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
        <p>{{ t("castes.import.empty") }}</p>
      </div>
    </div>
    <LoadingSpinner v-else />
  </main>
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed, inject, onMounted, ref, watchEffect } from "vue";
import { useI18n } from "vue-i18n";

import CasteImportCard from "@/components/castes/CasteImportCard.vue";
import ImportProgress from "@/components/import/ImportProgress.vue";
import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import TarButton from "@/components/tar/TarButton.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { Breadcrumb } from "@/types/tar/breadcrumb";
import type { Caste } from "@/types/castes";
import type { ImportData, ImportStatus } from "@/types/import";
import { compareCastes } from "@/utils/compare";
import { getCompendiumCastes } from "@/api/compendium";
import { handleErrorKey } from "@/inject";
import { listCastes, saveCaste } from "@/api/castes";
import { useDocument } from "@/composables/document";
import { useToastStore } from "@/stores/toast";

const document = useDocument();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const toasts = useToastStore();
const { orderBy } = arrayUtils;
const { t } = useI18n();

const count = ref<number>(0);
const data = ref<ImportData<Caste>[]>([]);
const hasLoaded = ref<boolean>(false);
const index = ref<number>(0);

const breadcrumb = computed<Breadcrumb>(() => ({ text: t("castes.title"), to: { name: "Castes" } }));
const canImport = computed<boolean>(() => data.value.some(({ status }) => status !== "UpToDate"));
const help = computed<string>(() => t(canImport.value ? "castes.import.help" : "import.upToDate"));
const isImporting = computed<boolean>(() => Boolean(count.value));
const title = computed<string>(() => t("castes.import.title"));

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
function toggle(item: ImportData<Caste>): void {
  if (item.selected) {
    item.selected = false;
  } else if (item.status !== "UpToDate") {
    item.selected = true;
  }
}

async function onImport(): Promise<void> {
  if (!isImporting.value) {
    try {
      count.value = selectedCount.value;
      for (const item of data.value) {
        if (item.selected) {
          await saveCaste(item.reference);
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
    const [compendium, existing] = await Promise.all([getCompendiumCastes(), listCastes()]);

    const castes: Map<string, Caste> = new Map();
    existing.items.forEach((caste) => castes.set(caste.id, caste));

    data.value = [];
    orderBy(compendium.items, "name").forEach((reference) => {
      const existing: Caste | undefined = castes.get(reference.id);
      const status: ImportStatus = existing ? (compareCastes(existing, reference) ? "UpToDate" : "Outdated") : "NotImported";
      data.value.push({ existing, reference, selected: false, status });
    });
  } catch (e: unknown) {
    handleError(e);
  } finally {
    hasLoaded.value = true;
  }
}
onMounted(refresh);
</script>
