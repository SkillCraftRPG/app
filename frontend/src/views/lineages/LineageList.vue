<script setup lang="ts">
import { TarButton, parsingUtils, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils, objectUtils } from "logitar-js";
import { computed, inject, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppBreadcrumb from "@/components/shared/AppBreadcrumb.vue";
import AppPagination from "@/components/shared/AppPagination.vue";
import AttributeSelect from "@/components/game/AttributeSelect.vue";
import CountSelect from "@/components/shared/CountSelect.vue";
import CreateLineage from "@/components/lineages/CreateLineage.vue";
import LanguageSelect from "@/components/languages/LanguageSelect.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SizeCategorySelect from "@/components/game/SizeCategorySelect.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Attribute, SizeCategory } from "@/types/game";
import type { LineageModel, LineageSort, SearchLineagesPayload } from "@/types/lineages";
import { handleErrorKey } from "@/inject/App";
import { searchLineages } from "@/api/lineages";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { parseBoolean, parseNumber } = parsingUtils;
const { rt, t, tm } = useI18n();

const isLoading = ref<boolean>(false);
const lineages = ref<LineageModel[]>([]);
const timestamp = ref<number>(0);
const total = ref<number>(0);

const attribute = computed<Attribute | undefined>(() => (route.query.attribute?.toString() as Attribute) ?? undefined);
const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 10);
const isDescending = computed<boolean>(() => parseBoolean(route.query.isDescending?.toString()) ?? false);
const language = computed<string>(() => route.query.language?.toString() ?? "");
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const search = computed<string>(() => route.query.search?.toString() ?? "");
const size = computed<SizeCategory>(() => (route.query.size?.toString() as SizeCategory) ?? undefined);
const sort = computed<string>(() => route.query.sort?.toString() ?? "");

const sortOptions = computed<SelectOption[]>(() =>
  arrayUtils.orderBy(
    Object.entries(tm(rt("lineages.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

function onCreated(lineage: LineageModel): void {
  toasts.success("lineages.created");
  router.push({ name: "LineageEdit", params: { id: lineage.id } });
}

async function refresh(): Promise<void> {
  const payload: SearchLineagesPayload = {
    ids: [],
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => Boolean(term))
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    attribute: attribute.value,
    languageId: language.value,
    sizeCategory: size.value,
    sort: sort.value ? [{ field: sort.value as LineageSort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results = await searchLineages(payload);
    if (now === timestamp.value) {
      lineages.value = results.items;
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

function setQuery(key: string, value: string): void {
  const query = { ...route.query, [key]: value };
  switch (key) {
    case "attribute":
    case "language":
    case "search":
    case "size":
    case "count":
      query.page = "1";
      break;
  }
  router.replace({ ...route, query });
}

watch(
  () => route,
  (route) => {
    if (route.name === "LineageList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: objectUtils.isEmpty(query)
            ? {
                attribute: "",
                language: "",
                search: "",
                size: "",
                sort: "Name",
                isDescending: "false",
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
</script>

<template>
  <main class="container">
    <h1>{{ t("lineages.list") }}</h1>
    <AppBreadcrumb :current="t('lineages.list')" @error="handleError" />
    <div class="my-3">
      <TarButton
        class="me-1"
        :disabled="isLoading"
        icon="fas fa-rotate"
        :loading="isLoading"
        :status="t('loading')"
        :text="t('actions.refresh')"
        @click="refresh()"
      />
      <CreateLineage class="ms-1" @created="onCreated" @error="handleError" />
    </div>
    <div class="row">
      <AttributeSelect class="col-lg-4" :model-value="attribute" validation="server" @update:model-value="setQuery('attribute', $event ?? '')" />
      <LanguageSelect class="col-lg-4" :model-value="language" validation="server" @update:model-value="setQuery('language', $event ?? '')" />
      <SizeCategorySelect class="col-lg-4" :model-value="size" validation="server" @update:model-value="setQuery('size', $event ?? '')" />
    </div>
    <div class="row">
      <SearchInput class="col-lg-4" :model-value="search" @update:model-value="setQuery('search', $event ?? '')" />
      <SortSelect
        class="col-lg-4"
        :descending="isDescending"
        :model-value="sort"
        :options="sortOptions"
        @descending="setQuery('isDescending', $event.toString())"
        @update:model-value="setQuery('sort', $event ?? '')"
      />
      <CountSelect class="col-lg-4" :model-value="count" @update:model-value="setQuery('count', ($event ?? 10).toString())" />
    </div>
    <template v-if="lineages.length">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("lineages.sort.options.Name") }}</th>
            <th scope="col">{{ t("game.size.category") }}</th>
            <th scope="col">{{ t("lineages.sort.options.UpdatedOn") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="lineage in lineages" :key="lineage.id">
            <td>
              <RouterLink :to="{ name: 'LineageEdit', params: { id: lineage.id } }"><font-awesome-icon icon="fas fa-edit" />{{ lineage.name }}</RouterLink>
            </td>
            <td>{{ t(`game.size.categories.${lineage.size.category}`) }}</td>
            <td><StatusBlock :actor="lineage.updatedBy" :date="lineage.updatedOn" /></td>
          </tr>
        </tbody>
      </table>
      <AppPagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event.toString())" />
    </template>
    <p v-else>{{ t("lineages.empty") }}</p>
  </main>
</template>
