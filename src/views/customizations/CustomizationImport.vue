<template>
  <main class="container page" :aria-busy="isImporting">
    <div v-if="hasLoaded">
      <ImportProgress v-if="data.length" :count="count" :index="index">
        <h1>{{ title }}</h1>
        <WorldBreadcrumb :current="t('import.label')" :parent="breadcrumb" />
        <p class="text-body-secondary">{{ help }}</p>
        <div v-if="canImport" class="d-flex justify-content-end mb-3">
          <TarButton :icon="selectIcon" outline :text="selectText" @click="onSelect" />
        </div>
        <div class="row">
          <div v-for="item in data" :key="item.reference.id" class="col-md-6 col-lg-4 col-xl-3 mb-3">
            <CustomizationImportCard
              class="d-flex flex-column h-100"
              :customization="item.reference"
              :selected="item.selected"
              :status="item.status"
              @click="onToggle(item)"
            />
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
        <p>{{ t("customizations.import.empty") }}</p>
      </div>
    </div>
    <LoadingSpinner v-else />
  </main>
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed, inject, onMounted, ref, watchEffect } from "vue";
import { useI18n } from "vue-i18n";

import CustomizationImportCard from "@/components/customizations/CustomizationImportCard.vue";
import ImportProgress from "@/components/import/ImportProgress.vue";
import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import TarButton from "@/components/tar/TarButton.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { Breadcrumb } from "@/types/tar/breadcrumb";
import type { CreateOrReplaceCustomizationPayload, Customization, SearchCustomizationsPayload } from "@/types/customizations";
import type { ImportData, ImportStatus } from "@/types/import";
import type { SearchResults } from "@/types/search";
import { compareCustomizations } from "@/utils/compare";
import { getCompendiumCustomizations } from "@/api/compendium";
import { handleErrorKey } from "@/inject";
import { replaceCustomization, searchCustomizations } from "@/api/customizations";
import { useDocument } from "@/composables/document";
import { useToastStore } from "@/stores/toast";

const document = useDocument();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const toasts = useToastStore();
const { orderBy } = arrayUtils;
const { t } = useI18n();

const count = ref<number>(0);
const data = ref<ImportData<Customization>[]>([]);
const hasLoaded = ref<boolean>(false);
const index = ref<number>(0);

const breadcrumb = computed<Breadcrumb>(() => ({ text: t("customizations.title"), to: { name: "Customizations" } }));
const canImport = computed<boolean>(() => data.value.some(({ status }) => status !== "UpToDate"));
const help = computed<string>(() => t(canImport.value ? "customizations.import.help" : "import.upToDate"));
const isImporting = computed<boolean>(() => Boolean(count.value));
const title = computed<string>(() => t("customizations.import.title"));

const allSelected = computed<boolean>(() => data.value.every(({ selected }) => selected));
const anySelected = computed<boolean>(() => data.value.some(({ selected }) => selected));
const selectedCount = computed<number>(() => data.value.filter(({ selected }) => selected).length);
const selectIcon = computed<string>(() => (allSelected.value ? "far fa-square" : "far fa-square-check"));
const selectText = computed<string>(() => t(`actions.${allSelected.value ? "unselectAll" : "selectAll"}`));

function onSelect(): void {
  const selected: boolean = !allSelected.value;
  data.value.forEach((item) => {
    if (!selected || item.status !== "UpToDate") {
      item.selected = selected;
    }
  });
}

function onToggle(item: ImportData<Customization>): void {
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
          const payload: CreateOrReplaceCustomizationPayload = {
            kind: item.reference.kind,
            name: item.reference.name,
            summary: item.reference.summary,
            content: item.reference.content,
          };
          await replaceCustomization(item.reference.id, payload);
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

async function loadCustomizations(): Promise<SearchResults<Customization>> {
  const payload: SearchCustomizationsPayload = {
    ids: [],
    search: { terms: [], operator: "And" },
    sort: [],
    skip: 0,
    limit: 0,
  };
  const results: SearchResults<Customization> = await searchCustomizations(payload);
  return results;
}
async function refresh(): Promise<void> {
  try {
    const [compendium, existing] = await Promise.all([getCompendiumCustomizations(), loadCustomizations()]);

    const customizations: Map<string, Customization> = new Map();
    existing.items.forEach((customization) => customizations.set(customization.id, customization));

    data.value = [];
    orderBy(compendium.items, "name").forEach((reference) => {
      const existing: Customization | undefined = customizations.get(reference.id);
      const status: ImportStatus = existing ? (compareCustomizations(existing, reference) ? "UpToDate" : "Outdated") : "NotImported";
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
