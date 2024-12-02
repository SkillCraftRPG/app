<script setup lang="ts">
import { TarButton, parsingUtils, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils, objectUtils } from "logitar-js";
import { computed, inject, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppBreadcrumb from "@/components/shared/AppBreadcrumb.vue";
import AppPagination from "@/components/shared/AppPagination.vue";
import CountSelect from "@/components/shared/CountSelect.vue";
import CreateLanguage from "@/components/languages/CreateLanguage.vue";
import LanguageLink from "@/components/languages/LanguageLink.vue";
import ScriptSelect from "@/components/languages/ScriptSelect.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { LanguageModel, LanguageSort, SearchLanguagesPayload } from "@/types/languages";
import { handleErrorKey } from "@/inject/App";
import { searchLanguages } from "@/api/languages";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { parseBoolean, parseNumber } = parsingUtils;
const { rt, t, tm } = useI18n();

const isLoading = ref<boolean>(false);
const languages = ref<LanguageModel[]>([]);
const timestamp = ref<number>(0);
const total = ref<number>(0);

const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 10);
const isDescending = computed<boolean>(() => parseBoolean(route.query.isDescending?.toString()) ?? false);
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const script = computed<string>(() => route.query.script?.toString() ?? "");
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");

const sortOptions = computed<SelectOption[]>(() =>
  arrayUtils.orderBy(
    Object.entries(tm(rt("languages.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

function onCreated(aspect: LanguageModel): void {
  toasts.success("languages.created");
  router.push({ name: "LanguageEdit", params: { id: aspect.id } });
}

async function refresh(): Promise<void> {
  const payload: SearchLanguagesPayload = {
    ids: [],
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => Boolean(term))
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    script: script.value,
    sort: sort.value ? [{ field: sort.value as LanguageSort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results = await searchLanguages(payload);
    if (now === timestamp.value) {
      languages.value = results.items;
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
    case "script":
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
    if (route.name === "LanguageList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: objectUtils.isEmpty(query)
            ? {
                script: "",
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
    <h1>{{ t("languages.list") }}</h1>
    <AppBreadcrumb :current="t('languages.list')" @error="handleError" />
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
      <CreateLanguage class="ms-1" @created="onCreated" @error="handleError" />
    </div>
    <div class="row">
      <ScriptSelect class="col-lg-3" :model-value="script" @update:model-value="setQuery('script', $event ?? '')" @error="handleError" />
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
    <template v-if="languages.length">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("languages.sort.options.Name") }}</th>
            <th scope="col">{{ t("languages.script.label") }}</th>
            <th scope="col">{{ t("languages.typicalSpeakers") }}</th>
            <th scope="col">{{ t("languages.sort.options.UpdatedOn") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="language in languages" :key="language.id">
            <td>
              <LanguageLink edit :language="language" />
            </td>
            <td>{{ language.script ?? "—" }}</td>
            <td>{{ language.typicalSpeakers ?? "—" }}</td>
            <td><StatusBlock :actor="language.updatedBy" :date="language.updatedOn" /></td>
          </tr>
        </tbody>
      </table>
      <AppPagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event.toString())" />
    </template>
    <p v-else>{{ t("languages.empty") }}</p>
  </main>
</template>
