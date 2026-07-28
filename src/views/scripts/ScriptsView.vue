<template>
  <main class="container page">
    <div class="d-flex flex-column flex-sm-row justify-content-between align-items-sm-start gap-3">
      <h1 class="mb-0">{{ title }}</h1>
      <CreateScript class="mb-3" @created="onCreate" @error="handleError" />
    </div>
    <WorldBreadcrumb :current="title" />
    <section class="mb-3">
      <TarButton
        :disabled="isLoading"
        icon="fas fa-arrows-rotate"
        :loading="isLoading"
        :status="t('loading')"
        :text="t('actions.refresh')"
        variant="secondary"
        @click="refresh"
      />
    </section>
    <section class="row mb-3">
      <SearchInput class="col" :model-value="search" @update:model-value="setQuery('search', $event)" />
      <SortSelect
        class="col"
        :descending="isDescending"
        :model-value="sort"
        :options="sortOptions"
        @descending="setQuery('descending', $event)"
        @update:model-value="setQuery('sort', $event)"
      />
      <CountSelect class="col" :model-value="count" @update:model-value="setQuery('count', $event)" />
    </section>
    <section class="d-flex flex-column flex-grow-1">
      <div v-if="total" class="row">
        <div v-for="script in scripts" :key="script.id" class="col-sm-6 col-md-4 col-lg-3 mb-3">
          <ScriptCard class="d-flex flex-column h-100" :script="script" />
        </div>
      </div>
      <div v-else class="d-flex flex-column align-items-center justify-content-center text-center flex-grow-1 py-5">
        <font-awesome-icon icon="fas fa-magnifying-glass" class="display-4 text-body-secondary mb-3" aria-hidden="true" />
        <h2 class="h4 mb-2">{{ t("empty.lead") }}</h2>
        <p class="text-body-secondary mb-0">{{ t("empty.help") }}</p>
      </div>
    </section>
    <section v-if="total">
      <SearchPagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event)" />
    </section>
  </main>
</template>

<script setup lang="ts">
import { arrayUtils, objectUtils, parsingUtils } from "logitar-js";
import { computed, inject, ref, watch, watchEffect } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import CountSelect from "@/components/shared/CountSelect.vue";
import CreateScript from "@/components/scripts/CreateScript.vue";
import ScriptCard from "@/components/scripts/ScriptCard.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SearchPagination from "@/components/shared/SearchPagination.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import TarButton from "@/components/tar/TarButton.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { Script, ScriptSort, SearchScriptsPayload } from "@/types/scripts";
import type { SearchResults } from "@/types/search";
import type { SelectOption } from "@/types/tar/select";
import { handleErrorKey } from "@/inject";
import { searchScripts } from "@/api/scripts";
import { useDocument } from "@/composables/document";
import { useEventStore } from "@/stores/event";

const document = useDocument();
const events = useEventStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const { isEmpty } = objectUtils;
const { orderBy } = arrayUtils;
const { parseBoolean, parseNumber } = parsingUtils;
const { rt, t, tm } = useI18n();

const isLoading = ref<boolean>(false);
const scripts = ref<Script[]>([]);
const timestamp = ref<number>(0);
const total = ref<number>(0);

const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 10);
const isDescending = computed<boolean>(() => parseBoolean(route.query.descending?.toString()) ?? false);
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");
const title = computed<string>(() => t("scripts.title"));

const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("scripts.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

function onCreate(script: Script): void {
  events.push("created");
  router.push({ name: "Script", params: { id: script.id } });
}

function setQuery(key: string, value?: boolean | null | number | string): void {
  const query = { ...route.query, [key]: value?.toString() ?? "" };
  switch (key) {
    case "search":
    case "count":
      query.page = "1";
      break;
  }
  router.replace({ ...route, query });
}

async function refresh(): Promise<void> {
  const payload: SearchScriptsPayload = {
    ids: [],
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => term.length)
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    sort: sort.value ? [{ field: sort.value as ScriptSort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results: SearchResults<Script> = await searchScripts(payload);
    if (now === timestamp.value) {
      scripts.value = [...results.items];
      total.value = results.total;
    }
  } catch (e: unknown) {
    handleError(e);
  } finally {
    if (now === timestamp.value) {
      isLoading.value = false;
    }
  }
}

watch(
  () => route,
  (route) => {
    if (route.name === "Scripts") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: isEmpty(query)
            ? {
                search: "",
                sort: "UpdatedOn",
                descending: "true",
                page: 1,
                count: 10,
              }
            : {
                page: 1,
                count: 10,
                ...query,
              },
        });
      } else {
        refresh();
      }
    }
  },
  { deep: true, immediate: true },
);

watchEffect(() => document.setTitle(title.value));

// TODO(fpion): we could have a clear/reset filters button into the no-result section.
</script>
