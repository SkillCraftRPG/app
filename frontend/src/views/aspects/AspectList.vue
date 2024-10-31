<script setup lang="ts">
import { TarButton, parsingUtils, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils, objectUtils } from "logitar-js";
import { computed, inject, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppPagination from "@/components/shared/AppPagination.vue";
import CountSelect from "@/components/shared/CountSelect.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { AspectModel, AspectSort, SearchAspectsPayload } from "@/types/aspects";
import { handleErrorKey } from "@/inject/App";
import { searchAspects } from "@/api/aspects";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const { parseBoolean, parseNumber } = parsingUtils;
const { rt, t, tm } = useI18n();

const aspects = ref<AspectModel[]>([]);
const isLoading = ref<boolean>(false);
const timestamp = ref<number>(0);
const total = ref<number>(0);

const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 10);
const isDescending = computed<boolean>(() => parseBoolean(route.query.isDescending?.toString()) ?? false);
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");

const sortOptions = computed<SelectOption[]>(() =>
  arrayUtils.orderBy(
    Object.entries(tm(rt("aspects.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

type AttributeCategory = "mandatory" | "optional";
function formatAttributes(aspect: AspectModel, category: AttributeCategory): string {
  const attributes: string[] =
    category === "mandatory"
      ? [aspect.attributes.mandatory1, aspect.attributes.mandatory2]
          .filter((attribute) => Boolean(attribute))
          .map((attribute) => t(`game.attributes.${attribute}`))
      : [aspect.attributes.optional1, aspect.attributes.optional2]
          .filter((attribute) => Boolean(attribute))
          .map((attribute) => t(`game.attributes.${attribute}`));
  return attributes.join("<br />") || "—";
}
function formatSkills(aspect: AspectModel): string {
  const skills: string[] = [aspect.skills.discounted1, aspect.skills.discounted2].filter((skill) => Boolean(skill)).map((skill) => t(`game.skills.${skill}`));
  return skills.join("<br />") || "—";
}

async function refresh(): Promise<void> {
  const payload: SearchAspectsPayload = {
    ids: [],
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => Boolean(term))
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    sort: sort.value ? [{ field: sort.value as AspectSort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results = await searchAspects(payload);
    if (now === timestamp.value) {
      aspects.value = results.items;
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
    case "count":
      query.page = "1";
      break;
  }
  router.replace({ ...route, query });
}

watch(
  () => route,
  (route) => {
    if (route.name === "AspectList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: objectUtils.isEmpty(query)
            ? {
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
    <h1>{{ t("aspects.title.list") }}</h1>
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
      <TarButton class="ms-1" icon="fas fa-plus" :text="t('actions.create')" variant="success" />
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
    <template v-if="aspects.length">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("aspects.sort.options.Name") }}</th>
            <th scope="col">{{ t("aspects.attributes.mandatory") }}</th>
            <th scope="col">{{ t("aspects.attributes.optional") }}</th>
            <th scope="col">{{ t("aspects.skills") }}</th>
            <th scope="col">{{ t("aspects.sort.options.UpdatedOn") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="aspect in aspects" :key="aspect.id">
            <td>
              <RouterLink :to="{ name: 'AspectEdit', params: { id: aspect.id } }"><font-awesome-icon icon="fas fa-edit" />{{ aspect.name }}</RouterLink>
            </td>
            <td v-html="formatAttributes(aspect, 'mandatory')"></td>
            <td v-html="formatAttributes(aspect, 'optional')"></td>
            <td v-html="formatSkills(aspect)"></td>
            <td><StatusBlock :actor="aspect.updatedBy" :date="aspect.updatedOn" /></td>
          </tr>
        </tbody>
      </table>
      <AppPagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event.toString())" />
    </template>
    <p v-else>{{ t("aspects.empty") }}</p>
  </main>
</template>
