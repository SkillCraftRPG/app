<script setup lang="ts">
import { TarButton, parsingUtils, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils, objectUtils } from "logitar-js";
import { computed, inject, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppBreadcrumb from "@/components/shared/AppBreadcrumb.vue";
import AppPagination from "@/components/shared/AppPagination.vue";
import CountSelect from "@/components/shared/CountSelect.vue";
import CreateTalent from "@/components/talents/CreateTalent.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import TalentIcon from "@/components/talents/TalentIcon.vue";
import TalentSelect from "@/components/talents/TalentSelect.vue";
import TierSelect from "@/components/shared/TierSelect.vue";
import YesNoSelect from "@/components/shared/YesNoSelect.vue";
import type { TalentModel, TalentSort, SearchTalentsPayload } from "@/types/talents";
import { handleErrorKey } from "@/inject/App";
import { searchTalents } from "@/api/talents";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { parseBoolean, parseNumber } = parsingUtils;
const { rt, t, tm } = useI18n();

const isLoading = ref<boolean>(false);
const talents = ref<TalentModel[]>([]);
const timestamp = ref<number>(0);
const total = ref<number>(0);

const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 10);
const isDescending = computed<boolean>(() => parseBoolean(route.query.isDescending?.toString()) ?? false);
const multiple = computed<boolean | undefined>(() => {
  const multiple: string | undefined = route.query.multiple?.toString();
  return multiple === "" ? undefined : parseBoolean(multiple);
});
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const required = computed<string>(() => route.query.required?.toString() ?? "");
const search = computed<string>(() => route.query.search?.toString() ?? "");
const skill = computed<boolean | undefined>(() => {
  const skill: string | undefined = route.query.skill?.toString();
  return skill === "" ? undefined : parseBoolean(skill);
});
const sort = computed<string>(() => route.query.sort?.toString() ?? "");
const tier = computed<number | undefined>(() => {
  const tier: string | undefined = route.query.tier?.toString();
  return tier === "" ? undefined : parseNumber(tier);
});

const sortOptions = computed<SelectOption[]>(() =>
  arrayUtils.orderBy(
    Object.entries(tm(rt("talents.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

function onCreated(talent: TalentModel): void {
  toasts.success("talents.created");
  router.push({ name: "TalentEdit", params: { id: talent.id } });
}

async function refresh(): Promise<void> {
  const payload: SearchTalentsPayload = {
    ids: [],
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => Boolean(term))
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    allowMultiplePurchases: multiple.value,
    hasSkill: skill.value,
    requiredTalentId: required.value,
    tier: typeof tier.value === "number" ? { values: [tier.value], operator: "eq" } : undefined,
    sort: sort.value ? [{ field: sort.value as TalentSort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results = await searchTalents(payload);
    if (now === timestamp.value) {
      talents.value = results.items;
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
    case "multiple":
    case "required":
    case "search":
    case "skill":
    case "tier":
    case "count":
      query.page = "1";
      break;
  }
  router.replace({ ...route, query });
}

watch(
  () => route,
  (route) => {
    if (route.name === "TalentList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: objectUtils.isEmpty(query)
            ? {
                multiple: "",
                required: "",
                search: "",
                skill: "",
                tier: "",
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
    <h1>{{ t("talents.list") }}</h1>
    <AppBreadcrumb :current="t('talents.list')" @error="handleError" />
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
      <CreateTalent class="ms-1" @created="onCreated" @error="handleError" />
    </div>
    <div class="row">
      <TalentSelect
        class="col-lg-3"
        label="talents.required"
        :model-value="required"
        placeholder="talents.select.none"
        validation="server"
        @error="handleError"
        @update:model-value="setQuery('required', $event?.toString() ?? '')"
      />
      <TierSelect
        class="col-lg-3"
        :model-value="tier"
        placeholder="game.tier.all"
        validation="server"
        @update:model-value="setQuery('tier', $event?.toString() ?? '')"
      />
      <YesNoSelect
        class="col-lg-3"
        id="multiple"
        label="talents.allowMultiplePurchases"
        :model-value="multiple"
        validation="server"
        @update:model-value="setQuery('multiple', $event?.toString() ?? '')"
      />
      <YesNoSelect
        class="col-lg-3"
        id="skill"
        label="talents.hasSkill"
        :model-value="skill"
        validation="server"
        @update:model-value="setQuery('skill', $event?.toString() ?? '')"
      />
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
    <template v-if="talents.length">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("talents.sort.options.Name") }}</th>
            <th scope="col">{{ t("game.tier.label") }}</th>
            <th scope="col">{{ t("talents.required") }}</th>
            <th scope="col">{{ t("game.skill") }}</th>
            <th scope="col">{{ t("talents.allowMultiplePurchases") }}</th>
            <th scope="col">{{ t("talents.sort.options.UpdatedOn") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="talent in talents" :key="talent.id">
            <td>
              <RouterLink :to="{ name: 'TalentEdit', params: { id: talent.id } }"> <font-awesome-icon icon="fas fa-edit" />{{ talent.name }} </RouterLink>
            </td>
            <td>{{ talent.tier }}</td>
            <td>
              <RouterLink v-if="talent.requiredTalent" :to="{ name: 'TalentEdit', params: { id: talent.requiredTalent.id } }" target="_blank">
                <TalentIcon />{{ talent.requiredTalent.name }}
              </RouterLink>
              <template v-else>{{ "—" }}</template>
            </td>
            <td>{{ talent.skill ? t(`game.skills.options.${talent.skill}`) : "—" }}</td>
            <td>
              <template v-if="talent.allowMultiplePurchases"> <font-awesome-icon icon="fas fa-check" /> {{ t("yes") }} </template>
              <template v-else> <font-awesome-icon icon="fas fa-times" /> {{ t("no") }} </template>
            </td>
            <td><StatusBlock :actor="talent.updatedBy" :date="talent.updatedOn" /></td>
          </tr>
        </tbody>
      </table>
      <AppPagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event.toString())" />
    </template>
    <p v-else>{{ t("talents.empty") }}</p>
  </main>
</template>
