<script setup lang="ts">
import { TarButton, parsingUtils, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils, objectUtils } from "logitar-js";
import { computed, inject, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppBreadcrumb from "@/components/shared/AppBreadcrumb.vue";
import AppPagination from "@/components/shared/AppPagination.vue";
import CountSelect from "@/components/shared/CountSelect.vue";
import CreateEducation from "@/components/educations/CreateEducation.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SkillSelect from "@/components/game/SkillSelect.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { EducationModel, EducationSort, SearchEducationsPayload } from "@/types/educations";
import type { Skill } from "@/types/game";
import { handleErrorKey } from "@/inject/App";
import { searchEducations } from "@/api/educations";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { parseBoolean, parseNumber } = parsingUtils;
const { rt, t, tm } = useI18n();

const educations = ref<EducationModel[]>([]);
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
    Object.entries(tm(rt("educations.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

function onCreated(education: EducationModel): void {
  toasts.success("educations.created");
  router.push({ name: "EducationEdit", params: { id: education.id } });
}

async function refresh(): Promise<void> {
  const payload: SearchEducationsPayload = {
    ids: [],
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => Boolean(term))
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    skill: skill.value,
    sort: sort.value ? [{ field: sort.value as EducationSort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results = await searchEducations(payload);
    if (now === timestamp.value) {
      educations.value = results.items;
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
    if (route.name === "EducationList") {
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
    <h1>{{ t("educations.list") }}</h1>
    <AppBreadcrumb :current="t('educations.list')" @error="handleError" />
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
      <CreateEducation class="ms-1" @created="onCreated" @error="handleError" />
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
    <template v-if="educations.length">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("educations.sort.options.Name") }}</th>
            <th scope="col">{{ t("game.skill") }}</th>
            <th scope="col">{{ t("game.startingWealth") }}</th>
            <th scope="col">{{ t("educations.sort.options.UpdatedOn") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="education in educations" :key="education.id">
            <td>
              <RouterLink :to="{ name: 'EducationEdit', params: { id: education.id } }">
                <font-awesome-icon icon="fas fa-edit" />{{ education.name }}
              </RouterLink>
            </td>
            <td>{{ education.skill ? t(`game.skills.options.${education.skill}`) : "—" }}</td>
            <td>{{ education.wealthMultiplier ? `×${education.wealthMultiplier}` : "—" }}</td>
            <td><StatusBlock :actor="education.updatedBy" :date="education.updatedOn" /></td>
          </tr>
        </tbody>
      </table>
      <AppPagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event.toString())" />
    </template>
    <p v-else>{{ t("educations.empty") }}</p>
  </main>
</template>
