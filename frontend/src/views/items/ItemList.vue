<script setup lang="ts">
import { TarButton, parsingUtils, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils, objectUtils } from "logitar-js";
import { computed, inject, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppPagination from "@/components/shared/AppPagination.vue";
import CountSelect from "@/components/shared/CountSelect.vue";
import CreateItem from "@/components/items/CreateItem.vue";
import ItemCategorySelect from "@/components/items/ItemCategorySelect.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { ItemCategory, ItemModel, ItemSort, SearchItemsPayload } from "@/types/items";
import { handleErrorKey } from "@/inject/App";
import { searchItems } from "@/api/items";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { parseBoolean, parseNumber } = parsingUtils;
const { rt, t, tm } = useI18n();

const isLoading = ref<boolean>(false);
const items = ref<ItemModel[]>([]);
const timestamp = ref<number>(0);
const total = ref<number>(0);

const category = computed<ItemCategory>(() => (route.query.category?.toString() as ItemCategory) ?? "");
const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 10);
const isDescending = computed<boolean>(() => parseBoolean(route.query.isDescending?.toString()) ?? false);
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");

const sortOptions = computed<SelectOption[]>(() =>
  arrayUtils.orderBy(
    Object.entries(tm(rt("items.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

function onCreated(item: ItemModel): void {
  toasts.success("items.created");
  router.push({ name: "ItemEdit", params: { id: item.id } });
}

async function refresh(): Promise<void> {
  const payload: SearchItemsPayload = {
    ids: [],
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => Boolean(term))
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    category: category.value,
    sort: sort.value ? [{ field: sort.value as ItemSort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results = await searchItems(payload);
    if (now === timestamp.value) {
      items.value = results.items;
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
    case "category":
    case "search":
    case "count":
      query.page = "1";
      break;
  }
  router.replace({ ...route, query });
}

watch(
  () => route,
  (route) => {
    if (route.name === "ItemList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: objectUtils.isEmpty(query)
            ? {
                category: "",
                search: "",
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
    <h1>{{ t("items.list") }}</h1>
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
      <CreateItem class="ms-1" @created="onCreated" @error="handleError" />
    </div>
    <div class="row">
      <ItemCategorySelect class="col-lg-3" :model-value="category" validation="server" @update:model-value="setQuery('category', $event ?? '')" />
      <SearchInput class="col-lg-3" :model-value="search" @update:model-value="setQuery('search', $event ?? '')" />
      <SortSelect
        class="col-lg-3"
        :descending="isDescending"
        :model-value="sort"
        :options="sortOptions"
        @descending="setQuery('isDescending', $event.toString())"
        @update:model-value="setQuery('sort', $event ?? '')"
      />
      <CountSelect class="col-lg-3" :model-value="count" @update:model-value="setQuery('count', ($event ?? 10).toString())" />
    </div>
    <template v-if="items.length">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("items.sort.options.Name") }}</th>
            <th scope="col">{{ t("items.category.label") }}</th>
            <th scope="col">{{ t("items.sort.options.Value") }}</th>
            <th scope="col">{{ t("items.sort.options.Weight") }}</th>
            <th scope="col">{{ t("items.sort.options.UpdatedOn") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="item in items" :key="item.id">
            <td>
              <RouterLink :to="{ name: 'ItemEdit', params: { id: item.id } }"> <font-awesome-icon icon="fas fa-edit" />{{ item.name }} </RouterLink>
            </td>
            <td>{{ t(`items.category.options.${item.category}`) }}</td>
            <td>{{ item.value ?? "—" }}</td>
            <td>{{ item.weight ?? "—" }}</td>
            <td><StatusBlock :actor="item.updatedBy" :date="item.updatedOn" /></td>
          </tr>
        </tbody>
      </table>
      <AppPagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event.toString())" />
    </template>
    <p v-else>{{ t("items.empty") }}</p>
  </main>
</template>
