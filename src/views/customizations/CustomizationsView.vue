<template>
  <main class="container page">
    <div v-if="hasLoaded" class="d-flex flex-column flex-grow-1">
      <div class="d-flex flex-column flex-sm-row justify-content-between align-items-sm-start gap-3">
        <h1 class="mb-0">{{ title }}</h1>
        <CreateCustomization class="mb-3" @created="onCreate" @error="handleError" />
      </div>
      <WorldBreadcrumb :current="title" />
      <section>
        <div class="d-flex gap-2 mb-3">
          <TarButton
            :disabled="isLoading"
            icon="fas fa-arrows-rotate"
            :loading="isLoading"
            :status="t('loading')"
            :text="t('actions.refresh')"
            variant="secondary"
            @click="refresh"
          />
          <TarButton v-if="hasFilters" icon="fas fa-arrow-rotate-left" outline :text="t('filters.clear')" variant="secondary" @click="clearFilters" />
        </div>
      </section>
      <section>
        <div class="row">
          <div class="col-md-6 col-lg-3">
            <CustomizationKindSelect :model-value="kind" @update:model-value="setQuery('kind', $event)" />
          </div>
          <div class="col-md-6 col-lg-3">
            <SearchInput class="mb-3" :model-value="search" @update:model-value="setQuery('search', $event)" />
          </div>
          <div class="col-md-6 col-lg-3">
            <SortSelect
              class="mb-3"
              :descending="isDescending"
              :model-value="sort"
              :options="sortOptions"
              @descending="setQuery('descending', $event)"
              @update:model-value="setQuery('sort', $event)"
            />
          </div>
          <div class="col-md-6 col-lg-3">
            <CountSelect class="mb-3" :model-value="count" @update:model-value="setQuery('count', $event)" />
          </div>
        </div>
      </section>
      <section v-if="total" class="border-top border-secondary-subtle pt-4" :class="{ loading: isLoading }">
        <div class="row">
          <div v-for="customization in customizations" :key="customization.id" class="col-sm-6 col-md-4 col-lg-3 mb-3">
            <CustomizationCard class="d-flex flex-column h-100" :customization="customization" />
          </div>
        </div>
        <SearchPagination v-if="total > count" class="mt-3" :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event)" />
      </section>
      <section v-else class="d-flex flex-column align-items-center justify-content-center text-center flex-grow-1 py-5" :class="{ loading: isLoading }">
        <font-awesome-icon icon="fas fa-magnifying-glass" class="display-4 text-body-secondary mb-3" aria-hidden="true" />
        <h2 class="h4 mb-2">{{ t("empty.lead") }}</h2>
        <p class="text-body-secondary mb-0">{{ t("empty.help") }}</p>
        <TarButton
          v-if="hasFilters"
          class="mt-3"
          icon="fas fa-arrow-rotate-left"
          outline
          :text="t('filters.clear')"
          variant="secondary"
          @click="clearFilters"
        />
      </section>
    </div>
    <LoadingSpinner v-else />
  </main>
</template>

<script setup lang="ts">
import { arrayUtils, objectUtils, parsingUtils } from "logitar-js";
import { computed, inject, ref, watch, watchEffect } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import CountSelect from "@/components/shared/CountSelect.vue";
import CreateCustomization from "@/components/customizations/CreateCustomization.vue";
import CustomizationCard from "@/components/customizations/CustomizationCard.vue";
import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SearchPagination from "@/components/shared/SearchPagination.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import TarButton from "@/components/tar/TarButton.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { Customization, CustomizationKind, CustomizationSort, SearchCustomizationsPayload } from "@/types/customizations";
import type { SearchResults } from "@/types/search";
import type { SelectOption } from "@/types/tar/select";
import { handleErrorKey } from "@/inject";
import { searchCustomizations } from "@/api/customizations";
import { useDocument } from "@/composables/document";
import { useEventStore } from "@/stores/event";
import CustomizationKindSelect from "@/components/customizations/CustomizationKindSelect.vue";

const document = useDocument();
const events = useEventStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const { isEmpty } = objectUtils;
const { orderBy } = arrayUtils;
const { parseBoolean, parseNumber } = parsingUtils;
const { rt, t, tm } = useI18n();

const hasLoaded = ref<boolean>(false);
const isLoading = ref<boolean>(false);
const customizations = ref<Customization[]>([]);
const timestamp = ref<number>(0);
const total = ref<number>(0);

const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 10);
const isDescending = computed<boolean>(() => parseBoolean(route.query.descending?.toString()) ?? false);
const kind = computed<string>(() => route.query.kind?.toString() ?? "");
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");
const title = computed<string>(() => t("customizations.title"));

const hasFilters = computed<boolean>(() => Boolean(kind.value || search.value));

const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("customizations.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

function onCreate(customization: Customization): void {
  events.push("created");
  router.push({ name: "Customization", params: { id: customization.id } });
}

function clearFilters(): void {
  const query = { ...route.query, kind: "", search: "", page: 1 };
  router.replace({ ...route, query });
}

function setQuery(key: string, value?: boolean | null | number | string): void {
  const query = { ...route.query, [key]: value?.toString() ?? "" };
  switch (key) {
    case "kind":
    case "search":
    case "count":
      query.page = "1";
      break;
  }
  router.replace({ ...route, query });
}

async function refresh(): Promise<void> {
  const payload: SearchCustomizationsPayload = {
    ids: [],
    kind: kind.value ? (kind.value as CustomizationKind) : undefined,
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => term.length)
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    sort: sort.value ? [{ field: sort.value as CustomizationSort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results: SearchResults<Customization> = await searchCustomizations(payload);
    if (now === timestamp.value) {
      customizations.value = [...results.items];
      total.value = results.total;
    }
  } catch (e: unknown) {
    handleError(e);
  } finally {
    if (now === timestamp.value) {
      isLoading.value = false;
    }
    hasLoaded.value = true;
  }
}

watch(
  () => route,
  (route) => {
    if (route.name === "Customizations") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: isEmpty(query)
            ? {
                kind: "",
                search: "",
                sort: "Name",
                descending: "false",
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
</script>
