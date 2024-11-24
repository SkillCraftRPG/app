<script setup lang="ts">
import { TarButton, parsingUtils, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils, objectUtils } from "logitar-js";
import { computed, inject, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppBreadcrumb from "@/components/shared/AppBreadcrumb.vue";
import AppPagination from "@/components/shared/AppPagination.vue";
import CountSelect from "@/components/shared/CountSelect.vue";
import CreateCaste from "@/components/castes/CreateCaste.vue";
import FeaturesBlock from "@/components/castes/FeaturesBlock.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SkillSelect from "@/components/game/SkillSelect.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { CasteModel, CasteSort, SearchCastesPayload } from "@/types/castes";
import type { Skill } from "@/types/game";
import { handleErrorKey } from "@/inject/App";
import { searchCastes } from "@/api/castes";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { parseBoolean, parseNumber } = parsingUtils;
const { rt, t, tm } = useI18n();

const castes = ref<CasteModel[]>([]);
const isLoading = ref<boolean>(false);
const timestamp = ref<number>(0);
const total = ref<number>(0);

const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 10);
const isDescending = computed<boolean>(() => parseBoolean(route.query.isDescending?.toString()) ?? false);
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const search = computed<string>(() => route.query.search?.toString() ?? "");
const skill = computed<Skill | undefined>(() => (route.query.skill?.toString() as Skill) ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");

const sortOptions = computed<SelectOption[]>(() =>
  arrayUtils.orderBy(
    Object.entries(tm(rt("castes.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

function onCreated(caste: CasteModel): void {
  toasts.success("castes.created");
  router.push({ name: "CasteEdit", params: { id: caste.id } });
}

async function refresh(): Promise<void> {
  const payload: SearchCastesPayload = {
    ids: [],
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => Boolean(term))
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    skill: skill.value,
    sort: sort.value ? [{ field: sort.value as CasteSort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results = await searchCastes(payload);
    if (now === timestamp.value) {
      castes.value = results.items;
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
    case "skill":
    case "count":
      query.page = "1";
      break;
  }
  router.replace({ ...route, query });
}

watch(
  () => route,
  (route) => {
    if (route.name === "CasteList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: objectUtils.isEmpty(query)
            ? {
                search: "",
                skill: "",
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
    <h1>{{ t("castes.list") }}</h1>
    <AppBreadcrumb :current="t('castes.list')" @error="handleError" />
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
      <CreateCaste class="ms-1" @created="onCreated" @error="handleError" />
    </div>
    <div class="row">
      <SkillSelect class="col-lg-3" :model-value="skill" validation="server" @update:model-value="setQuery('skill', $event ?? '')" />
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
    <template v-if="castes.length">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("castes.sort.options.Name") }}</th>
            <th scope="col">{{ t("game.skill.label") }}</th>
            <th scope="col">{{ t("game.startingWealth") }}</th>
            <th scope="col">{{ t("castes.features.label") }}</th>
            <th scope="col">{{ t("castes.sort.options.UpdatedOn") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="caste in castes" :key="caste.id">
            <td>
              <RouterLink :to="{ name: 'CasteEdit', params: { id: caste.id } }"><font-awesome-icon icon="fas fa-edit" />{{ caste.name }}</RouterLink>
            </td>
            <td>{{ caste.skill ? t(`game.skill.options.${caste.skill}`) : "—" }}</td>
            <td>{{ caste.wealthRoll ?? "—" }}</td>
            <td><FeaturesBlock :caste="caste" /></td>
            <td><StatusBlock :actor="caste.updatedBy" :date="caste.updatedOn" /></td>
          </tr>
        </tbody>
      </table>
      <AppPagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event.toString())" />
    </template>
    <p v-else>{{ t("castes.empty") }}</p>
  </main>
</template>
