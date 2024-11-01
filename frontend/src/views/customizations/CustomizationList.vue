<script setup lang="ts">
import { TarButton, parsingUtils, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils, objectUtils } from "logitar-js";
import { computed, inject, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppPagination from "@/components/shared/AppPagination.vue";
import CountSelect from "@/components/shared/CountSelect.vue";
import CreateCustomization from "@/components/customizations/CreateCustomization.vue";
import CustomizationTypeSelect from "@/components/customizations/CustomizationTypeSelect.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { CustomizationModel, CustomizationSort, CustomizationType, SearchCustomizationsPayload } from "@/types/customizations";
import { handleErrorKey } from "@/inject/App";
import { searchCustomizations } from "@/api/customizations";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { parseBoolean, parseNumber } = parsingUtils;
const { rt, t, tm } = useI18n();

const customizations = ref<CustomizationModel[]>([]);
const isLoading = ref<boolean>(false);
const timestamp = ref<number>(0);
const total = ref<number>(0);

const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 10);
const isDescending = computed<boolean>(() => parseBoolean(route.query.isDescending?.toString()) ?? false);
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");
const type = computed<CustomizationType>(() => (route.query.type?.toString() as CustomizationType) ?? "");

const sortOptions = computed<SelectOption[]>(() =>
  arrayUtils.orderBy(
    Object.entries(tm(rt("customizations.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

function onCreated(customization: CustomizationModel): void {
  toasts.success("customizations.created");
  router.push({ name: "CustomizationEdit", params: { id: customization.id } });
}

async function refresh(): Promise<void> {
  const payload: SearchCustomizationsPayload = {
    ids: [],
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => Boolean(term))
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    type: type.value,
    sort: sort.value ? [{ field: sort.value as CustomizationSort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results = await searchCustomizations(payload);
    if (now === timestamp.value) {
      customizations.value = results.items;
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
    case "search":
    case "type":
    case "count":
      query.page = "1";
      break;
  }
  router.replace({ ...route, query });
}

watch(
  () => route,
  (route) => {
    if (route.name === "CustomizationList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: objectUtils.isEmpty(query)
            ? {
                search: "",
                type: "",
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
    <h1>{{ t("customizations.list") }}</h1>
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
      <CreateCustomization class="ms-1" @created="onCreated" @error="handleError" />
    </div>
    <div class="row">
      <CustomizationTypeSelect class="col-lg-3" :model-value="type" @update:model-value="setQuery('type', $event ?? '')" />
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
    <template v-if="customizations.length">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("customizations.sort.options.Name") }}</th>
            <th scope="col">{{ t("customizations.type.label") }}</th>
            <th scope="col">{{ t("customizations.sort.options.UpdatedOn") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="customization in customizations" :key="customization.id">
            <td>
              <RouterLink :to="{ name: 'CustomizationEdit', params: { id: customization.id } }">
                <font-awesome-icon icon="fas fa-edit" />{{ customization.name }}
              </RouterLink>
            </td>
            <td>{{ t(`customizations.type.options.${customization.type}`) }}</td>
            <td><StatusBlock :actor="customization.updatedBy" :date="customization.updatedOn" /></td>
          </tr>
        </tbody>
      </table>
      <AppPagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event.toString())" />
    </template>
    <p v-else>{{ t("customizations.empty") }}</p>
  </main>
</template>
